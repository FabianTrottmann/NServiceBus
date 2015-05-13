﻿namespace NServiceBus.Core.Tests.Serializers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using NServiceBus.Pipeline.Contexts;
    using NServiceBus.Serialization;
    using NServiceBus.Unicast;
    using NServiceBus.Unicast.Messages;
    using NUnit.Framework;
    using Conventions = NServiceBus.Conventions;

    public class SerializeMessagesBehaviorTests
    {
        [Test]
        public async void Should_set_content_type_header()
        {
            var registry = new MessageMetadataRegistry(true,new Conventions());

            registry.RegisterMessageType(typeof(MyMessage));

            var behavior = new SerializeMessagesBehavior(new FakeSerializer("myContentType"),registry);

            var context = new OutgoingContext(null, new SendMessageOptions("test"), new Dictionary<string, string>(), "msg id", MessageIntentEnum.Send, typeof(MyMessage), null, null);

            await behavior.Invoke(context, c => Task.FromResult(true));

            Assert.AreEqual("myContentType", context.Headers[Headers.ContentType]);
        }

        public class FakeSerializer : IMessageSerializer
        {
            public FakeSerializer(string contentType)
            {
                ContentType = contentType;
            }

            public void Serialize(object message, Stream stream)
            {
                
            }

            public object[] Deserialize(Stream stream, IList<Type> messageTypes = null)
            {
                throw new NotImplementedException();
            }

            public string ContentType { get; private set; }
        }

        class MyMessage { }
    }
}