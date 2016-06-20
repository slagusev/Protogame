﻿using System;
using System.Linq;
using Protoinject;

namespace Protogame
{
    public class WorldNetworkEventBinder : IEventBinder<INetworkEventContext>
    {
        private IKernel _kernel;
        private IHierarchy _hierarchy;
        private INetworkMessageSerialization _networkMessageSerialization;

        public int Priority => 150;

        public void Assign(IKernel kernel)
        {
            _kernel = kernel;
            _hierarchy = kernel.Hierarchy;
            _networkMessageSerialization = kernel.Get<INetworkMessageSerialization>();
        }

        public bool Handle(INetworkEventContext context, IEventEngine<INetworkEventContext> eventEngine, Event @event)
        {
            var networkReceiveEvent = @event as NetworkMessageReceivedEvent;
            if (networkReceiveEvent == null)
            {
                return false;
            }

            var @object = _networkMessageSerialization.Deserialize(networkReceiveEvent.Payload);

            if (networkReceiveEvent.GameContext != null)
            {
                // Messages which are only allowed to be handled by the client.

                var createEntityMessage = @object as EntityCreateMessage;
                if (createEntityMessage != null)
                {
                    // Spawn an entity in the world...
                    var world = networkReceiveEvent.GameContext.World;
                    var spawnedEntity = _kernel.Get(
                        Type.GetType(createEntityMessage.EntityType),
                        _hierarchy.Lookup(world)) as IEntity;

                    if (spawnedEntity != null)
                    {
                        spawnedEntity.Transform = createEntityMessage.InitialTransform.DeserializeFromNetwork();
                    }

                    var networkIdentifiableEntity = spawnedEntity as INetworkIdentifiable;
                    if (networkIdentifiableEntity != null)
                    {
                        networkIdentifiableEntity.ReceiveNetworkIDFromServer(
                            networkReceiveEvent.GameContext,
                            networkReceiveEvent.UpdateContext,
                            createEntityMessage.EntityID);
                    }

                    return true;
                }
            }

            return false;
        }
    }
}
