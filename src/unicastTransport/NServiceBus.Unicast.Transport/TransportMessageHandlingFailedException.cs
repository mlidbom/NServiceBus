using System;

namespace NServiceBus.Unicast.Transport
{
    /// <summary>
    /// Exception used to transport exceptions encountered in messagehandlers
    /// </summary>
    public class TransportMessageHandlingFailedException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="originalException"></param>
        public TransportMessageHandlingFailedException(Exception innerException):base("Message handling failed", innerException)
        {
        }
    }
}