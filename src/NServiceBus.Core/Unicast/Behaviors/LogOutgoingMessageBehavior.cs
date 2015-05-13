﻿namespace NServiceBus
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using NServiceBus.Logging;
    using NServiceBus.Pipeline;
    using NServiceBus.Pipeline.Contexts;
    using NServiceBus.Unicast;

    class LogOutgoingMessageBehavior : Behavior<OutgoingContext>
    {
        public override Task Invoke(OutgoingContext context, Func<Task> next)
        {
            var options = context.DeliveryMessageOptions as SendMessageOptions;

            if (options != null && log.IsDebugEnabled && context.MessageInstance != null)
            {
                var destination = options.Destination;

                log.DebugFormat("Sending message '{0}' with id '{1}' to destination '{2}'.\n" +
                                "ToString() of the message yields: {3}\n" +
                                "Message headers:\n{4}",
                                context.MessageType.AssemblyQualifiedName,
                    context.MessageId,
                    destination,
                    context.MessageInstance,
                    string.Join(", ", context.Headers.Select(h => h.Key + ":" + h.Value).ToArray()));
            }

            return next();
        }

        static ILog log = LogManager.GetLogger("LogOutgoingMessage");
    }
}