using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FunFair.Common.Environment;
using FunFair.Common.Environment.Extensions;
using FunFair.Common.Middleware;
using FunFair.Ethereum.Balances.Interfaces;
using FunFair.Ethereum.Client.Interfaces.Exceptions;
using FunFair.Ethereum.Contracts;
using FunFair.Ethereum.Contracts.Erc20;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Extensions;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Ethereum.Transactions.Data.Interfaces.Models;
using FunFair.Ethereum.Transactions.Interfaces;
using FunFair.Ethereum.Wallet.Interfaces;
using FunFair.Labs.ScalingEthereum.Contracts;
using FunFair.Labs.ScalingEthereum.Contracts.Faucet.Transactions;
using FunFair.Labs.ScalingEthereum.Data.Interfaces.Faucet;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.Logic.Exceptions;
using FunFair.Labs.ScalingEthereum.Logic.Faucet.Models;
using Microsoft.Extensions.Logging;

namespace FunFair.Labs.ScalingEthereum.Logic.Faucet.Services
{
    /// <summary>
    ///     Faucet manager.
    /// </summary>
    public sealed class FaucetManager : IFaucetManager
    {
        private readonly IEthereumAccountBalanceSource _ethereumAccountBalanceSource;
        private readonly IEthereumAccountManager _ethereumAccountManager;

        private readonly ExecutionEnvironment _executionEnvironment;
        private readonly IContractInfo _faucetContract;

        private readonly IFaucetDataManager _faucetDataManager;
        private readonly IWhiteListedIpAddressIdentifier _fundingWhiteList;
        private readonly ILogger<FaucetManager> _logger;

        private readonly Limits<EthereumAmount> _nativeCurrencyLimits;
        private readonly Erc20TokenContractInfo _tokenContract;
        private readonly Limits<Token> _tokenCurrencyLimits;

        private readonly ITransactionExecutorFactory _transactionExecutorFactory;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="transactionExecutorFactory">Transaction execution</param>
        /// <param name="faucetDataManager">Data manager for faucet.</param>
        /// <param name="ethereumAccountBalanceSource">Balance source for Ethereum/ERC20 tokens.</param>
        /// <param name="contractInfoRegistry">Registry of contracts.</param>
        /// <param name="houseFundingIpWhiteList">IP Whitelist for faucet/funding operations.</param>
        /// <param name="executionEnvironment">The Execution Environment.</param>
        /// <param name="faucetConfiguration">Faucet configuration.</param>
        /// <param name="ethereumAccountManager">Ethereum account manager</param>
        /// <param name="logger">Logging.</param>
        public FaucetManager(ITransactionExecutorFactory transactionExecutorFactory,
                             IFaucetDataManager faucetDataManager,
                             IEthereumAccountBalanceSource ethereumAccountBalanceSource,
                             IContractInfoRegistry contractInfoRegistry,
                             IWhiteListedIpAddressIdentifier houseFundingIpWhiteList,
                             ExecutionEnvironment executionEnvironment,
                             IFaucetConfiguration faucetConfiguration,
                             IEthereumAccountManager ethereumAccountManager,
                             ILogger<FaucetManager> logger)
        {
            if (contractInfoRegistry == null)
            {
                throw new ArgumentNullException(nameof(contractInfoRegistry));
            }

            if (faucetConfiguration == null)
            {
                throw new ArgumentNullException(nameof(faucetConfiguration));
            }

            this._faucetDataManager = faucetDataManager ?? throw new ArgumentNullException(nameof(faucetDataManager));
            this._ethereumAccountBalanceSource = ethereumAccountBalanceSource ?? throw new ArgumentNullException(nameof(ethereumAccountBalanceSource));
            this._transactionExecutorFactory = transactionExecutorFactory ?? throw new ArgumentNullException(nameof(transactionExecutorFactory));
            this._fundingWhiteList = houseFundingIpWhiteList ?? throw new ArgumentNullException(nameof(houseFundingIpWhiteList));
            this._executionEnvironment = executionEnvironment;
            this._ethereumAccountManager = ethereumAccountManager ?? throw new ArgumentNullException(nameof(ethereumAccountManager));

            this._nativeCurrencyLimits = new Limits<EthereumAmount>(amountToIssue: faucetConfiguration.EthToGive, faucetConfiguration.EthToGive / 2);
            this._tokenCurrencyLimits = new Limits<Token>(amountToIssue: faucetConfiguration.TokenToGive, faucetConfiguration.TokenToGive / 2);

            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

            this._tokenContract = (Erc20TokenContractInfo) contractInfoRegistry.FindContractInfo(WellKnownContracts.Token);
            this._faucetContract = contractInfoRegistry.FindContractInfo(WellKnownContracts.Faucet);
        }

        /// <inheritdoc />
        public async Task<FaucetDrip> OpenAsync(IPAddress ipAddress, INetworkAccount recipient, INetworkBlockHeader networkBlockHeader, CancellationToken cancellationToken)
        {
            const string sourceName = @"Test Network Faucet";

            if (!this._faucetContract.Addresses.TryGetValue(key: recipient.Network, out ContractAddress? faucetContractAddress))
            {
                this._logger.LogCritical($"{recipient.Network.Name}: Cannot issue {recipient.Network.NativeCurrency} from faucet. Faucet not available on network");

                throw new InsufficientTokenException();
            }

            INetworkSigningAccount networkSigningAccount = this._ethereumAccountManager.GetAccount(network: recipient.Network);

            (EthereumAmount sourceAccountBalance, EthereumAmount recipientEthBalance) = await this.GetNativeCurrencyBalancesAsync(
                recipient: recipient,
                faucetContractAddress: faucetContractAddress,
                networkBlockHeader: networkBlockHeader,
                cancellationToken: cancellationToken);

            (Token sourceBalanceForToken, Token recipientBalanceForToken) =
                await this.GetTokenBalancesAsync(recipient: recipient, networkBlockHeader: networkBlockHeader, faucetContractAddress: faucetContractAddress, cancellationToken: cancellationToken);

            (EthereumAmount nativeCurrencyAmount, Token tokenAmount) = this.CalculateFundsToIssue(recipient: recipient,
                                                                                       recipientNativeCurrencyBalance: recipientEthBalance,
                                                                                       recipientTokenBalance: recipientBalanceForToken,
                                                                                       faucetNativeCurrencyBalance: sourceAccountBalance,
                                                                                       faucetTokenBalance: sourceBalanceForToken);

            if (!await this.IsAllowedToIssueFromFaucetAsync(ipAddress: ipAddress, recipientAddress: recipient.Address))
            {
                throw new TooFrequentTokenException();
            }

            Issuance issuance =
                new(recipient: recipient, ethToIssue: nativeCurrencyAmount, tokenToIssue: tokenAmount, sourceAccountBalance: sourceAccountBalance, sourceTokenBalance: sourceBalanceForToken, sourceName:
                    sourceName, new NetworkContract(network: recipient.Network, contractAddress: faucetContractAddress), contractInfo: this._faucetContract);

            try
            {
                PendingTransaction tx = await this.IssueFundsAsync(networkSigningAccount: networkSigningAccount,
                                                                   issuance: issuance,
                                                                   new TransactionContext(contextType: WellKnownContracts.Faucet, recipient.Address.ToString()));

                await this.RecordSuccessfulFaucetDripAsync(recipient: recipient, nativeCurrencyAmount: nativeCurrencyAmount, tokenAmount: tokenAmount, ipAddress: ipAddress);

                return new FaucetDrip(transaction: tx, ethAmount: nativeCurrencyAmount, tokenAmount: tokenAmount);
            }
            catch (TransactionWillAlwaysFailException exception)
            {
                this._logger.LogCritical(new EventId(exception.HResult),
                                         exception: exception,
                                         $"{issuance.Recipient.Network.Name}: Cannot issue {this._tokenContract.Symbol} from faucet: {exception.Message}");

                throw new InsufficientTokenException(message: "Could not request funds from faucet", innerException: exception);
            }
        }

        private Task RecordSuccessfulFaucetDripAsync(INetworkAccount recipient, EthereumAmount nativeCurrencyAmount, Token tokenAmount, IPAddress ipAddress)
        {
            return this._faucetDataManager.RecordFundsIssuedAsync(recipient: recipient, nativeCurrencyAmount: nativeCurrencyAmount, tokenAmount: tokenAmount, ipAddress: ipAddress);
        }

        private (EthereumAmount nativeCurrencyAmount, Token tokenAmount) CalculateFundsToIssue(INetworkAccount recipient,
                                                                                               EthereumAmount recipientNativeCurrencyBalance,
                                                                                               Token recipientTokenBalance,
                                                                                               EthereumAmount faucetNativeCurrencyBalance,
                                                                                               Token faucetTokenBalance)
        {
            bool giveNativeCurrency = recipientNativeCurrencyBalance < this._nativeCurrencyLimits.MaximumRecipientBalance;
            bool giveToken = recipientTokenBalance < this._tokenCurrencyLimits.MaximumRecipientBalance;

            this._logger.LogInformation(
                $"{recipient.Network.Name}: Source: {faucetNativeCurrencyBalance.ToFormattedUnitWithSymbol()} {faucetTokenBalance.ToFormattedUnitWithSymbol()} Recipient: {recipientNativeCurrencyBalance.ToFormattedUnitWithSymbol()} {recipientTokenBalance.ToFormattedUnitWithSymbol()} Issue: ETH: {giveNativeCurrency} FUN: {giveToken} Max Eth: {this._nativeCurrencyLimits.MaximumRecipientBalance.ToFormattedUnitWithSymbol()} Max FUN: {this._tokenCurrencyLimits.MaximumRecipientBalance.ToFormattedUnitWithSymbol()}");

            if (!giveNativeCurrency && !giveToken)
            {
                if (this._executionEnvironment.IsDevelopmentOrTest())
                {
                    this._logger.LogWarning(
                        $"{recipient.Network.Name}: Could not issue {recipient.Network.NativeCurrency} or {this._tokenContract.Symbol} to {recipient.Address} - Recipient balance > max");
                }

                throw new TooMuchTokenException();
            }

            EthereumAmount nativeCurrencyAmount = giveNativeCurrency ? this.CalculateAmountOfNativeCurrencyToIssueFromFaucet(recipientNativeCurrencyBalance) : EthereumAmount.Zero;
            Token tokenAmount = giveToken ? this.CalculateAmountOfTokenToIssueFromFaucet(recipientTokenBalance) : Token.Zero;

            return (nativeCurrencyAmount, tokenAmount);
        }

        private async Task<(Token sourceBalanceForToken, Token recipientBalanceForToken)> GetTokenBalancesAsync(INetworkAccount recipient,
                                                                                                                INetworkBlockHeader networkBlockHeader,
                                                                                                                ContractAddress faucetContractAddress,
                                                                                                                CancellationToken cancellationToken)
        {
            IReadOnlyDictionary<EthereumAddress, Erc20TokenBalance> balances = await this._ethereumAccountBalanceSource.GetErc20TokenBalancesAsync(
                addresses: new EthereumAddress[] {faucetContractAddress, recipient.Address},
                networkBlockHeader: networkBlockHeader,
                tokenContract: this._tokenContract,
                cancellationToken: cancellationToken);

            if (!balances.TryGetValue(key: faucetContractAddress, out Erc20TokenBalance sourceTokenBalance))
            {
                this._logger.LogCritical($"{recipient.Network.Name}: Could not retrieve {this._tokenContract.Symbol} balance for {faucetContractAddress}");

                throw new InsufficientTokenException();
            }

            if (!balances.TryGetValue(key: recipient.Address, out Erc20TokenBalance recipientTokenBalance))
            {
                this._logger.LogCritical($"{recipient.Network.Name}: Could not retrieve {this._tokenContract.Symbol} balance for {recipient.Address}");

                throw new InsufficientTokenException();
            }

            return (sourceBalanceForToken: new(sourceTokenBalance), recipientBalanceForToken: new(recipientTokenBalance));
        }

        private async Task<(EthereumAmount sourceAccountBalance, EthereumAmount recipientEthBalance)> GetNativeCurrencyBalancesAsync(
            INetworkAccount recipient,
            ContractAddress faucetContractAddress,
            INetworkBlockHeader networkBlockHeader,
            CancellationToken cancellationToken)
        {
            IReadOnlyDictionary<EthereumAddress, EthereumAmount> accountBalances = await this._ethereumAccountBalanceSource.GetAccountBalancesAsync(
                networkAccounts: new EthereumAddress[] {faucetContractAddress, recipient.Address},
                networkBlockHeader: networkBlockHeader,
                cancellationToken: cancellationToken);

            // get the SOURCE account's balance
            if (!accountBalances.TryGetValue(key: faucetContractAddress, out EthereumAmount? sourceAccountBalance))
            {
                this._logger.LogCritical($"{recipient.Network.Name}: Could not retrieve {recipient.Network.NativeCurrency} balance for {faucetContractAddress}");

                throw new InsufficientTokenException();
            }

            if (!accountBalances.TryGetValue(key: recipient.Address, out EthereumAmount? recipientEthBalance))
            {
                this._logger.LogCritical($"{recipient.Network.Name}: Could not retrieve {recipient.Network.NativeCurrency} balance for {recipient.Address}");

                throw new InsufficientTokenException();
            }

            return (sourceAccountBalance, recipientEthBalance);
        }

        private Token CalculateAmountOfTokenToIssueFromFaucet(Token recipientTokenBalance)
        {
            return this._tokenCurrencyLimits.AmountToIssue - recipientTokenBalance;
        }

        private EthereumAmount CalculateAmountOfNativeCurrencyToIssueFromFaucet(EthereumAmount recipientEthBalance)
        {
            return this._nativeCurrencyLimits.AmountToIssue - recipientEthBalance;
        }

        private static Token MinSourceTokenAccountBalance(Token amountToGive)
        {
            return amountToGive * 2;
        }

        private static EthereumAmount MinSourceNativeCurrencyAccountBalance(EthereumAmount amountToGive)
        {
            return new(amountToGive.Value * 2);
        }

        private Task<PendingTransaction> IssueFundsAsync(INetworkSigningAccount networkSigningAccount, Issuance issuance, TransactionContext context)
        {
            this._logger.LogDebug($"Faucet Contract Balances: {issuance.SourceAccountBalance.ToFormattedUnitWithSymbol()}, {issuance.SourceTokenBalance.ToFormattedUnitWithSymbol()}");

            EthereumAmount minEth = MinSourceNativeCurrencyAccountBalance(issuance.EthToIssue);
            Token minFun = MinSourceTokenAccountBalance(issuance.TokenToIssue);

            // don't allow the source account balance to go too low
            if (issuance.SourceAccountBalance < minEth)
            {
                string message =
                    $"{issuance.Recipient.Network.Name}: Cannot issue {issuance.Recipient.Network.NativeCurrency} from {issuance.SourceName}. Faucet Contract {issuance.FundsSourceContract.Address} is low on ETH (Has {issuance.SourceAccountBalance.ToFormattedUnitWithSymbol()}. Minimum {minEth.ToFormattedUnitWithSymbol()})";
                this._logger.LogCritical(message);

                throw new InsufficientTokenException(message);
            }

            if (issuance.SourceTokenBalance < minFun)
            {
                string message =
                    $"{issuance.Recipient.Network.Name}: Cannot issue {this._tokenContract.Symbol} from {issuance.SourceName}. Faucet Contract {issuance.FundsSourceContract.Address} is low on FUN (Has {issuance.SourceTokenBalance.ToFormattedUnitWithSymbol()}. Minimum {minFun.ToFormattedUnitWithSymbol()})";
                this._logger.LogCritical(message);

                throw new InsufficientTokenException(message);
            }

            return issuance.SendFundsAsync(networkSigningAccount: networkSigningAccount, context: context, transactionExecutorFactory: this._transactionExecutorFactory);
        }

        private async Task<bool> IsAllowedToIssueFromFaucetAsync(IPAddress ipAddress, AccountAddress recipientAddress)
        {
            if (this._executionEnvironment.IsLocalDevelopmentOrTest() || this.IsFromAlwaysAllowedAddress(ipAddress))
            {
                return true;
            }

            return await this._faucetDataManager.IsAllowedToIssueFromFaucetAsync(ipAddress: ipAddress, address: recipientAddress);
        }

        private bool IsFromAlwaysAllowedAddress(IPAddress ipAddress)
        {
            return this._fundingWhiteList.IsWhitelisted(ipAddress: ipAddress, out bool _);
        }

        private sealed class Limits<T>
        {
            public Limits(T amountToIssue, T maximumRecipientBalance)
            {
                this.AmountToIssue = amountToIssue;
                this.MaximumRecipientBalance = maximumRecipientBalance;
            }

            public T AmountToIssue { get; }

            public T MaximumRecipientBalance { get; }
        }

        private sealed class Issuance
        {
            private readonly IContractInfo _contractInfo;

            public Issuance(INetworkAccount recipient,
                            EthereumAmount ethToIssue,
                            Token tokenToIssue,
                            EthereumAmount sourceAccountBalance,
                            Token sourceTokenBalance,
                            string sourceName,
                            NetworkContract fundsSourceContract,
                            IContractInfo contractInfo)
            {
                this.Recipient = recipient ?? throw new ArgumentNullException(nameof(recipient));
                this.EthToIssue = ethToIssue ?? throw new ArgumentNullException(nameof(ethToIssue));
                this.TokenToIssue = tokenToIssue ?? throw new ArgumentNullException(nameof(tokenToIssue));
                this.SourceAccountBalance = sourceAccountBalance ?? throw new ArgumentNullException(nameof(sourceAccountBalance));
                this.SourceTokenBalance = sourceTokenBalance ?? throw new ArgumentNullException(nameof(sourceTokenBalance));
                this.SourceName = sourceName ?? throw new ArgumentNullException(nameof(sourceName));
                this.FundsSourceContract = fundsSourceContract ?? throw new ArgumentNullException(nameof(fundsSourceContract));
                this._contractInfo = contractInfo ?? throw new ArgumentNullException(nameof(contractInfo));
            }

            public INetworkAccount Recipient { get; }

            public EthereumAmount EthToIssue { get; }

            public Token TokenToIssue { get; }

            public EthereumAmount SourceAccountBalance { get; }

            public Token SourceTokenBalance { get; }

            public string SourceName { get; }

            public NetworkContract FundsSourceContract { get; }

            public Task<PendingTransaction> SendFundsAsync(INetworkSigningAccount networkSigningAccount, TransactionContext context, ITransactionExecutorFactory transactionExecutorFactory)
            {
                if (this.EthToIssue != EthereumAmount.Zero && this.TokenToIssue != Token.Zero)
                {
                    return this.IssueEthereumAndFunAsync(networkSigningAccount: networkSigningAccount, context: context, transactionExecutorFactory: transactionExecutorFactory);
                }

                if (this.TokenToIssue != Token.Zero)
                {
                    return this.IssueFunAsync(networkSigningAccount: networkSigningAccount, context: context, transactionExecutorFactory: transactionExecutorFactory);
                }

                return this.IssueEthereumAsync(networkSigningAccount: networkSigningAccount, context: context, transactionExecutorFactory: transactionExecutorFactory);
            }

            private Task<PendingTransaction> IssueEthereumAsync(INetworkSigningAccount networkSigningAccount, TransactionContext context, ITransactionExecutorFactory transactionExecutorFactory)
            {
                return this._contractInfo.SubmitTransactionAsync(transactionExecutorFactory: transactionExecutorFactory,
                                                                 account: networkSigningAccount,
                                                                 priority: TransactionPriority.NORMAL,
                                                                 input: new DistributeEthInput(recipient: this.Recipient.Address, ethAmount: this.EthToIssue),
                                                                 context: context,
                                                                 cancellationToken: CancellationToken.None);
            }

            private Task<PendingTransaction> IssueFunAsync(INetworkSigningAccount networkSigningAccount, TransactionContext context, ITransactionExecutorFactory transactionExecutorFactory)
            {
                return this._contractInfo.SubmitTransactionAsync(transactionExecutorFactory: transactionExecutorFactory,
                                                                 account: networkSigningAccount,
                                                                 priority: TransactionPriority.NORMAL,
                                                                 input: new DistributeTokenInput(recipient: this.Recipient.Address, tokenAmount: this.TokenToIssue),
                                                                 context: context,
                                                                 cancellationToken: CancellationToken.None);
            }

            private Task<PendingTransaction> IssueEthereumAndFunAsync(INetworkSigningAccount networkSigningAccount, TransactionContext context, ITransactionExecutorFactory transactionExecutorFactory)
            {
                return this._contractInfo.SubmitTransactionAsync(transactionExecutorFactory: transactionExecutorFactory,
                                                                 account: networkSigningAccount,
                                                                 priority: TransactionPriority.NORMAL,
                                                                 input: new DistributeTokenAndEthInput(recipient: this.Recipient.Address, ethAmount: this.EthToIssue, tokenAmount: this.TokenToIssue),
                                                                 context: context,
                                                                 cancellationToken: CancellationToken.None);
            }
        }
    }
}