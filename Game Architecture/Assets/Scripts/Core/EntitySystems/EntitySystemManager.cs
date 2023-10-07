using System;
using System.Collections.Generic;

namespace RootName.Core.EntitySystems
{
    internal sealed class EntitySystemManager
    {
        private readonly IDictionary<Type, IEntitySystem> entitySystems =
            new Dictionary<Type, IEntitySystem>();

        public void AddEntitySystem<TEntitySystem, TBindTo>(TEntitySystem entitySystem)
            where TEntitySystem : TBindTo, IEntitySystem
        {
            if (entitySystem == null)
            {
                throw new NullReferenceException($"{nameof(entitySystem)} cannot be null");
            }
            
            var entitySystemType = typeof(TBindTo);
            entitySystems[entitySystemType] = entitySystem;
        }
        
        public TEntitySystem GetEntitySystem<TEntitySystem>()
        {
            var entitySystemType = typeof(TEntitySystem);
            if (entitySystems.TryGetValue(entitySystemType, out var entitySystem))
            {
                return (TEntitySystem)entitySystem;
            }

            throw new Exception($"EntitySystem {entitySystemType} does not exist");
        }
    }
}