﻿using NServiceBus.ObjectBuilder;

namespace NServiceBus.Host.Internal.ProfileHandlers
{
    internal class LiteProfileHandler : IHandleProfile<Lite>, IWantTheEndpointConfig
    {
        void IHandleProfile.ProfileActivated()
        {
            Configure.Instance.Configurer.ConfigureComponent<InMemorySagaPersister>(ComponentCallModelEnum.Singleton);

            if (Config is AsA_Publisher)
                Configure.Instance.InMemorySubscriptionStorage();
        }

        public IConfigureThisEndpoint Config { get; set; }
    }
}
