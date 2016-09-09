using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JulJul.Core;
using JulJul.Core.Distributed;
using JulJul.Core.Domain;
using JulJul.Core.Expressions;
using JulJul.Repository;

namespace JulJul.Services
{
    public class ContentServices
    {
        private IContentRepository _repository;

        public ContentServices(IContentRepository repository)
        {
            _repository = repository;
        }
        
        public List<Content> GetForEntity(string entityName, Guid entityId, Guid languageId)
        {
            if (string.IsNullOrEmpty(entityName)) throw new ArgumentNullException(nameof(entityName));

            return _repository.SelectBy(new ExpressionWhere<Content>(i => i.EntityId == entityId && i.LanguageId == languageId
                                           && i.Entity.Equals(entityName, StringComparison.OrdinalIgnoreCase))).ToList();
        }

        public List<Content> GetForEntity(Type type, Guid entityId, Guid languageId)
        {
            var entityName = type.Name;
            return GetForEntity(entityName, entityId, languageId);
        }

        public List<Content> GetForEntity(IEntity entity, Guid languageId)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var entityName = entity.GetEntityName();
            return _repository.SelectBy(new ExpressionWhere<Content>(i => i.EntityId == entity.Id && i.LanguageId == languageId
                                         && i.Entity.Equals(entityName, StringComparison.OrdinalIgnoreCase))).ToList();
        }
   }
}
