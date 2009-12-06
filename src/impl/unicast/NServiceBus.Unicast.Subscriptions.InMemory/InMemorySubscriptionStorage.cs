﻿using System.Collections.Generic;
using System.Linq;

namespace NServiceBus.Unicast.Subscriptions.InMemory
{
    /// <summary>
    /// Inmemory implementation of the subscription storage
    /// </summary>
    public class InMemorySubscriptionStorage : ISubscriptionStorage
    {
        /// <summary>
        /// Adds the given subscription to the inmemory list
        /// </summary>
        /// <param name="client"></param>
        /// <param name="messageTypes"></param>
        public void Subscribe(string client, IList<string> messageTypes)
        {
            messageTypes.ToList().ForEach(m =>
                                              {
                                                  if (!storage.ContainsKey(m))
                                                      storage[m] = new List<string>();

                                                  if (!storage[m].Contains(client))
                                                      storage[m].Add(client);
                                              });
        }

        /// <summary>
        /// Removes the subscription
        /// </summary>
        /// <param name="client"></param>
        /// <param name="messageTypes"></param>
        public void Unsubscribe(string client, IList<string> messageTypes)
        {
            messageTypes.ToList().ForEach(m =>
                                              {
                                                  if (storage.ContainsKey(m))
                                                      storage[m].Remove(client);
                                              });
        }

        /// <summary>
        /// Lists all subscribers for the given message types
        /// </summary>
        /// <param name="messageTypes"></param>
        /// <returns></returns>
        public IList<string> GetSubscribersForMessage(IList<string> messageTypes)
        {
            var result = new List<string>();
            messageTypes.ToList().ForEach(m =>
                                              {
                                                  if (storage.ContainsKey(m))
                                                      result.AddRange(storage[m]);
                                              });

            return result;
        }

        public void Init()
        {
        }

        private readonly Dictionary<string, List<string>> storage = new Dictionary<string, List<string>>();
    }
}