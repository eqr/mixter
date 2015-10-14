﻿using System.Collections.Generic;
using Mixter.Domain.Core.Messages.Events;
using Mixter.Domain.Identity;

namespace Mixter.Domain.Core.Messages
{
    public class Message
    {
        private readonly DecisionProjection _projection = new DecisionProjection();

        public Message(IEnumerable<IDomainEvent> events)
        {
            foreach (var @event in events)
            {
                _projection.Apply(@event);
            }
        }

        public static MessageId Quack(IEventPublisher eventPublisher, UserId author, string content)
        {
            var messageId = MessageId.Generate();
            eventPublisher.Publish(new MessageQuacked(messageId, author, content));
            return messageId;
        }

        public void Requack(IEventPublisher eventPublisher, UserId requacker)
        {
            if (_projection.Author.Equals(requacker))
            {
                return;
            }

            var evt = new MessageRequacked(_projection.Id, requacker);
            eventPublisher.Publish(evt);
        }

        private class DecisionProjection : DecisionProjectionBase
        {
            public MessageId Id { get; private set; }

            public UserId Author { get; private set; }

            public DecisionProjection()
            {
                AddHandler<MessageQuacked>(When);
            }
            
            private void When(MessageQuacked evt)
            {
                Id = evt.Id;
                Author = evt.Author;
            }
        }
    }
}
