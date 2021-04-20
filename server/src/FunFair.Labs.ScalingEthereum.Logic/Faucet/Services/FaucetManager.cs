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

        private readonly EthereumAmount _ethToGive;
        private readonly ExecutionEnvironment _executionEnvironment;
        private readonly IContractInfo _faucetContract;

        private readonly IFaucetDataManager _faucetDataManager;
        private readonly IWhiteListedIpAddressIdentifier _fundingWhiteList;
        private readonly ILogger<FaucetManager> _logger;
        private readonly EthereumAmount _maximumRecipientEthBalance;
        private readonly Token _maximumRecipientTokenBalance;
        private readonly Erc20TokenContractInfo _tokenContract;
        private readonly Token _tokenToGiven;

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
            this._ethereumAccountManager = ethereumAccountManager;
            this._ethToGive = faucetConfiguration.EthToGive;
            this._tokenToGiven = faucetConfiguration.TokenToGive;
            this._maximumRecipientEthBalance = this._ethToGive / 2;
            this._maximumRecipientTokenBalance = this._tokenToGiven / 2;

            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

            this._tokenContract = (Erc20TokenContractInfo) contractInfoRegistry.FindContractInfo(WellKnownContracts.Token);
            this._faucetContract = contractInfoRegistry.FindContractInfo(WellKnownContracts.Faucet);
        }

        /// <inheritdoc />
        public async Task<FaucetDrip> OpenAsync(IPAddress ipAddress, INetworkAccount recipient, INetworkBlockHeader networkBlockHeader, CancellationToken cancellationToken)
        {
            const string sourceName = @"Test Network Faucet";

            // ETH & FUN source (Faucet account - no need to be a signing account)
            if (!this._faucetContract.Addresses.TryGetValue(key: recipient.Network, out ContractAddress? contractAddress))
            {
                this._logger.LogCritical($"{recipient.Network.Name}: Cannot issue ETH from faucet. Faucet not available on network");

                throw new InsufficientTokenException();
            }

            INetworkSigningAccount networkSigningAccount = this._ethereumAccountManager.GetAccount(network: recipient.Network);

            NetworkContract fundsSourceContract = new(network: recipient.Network, contractAddress: contractAddress);

            IReadOnlyList<EthereumAddress> networkAccounts = new EthereumAddress[] {contractAddress, recipient.Address};

            IReadOnlyDictionary<EthereumAddress, EthereumAmount> accountBalances =
                await this._ethereumAccountBalanceSource.GetAccountBalancesAsync(networkAccounts: networkAccounts, networkBlockHeader: networkBlockHeader, cancellationToken: CancellationToken.None);

            // get the SOURCE account's balance
            if (!accountBalances.TryGetValue(key: contractAddress, out EthereumAmount? sourceAccountBalance))
            {
                this._logger.LogCritical($"{recipient.Network.Name}: Could not retrieve balance for {networkSigningAccount.Address}");

                throw new InsufficientTokenException();
            }

            // get the account's current ETH balance
            if (!accountBalances.TryGetValue(key: recipient.Address, out EthereumAmount? recipientEthBalance))
            {
                this._logger.LogCritical($"{recipient.Network.Name}: Could not retrieve balance for {recipient.Address}");

                throw new InsufficientTokenException();
            }

            IReadOnlyDictionary<EthereumAddress, Erc20TokenBalance> balances = await this._ethereumAccountBalanceSource.GetErc20TokenBalancesAsync(
                addresses: networkAccounts,
                networkBlockHeader: networkBlockHeader,
                tokenContract: this._tokenContract,
                cancellationToken: cancellationToken);

            // get the SOURCE account's current FUN balance
            if (!balances.TryGetValue(key: fundsSourceContract.Address, out Erc20TokenBalance sourceTokenBalance))
            {
                this._logger.LogCritical($"{recipient.Network.Name}: Could not retrieve balance for {fundsSourceContract.Address}");

                throw new InsufficientTokenException();
            }

            Token sourceBalanceForToken = new(sourceTokenBalance);

            // get the recipient account's current FUN balance
            if (!balances.TryGetValue(key: recipient.Address, out Erc20TokenBalance recipientTokenBalance))
            {
                this._logger.LogCritical($"{recipient.Network.Name}: Could not retrieve balance for {recipient.Address}");

                throw new InsufficientTokenException();
            }

            Token recipientBalanceForToken = new(recipientTokenBalance);

            bool giveToken = recipientBalanceForToken < this._maximumRecipientTokenBalance;
            bool giveEth = recipientEthBalance < this._maximumRecipientEthBalance;

            this._logger.LogInformation(
                $"{recipient.Network.Name}: Source: {sourceAccountBalance.ToFormattedUnitWithSymbol()} {sourceBalanceForToken.ToFormattedUnitWithSymbol()} Recipient: {recipientEthBalance.ToFormattedUnitWithSymbol()} {recipientBalanceForToken.ToFormattedUnitWithSymbol()} Issue: ETH: {giveEth} FUN: {giveToken} Max Eth: {this._maximumRecipientEthBalance.ToFormattedUnitWithSymbol()} Max FUN: {this._maximumRecipientTokenBalance.ToFormattedUnitWithSymbol()}");

            if (!giveToken && !giveEth)
            {
                if (this._executionEnvironment.IsDevelopmentOrTest())
                {
                    this._logger.LogWarning($"{recipient.Network.Name}: Could not issue eth to {recipient.Address} - Recipient balance > max");
                }

                throw new TooMuchTokenException();
            }

            if (!await this.IsAllowedToIssueFromFaucetAsync(ipAddress: ipAddress, recipientAddress: recipient.Address))
            {
                throw new TooFrequentTokenException();
            }

            EthereumAmount ethAmount = giveEth ? this.CalculateAmountOfEthToIssueFromFaucet(recipientEthBalance) : EthereumAmount.Zero;
            Token tokenAmount = giveToken ? this.CalculateAmountOfFunToIssueFromFaucet(recipientBalanceForToken) : Token.Zero;

            Issuance issuance =
                new(recipient: recipient, ethToIssue: ethAmount, tokenToIssue: tokenAmount, sourceAccountBalance: sourceAccountBalance, sourceTokenBalance: sourceBalanceForToken, sourceName:
                    sourceName, fundsSourceContract: fundsSourceContract, contractInfo: this._faucetContract);

            try
            {
                PendingTransaction tx = await this.IssueFundsAsync(networkSigningAccount: networkSigningAccount,
                                                                   issuance: issuance,
                                                                   new TransactionContext(contextType: WellKnownContracts.Faucet, recipient.Address.ToString()));

                return new FaucetDrip(transaction: tx, ethAmount: ethAmount, tokenAmount: tokenAmount);
            }
            catch (TransactionWillAlwaysFailException exception)
            {
                this._logger.LogCritical(new EventId(exception.HResult), exception: exception, $"{issuance.Recipient.Network.Name}: Cannot issue FUN from faucet: {exception.Message}");

                throw new InsufficientTokenException(message: "Could not request fun from faucet", innerException: exception);
            }
        }

        private Token CalculateAmountOfFunToIssueFromFaucet(Token recipientTokenBalance)
        {
            return this._tokenToGiven - recipientTokenBalance;
        }

        private EthereumAmount CalculateAmountOfEthToIssueFromFaucet(EthereumAmount recipientEthBalance)
        {
            return this._ethToGive - recipientEthBalance;
        }

        private static Token MinSourceTokenAccountBalance(Token amountToGive)
        {
            return amountToGive * 2;
        }

        private static EthereumAmount MinSourceEthAccountBalance(EthereumAmount amountToGive)
        {
            return new(amountToGive.Value * 2);
        }

        private Task<PendingTransaction> IssueFundsAsync(INetworkSigningAccount networkSigningAccount, Issuance issuance, TransactionContext context)
        {
            this._logger.LogDebug($"Faucet Contract Balances: {issuance.SourceAccountBalance.ToFormattedUnitWithSymbol()}, {issuance.SourceTokenBalance.ToFormattedUnitWithSymbol()}");

            EthereumAmount minEth = MinSourceEthAccountBalance(issuance.EthToIssue);
            Token minFun = MinSourceTokenAccountBalance(issuance.TokenToIssue);

            // don't allow the source account balance to go too low
            if (issuance.SourceAccountBalance < minEth)
            {
                string message =
                    $"{issuance.Recipient.Network.Name}: Cannot issue ETH from {issuance.SourceName}. Faucet Contract {issuance.FundsSourceContract.Address} is low on ETH (Has {issuance.SourceAccountBalance.ToFormattedUnitWithSymbol()}. Minimum {minEth.ToFormattedUnitWithSymbol()})";
                this._logger.LogCritical(message);

                throw new InsufficientTokenException(message);
            }

            if (issuance.SourceTokenBalance < minFun)
            {
                string message =
                    $"{issuance.Recipient.Network.Name}: Cannot issue TOKEN from {issuance.SourceName}. Faucet Contract {issuance.FundsSourceContract.Address} is low on FUN (Has {issuance.SourceTokenBalance.ToFormattedUnitWithSymbol()}. Minimum {minFun.ToFormattedUnitWithSymbol()})";
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