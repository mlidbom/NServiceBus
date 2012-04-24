using System;

namespace NServiceBus.Unicast.Transport.Transactional
{
    public class MessageHandlingFailedException : Exception
    {
        public MessageHandlingFailedException(Exception innerException)
            : base("Failed while handling messages", innerException)
        {

        }
    }
}