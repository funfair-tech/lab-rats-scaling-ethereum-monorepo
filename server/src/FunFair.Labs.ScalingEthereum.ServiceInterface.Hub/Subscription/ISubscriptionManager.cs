using System.Collections.Generic;

namespace FunFair.Labs.ScalingEthereum.ServiceInterface.Hub.Subscription
{
    public interface ISubscriptionManager
    {
        /// <summary>
        ///     Subscribe members to given group
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="groupMember"></param>
        /// <returns></returns>

        // ReSharper disable once UnusedMethodReturnValue.Global
        IList<string> Subscribe(string groupName, IList<string> groupMember);

        /// <summary>
        ///     Attempts to remove and return the value with the specified key from the
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="groupMembers"></param>
        /// <returns></returns>
        bool Unsubscribe(string groupName, out IList<string> groupMembers);
    }
}