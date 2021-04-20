using FunFair.Common.Data.Builders;
using FunFair.Common.Data.Extensions;
using FunFair.Ethereum.Events.Data.Interfaces.Models;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Events.Builders.ObjectBuilders.Entities;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Events.Builders.ObjectBuilders
{
    /// <summary>
    ///     Builder of <see cref="EventContractCheckpoint" /> from <see cref="EventContractCheckpointEntity" />
    /// </summary>
    public sealed class EventContractCheckpointBuilder : IObjectBuilder<EventContractCheckpointEntity, EventContractCheckpoint>
    {
        /// <inheritdoc />
        public EventContractCheckpoint? Build(EventContractCheckpointEntity? source)
        {
            if (source == null)
            {
                return null;
            }

            return new EventContractCheckpoint(contractAddress: source.ContractAddress ?? source.DataError(x => x.ContractAddress),
                                               firstBlockProcessed: source.StartBlock ?? source.DataError(x => x.StartBlock),
                                               lastBlockProcessed: source.CurrentBlock ?? source.DataError(x => x.CurrentBlock),
                                               lastUpdated: source.LastUpdated);
        }
    }
}