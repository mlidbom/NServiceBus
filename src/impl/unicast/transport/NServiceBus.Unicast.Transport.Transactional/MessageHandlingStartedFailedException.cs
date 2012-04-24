using System;

namespace NServiceBus.Unicast.Transport.Transactional
{
    public class MessageHandlingStartedFailedException : Exception
    {
        public MessageHandlingStartedFailedException(Exception innerException)
            : base("Failed while initializing message processing", innerException)
        {

        }
    }
}