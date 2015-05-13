﻿namespace NServiceBus.Pipeline
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using NServiceBus.Unicast.Transport;

    [DebuggerDisplay("{type.Name}")]
    class BehaviorInstance
    {
        readonly IBehavior instance;
        readonly Type type;
        readonly IBehaviorInvoker invoker;

        public BehaviorInstance(Type behaviorType, IBehavior instance)
        {
            this.instance = instance;
            type = behaviorType;
            invoker = CreateInvoker(type);
        }

        static IBehaviorInvoker CreateInvoker(Type type)
        {
            var behaviorInterface = type.GetInterfaces().First(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IBehavior<,>));
            var invokerType = typeof(BehaviorInvoker<,>).MakeGenericType(behaviorInterface.GetGenericArguments());
            return (IBehaviorInvoker) Activator.CreateInstance(invokerType);
        }

        public Type Type { get { return type; } }

        public Task Invoke(BehaviorContext context, Func<BehaviorContext, Task> next)
        {
            return invoker.Invoke(instance, context, next);
        }

        public void Initialize(PipelineInfo pipelineInfo)
        {
            instance.Initialize(pipelineInfo);
        }

        public Task Cooldown()
        {
            return instance.Cooldown();
        }

        public Task Warmup()
        {
            return instance.Warmup();
        }
    }
}