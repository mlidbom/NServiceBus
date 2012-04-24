﻿using System.Collections.Generic;
using Common.Logging;
using NServiceBus.Faults;
using NServiceBus.ObjectBuilder;
using System.Linq;

namespace NServiceBus.Faults.Forwarder
{
	using System;
	using Unicast.Transport;
    using Unicast.Queuing;

    /// <summary>
    /// Implementation of IManageMessageFailures by forwarding messages
    /// using ISendMessages.
    /// </summary>
    public class FaultManager : IManageMessageFailures
    {
        void IManageMessageFailures.SerializationFailedForMessage(TransportMessage message, Exception e)
        {
            SendFailureMessage(message, e, "SerializationFailed");
        }

        void IManageMessageFailures.ProcessingAlwaysFailsForMessage(TransportMessage message, Exception e)
        {
            if (SanitizeProcessingExceptions)
                e = ExceptionSanitizer.Sanitize(e);

            var id = message.Id;
            SendFailureMessage(message, e, "ProcessingFailed"); //overwrites message.Id
            message.Id = id;
        }

        void IManageMessageFailures.Init(Address address)
        {
            localAddress = address;
        }

        // Intentionally service-locate ISendMessages to avoid circular
        // resolution problem in the container
        void SendFailureMessage(TransportMessage message, Exception e, string reason)
        {
            SetExceptionHeaders(message, e, reason);
            var sender = Configure.Instance.Builder.Build<ISendMessages>();
            try
            {
                sender.Send(message, ErrorQueue);
            }
            catch (Exception exception)
            {
                var qnfEx = exception as QueueNotFoundException;
                string errorMessage;
                if (qnfEx != null)
                    errorMessage = string.Format("Could not forward failed message to error queue '{0}' as it could not be found.", qnfEx.Queue);
                else
                    errorMessage = string.Format("Could not forward failed message to error queue, reason: {0}.", exception.ToString());
                Logger.Fatal(errorMessage);
                throw new InvalidOperationException(errorMessage, exception);
            }
            
        }

        /// <summary>
        /// Should be used by programmer, not administrator.
        /// Sets <see cref="IBuilder"/> implementation that will be used to 
        /// dynamically instantiate and execute failure header providers.
        /// </summary>
        public IBuilder Builder { get; set; }

        void SetExceptionHeaders(TransportMessage message, Exception e, string reason)
        {
            message.Headers["NServiceBus.ExceptionInfo.Reason"] = reason;
			message.Headers["NServiceBus.ExceptionInfo.ExceptionType"] = e.GetType().FullName;

			if (e.InnerException != null)
				message.Headers["NServiceBus.ExceptionInfo.InnerExceptionType"] = e.InnerException.GetType().FullName;
            
			message.Headers["NServiceBus.ExceptionInfo.HelpLink"] = e.HelpLink;
            message.Headers["NServiceBus.ExceptionInfo.Message"] = e.Message;
            message.Headers["NServiceBus.ExceptionInfo.Source"] = e.Source;
            message.Headers["NServiceBus.ExceptionInfo.StackTrace"] = CreateStackTrace(e);

            message.Headers[TransportHeaderKeys.OriginalId] = message.Id;

            var failedQ = localAddress ?? Address.Local;

            message.Headers[FaultsHeaderKeys.FailedQ] = failedQ.ToString();
            message.Headers["NServiceBus.TimeOfFailure"] = DateTime.UtcNow.ToWireFormattedString();

            CollectHeadersFromFailureHeaderProviders(message, e);
        }

        private string CreateStackTrace(Exception exception)
        {
            var stackTraces = 
                GetNestedExceptionsList(exception)
                .Reverse()
                .Select(currentException => string.Format("Exception:{1}{0}Message:{2}{0}{0}{3}",
                                                          Environment.NewLine,
                                                          currentException.GetType().FullName,
                                                          currentException.Message, currentException.StackTrace))
                .ToList();

            return string.Join(string.Format("{0}   ---End of inner stack trace---{0}{0}", Environment.NewLine),
                               stackTraces);
        }

        private IEnumerable<Exception> GetNestedExceptionsList(Exception exception)
        {
            yield return exception;
            while (exception.InnerException != null)
            {
                yield return exception.InnerException;
                exception = exception.InnerException;
            }
        }

        private void CollectHeadersFromFailureHeaderProviders(TransportMessage message, Exception e)
        {
            IEnumerable<IProvideFailureHeaders> providers = Builder.BuildAll<IProvideFailureHeaders>();
            foreach(var provider in  providers)
            {
                foreach (var exceptionHeader in provider.GetExceptionHeaders(message, e))
                {
                    message.Headers[exceptionHeader.Key] = exceptionHeader.Value;
                }
            }
        }

        /// <summary>
        /// Endpoint to which message failures are forwarded
        /// </summary>
        public Address ErrorQueue { get; set; }

        /// <summary>
        /// Indicates of exceptions should be sanitized before sending them on
        /// </summary>
        public bool SanitizeProcessingExceptions { get; set; }

        Address localAddress;
        static ILog Logger = LogManager.GetLogger("NServiceBus");
    }
}
