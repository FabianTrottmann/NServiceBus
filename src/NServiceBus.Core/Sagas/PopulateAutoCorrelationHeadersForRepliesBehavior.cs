﻿namespace NServiceBus
{
    using System;
    using System.Threading.Tasks;
    using NServiceBus.Saga;
    using Pipeline;
    using Pipeline.Contexts;

    class PopulateAutoCorrelationHeadersForRepliesBehavior : Behavior<OutgoingContext>
    {
        public override Task Invoke(OutgoingContext context, Func<Task> next)
        {
            AttachSagaDetailsToOutGoingMessage(context);

            FlowDetailsForRequestingSagaToOutgoingMessage(context);

            return next();
        }

        static void FlowDetailsForRequestingSagaToOutgoingMessage(OutgoingContext context)
        {
            TransportMessage incomingMessage;

            if (context.TryGet(TransportReceiveContext.IncomingPhysicalMessageKey, out incomingMessage))
            {
                //flow the the saga id of the calling saga (if any) to outgoing message in order to support autocorrelation
                if (context.Intent == MessageIntentEnum.Reply)
                {
                    string sagaId;

                    if (incomingMessage.Headers.TryGetValue(Headers.OriginatingSagaId, out sagaId))
                    {
                        context.Headers[Headers.SagaId] = sagaId;
                    }

                    string sagaType;

                    if (incomingMessage.Headers.TryGetValue(Headers.OriginatingSagaType, out sagaType))
                    {
                        context.Headers[Headers.SagaType] = sagaType;
                    }
                }
            }
        }

        static void AttachSagaDetailsToOutGoingMessage(OutgoingContext context)
        {
            ActiveSagaInstance saga;


            //attach the current saga details to the outgoing headers for correlation
            if (context.TryGet(out saga) && HasBeenFound(saga))
            {
                context.Headers[Headers.OriginatingSagaId] = saga.SagaId;
                context.Headers[Headers.OriginatingSagaType] = saga.Metadata.SagaType.AssemblyQualifiedName;
            }
        }

        static bool HasBeenFound(ActiveSagaInstance saga)
        {
            return !saga.NotFound;
        }
    }
}