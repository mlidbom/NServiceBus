using System;

namespace NServiceBus.Unicast.Transport.Transactional
{
    public class MessageModuleFailedException : Exception
    {
        public MessageModuleFailedException(Exception innerException)
            : base("Failed while invoking message modules", innerException)
        {

        }
    }
}