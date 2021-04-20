using System.Collections.Generic;
using NonBlocking;

namespace FunFair.Labs.ScalingEthereum.ServiceInterface.Hub.Subscription
{
    /// <inheritdoc />
    public sealed class SubscriptionManager : ISubscriptionManager
    {
        private readonly ConcurrentDictionary<string, IList<string>> _subscribedGroups = new();

        /// <summary>
        ///     Subscribe members to given group
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="groupMember"></param>
        /// <returns></returns>
        public IList<string> Subscribe(string groupName, IList<string> groupMember)
        {
            return this._subscribedGroups.AddOrUpdate(key: groupName,
                                                      addValue: groupMember,
                                                      updateValueFactory: (_, list) =>
                                                                          {
                                                                              lock (list)
                                                                              {
                                                                                  foreach (string entry in groupMember)
                                                                                  {
                                                                                      if (!list.Contains(entry))
                                                                                      {
                                                                                          list.Add(entry);
                                                                                      }
                                                                                  }

                                                                                  return list;
                                                                              }
                                                                          });
        }

        /// <summary>
        ///     Attempts to remove and return the value with the specified key from the
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="groupMembers"></param>
        /// <returns></returns>
        public bool Unsubscribe(string groupName, out IList<string> groupMembers)
        {
            return this._subscribedGroups.TryRemove(key: groupName, out groupMembers!);
        }
    }
}