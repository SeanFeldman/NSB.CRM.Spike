﻿namespace Endpoint1
{
    using System;
    using System.Threading.Tasks;
    using NServiceBus;
    using NServiceBus.Features;
    using NServiceBus.Pipeline;

    public class StampCrmMessagesWithHeaderFeature : Feature
    {
        internal StampCrmMessagesWithHeaderFeature()
        {
            EnableByDefault();
        }

        protected override void Setup(FeatureConfigurationContext context)
        {
            var pipeline = context.Pipeline;
            pipeline.Register<StampCrmMessagesWithHeaderRegistration>();
        }
    }

    public class StampCrmMessagesWithHeaderRegistration : RegisterStep
    {
        public StampCrmMessagesWithHeaderRegistration() : base("StampCrmMessagesWithHeader", typeof(StampCrmMessagesWithHeaderBehavior), "Stamp incoming CRM messages with NSB header to allow processing in a handler")
        {
        }
    }

    public class StampCrmMessagesWithHeaderBehavior : Behavior<IIncomingPhysicalMessageContext>
    {
        public override Task Invoke(IIncomingPhysicalMessageContext context, Func<Task> next)
        {
            //I need to check to see if it already has a header else it modifies the response messages from endpoint 2
            if (!context.Message.Headers.ContainsKey("NServiceBus.EnclosedMessageTypes"))
            {
                context.Message.Headers[Headers.EnclosedMessageTypes] = typeof(CrmMessage).AssemblyQualifiedName;
            }
                return next();
            
        }
    }
}