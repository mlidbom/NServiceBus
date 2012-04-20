using System;
using System.Collections.Generic;
using NServiceBus.Unicast.Transport;

namespace NServiceBus.Faults
{
    /// <summary>
    /// Interface for providing extra header information when messages fail.
    /// </summary>
    public interface IProvideFailureHeaders
    {
        /// <summary>
        /// Invoked to collect informational headers intended to help with debugging.
        /// </summary>
        /// <param name="message">The failing message</param>
        /// <param name="e">The thrown exception</param>
        IDictionary<string,string> GetExceptionHeaders(TransportMessage message, Exception e);
    }
}