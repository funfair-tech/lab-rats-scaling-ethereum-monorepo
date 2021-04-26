using System;
using FunFair.Common.Interfaces;
using FunFair.Ethereum.Blocks.Interfaces;
using FunFair.Ethereum.Blocks.Interfaces.EventArguments;
using FunFair.Ethereum.DataTypes;
using FunFair.Labs.ScalingEthereum.Data.Interfaces.GameRound;
using NonBlocking;

namespace FunFair.Labs.ScalingEthereum.Logic.Games.EventHandlers
{
    /// <summary>
    ///     Extension methods for calculating the amount of time left in a game.
    /// </summary>
    public sealed class GameRoundTimeCalculator : IGameRoundTimeCalculator
    {
        private readonly ConcurrentDictionary<EthereumNetwork, BlockStatus> _blockStatus;
        private readonly IDateTimeSource _dateTimeSource;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly IEthereumBlockStatus _ethereumBlockStatus;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="ethereumBlockStatus">Ethereum block status.</param>
        /// <param name="dateTimeSource">Source of date/time.</param>
        public GameRoundTimeCalculator(IEthereumBlockStatus ethereumBlockStatus, IDateTimeSource dateTimeSource)
        {
            this._ethereumBlockStatus = ethereumBlockStatus ?? throw new ArgumentNullException(nameof(ethereumBlockStatus));
            this._dateTimeSource = dateTimeSource ?? throw new ArgumentNullException(nameof(dateTimeSource));

            this._blockStatus = new ConcurrentDictionary<EthereumNetwork, BlockStatus>();
            this._ethereumBlockStatus.OnNewLatestBlock += (_, networkBlockHeader) => this.NewBlockReceived(networkBlockHeader);
        }

        /// <inheritdoc />
        public int CalculateTimeLeft(GameRound gameRound)
        {
            if (gameRound.DateStarted == null)
            {
                return 0;
            }

            DateTime gameWillEnd = gameRound.DateStarted.Value.AddSeconds(gameRound.RoundDuration.TotalSeconds);

            DateTime currentAmortizedTime = this.GetCurrentTime(gameRound.GameContract.Network);

            return (int) (gameWillEnd - currentAmortizedTime).TotalSeconds;
        }

        /// <inheritdoc />
        public int CalculateSecondsUntilNextRound(GameRound gameRound)
        {
            if (gameRound.DateClosed == null)
            {
                return 0;
            }

            DateTime currentAmortizedTime = this.GetCurrentTime(gameRound.GameContract.Network);
            DateTime nextStartTime = gameRound.DateClosed.Value + GameRoundParameters.InterGameDelay;

            return (int) (nextStartTime - currentAmortizedTime).TotalSeconds;
        }

        private void NewBlockReceived(NewBlockEventArgs networkBlockHeader)
        {
            BlockStatus blockStatus = this.GetBlockStatus(networkBlockHeader.Network);

            blockStatus.UpdateHeader(networkBlockHeader.LatestBlock);
        }

        private BlockStatus GetBlockStatus(EthereumNetwork network)
        {
            return this._blockStatus.GetOrAdd(key: network, new BlockStatus(this._dateTimeSource));
        }

        private DateTime GetCurrentTime(EthereumNetwork network)
        {
            BlockStatus blockStatus = this.GetBlockStatus(network);

            return blockStatus.UtcNow();
        }

        private sealed class BlockStatus
        {
            private readonly IDateTimeSource _dateTimeSource;

            private Current? _currentTime;

            public BlockStatus(IDateTimeSource dateTimeSource)
            {
                this._dateTimeSource = dateTimeSource ?? throw new ArgumentNullException(nameof(dateTimeSource));
                this._currentTime = null;
            }

            public void UpdateHeader(INetworkBlockHeader networkBlockHeader)
            {
                if (this._currentTime != null)
                {
                    if (this._currentTime.NetworkBlockHeader.Hash == networkBlockHeader.Hash)
                    {
                        return;
                    }
                }

                DateTime now = this._dateTimeSource.UtcNow();

                // Not sure if the timestamp from the block is good enough - need to experiment or just use the server's time.
                TimeSpan offset = now - networkBlockHeader.Timestamp;

                this._currentTime = new Current(networkBlockHeader: networkBlockHeader, blockSeenTime: now, currentServerTimeOffset: offset);
            }

            public DateTime UtcNow()
            {
                TimeSpan offset = (this._currentTime?.CurrentServerTimeOffset).GetValueOrDefault(TimeSpan.Zero);

                return this._dateTimeSource.UtcNow()
                           .Subtract(offset);
            }

            private sealed class Current
            {
                public Current(INetworkBlockHeader networkBlockHeader, DateTime blockSeenTime, TimeSpan currentServerTimeOffset)
                {
                    this.NetworkBlockHeader = networkBlockHeader;
                    this.BlockSeenTime = blockSeenTime;
                    this.CurrentServerTimeOffset = currentServerTimeOffset;
                }

                public INetworkBlockHeader NetworkBlockHeader { get; }

                // ReSharper disable once UnusedAutoPropertyAccessor.Local
                public DateTime BlockSeenTime { get; }

                public TimeSpan CurrentServerTimeOffset { get; }
            }
        }
    }
}