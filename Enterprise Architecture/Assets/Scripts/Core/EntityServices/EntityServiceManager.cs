using System;
using System.Collections.Generic;

namespace RootName.Core.EntityServices
{
    internal sealed class EntityServiceManager
    {
        private readonly IDictionary<Type, IEntityService> entityServices =
            new Dictionary<Type, IEntityService>();

        public void AddEntityService<TEntityService, TBindTo>(TEntityService entityService)
            where TEntityService : TBindTo, IEntityService
        {
            if (entityService == null)
            {
                throw new NullReferenceException($"{nameof(entityService)} cannot be null");
            }
            
            var entityServiceType = typeof(TBindTo);
            entityServices[entityServiceType] = entityService;
        }
        
        public TEntityService GetEntityService<TEntityService>()
        {
            var entityServiceType = typeof(TEntityService);
            if (entityServices.TryGetValue(entityServiceType, out var entityService))
            {
                return (TEntityService)entityService;
            }

            throw new Exception($"EntityService {entityServiceType} does not exist");
        }
    }
}