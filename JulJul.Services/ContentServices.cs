using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JulJul.Core;
using JulJul.Core.Domain;
using JulJul.Core.Expressions;
using JulJul.Repository;

namespace JulJul.Services
{
    public class ContentServices:IContentServices
    {
        private IContentRepository _repository;

        public ContentServices(IContentRepository repository)
        {
            _repository = repository;
        }


        public long CreateOrEdit<TEntity, TView>(TEntity entity, Guid languageId, Dictionary<string, string> contentLanguages)
            where TEntity : IEntity, new() where TView : AbstractDetails<TEntity, TView>, new()
        {
            if (entity.Id == Guid.Empty) throw new Exception("Id not assigned. Must be Id>0");
            if (languageId == Guid.Empty) throw new Exception("LanguageId not assigned. Must be LanguageId>0");

            var entityName = entity.GetEntityName();

            AddOrUpdateMultiLang(languageId, entityName, entity.Id, contentLanguages);

            return contentLanguages.Count;
        }

        public long CreateOrEdit<TEntity,TView>(AbstractDetails<TEntity,TView> fromView) 
            where TEntity : IEntity, new() where TView: AbstractDetails<TEntity,TView>,new()
        {
            Dictionary<string, string> contentLanguages;
            var entity = fromView.ConvertToEntity(out contentLanguages);
            var entityName = entity.GetEntityName();

            if (fromView.Id == Guid.Empty || entity.Id == Guid.Empty) throw new Exception("Id not assigned. Must be Id>0");
            if (fromView.LanguageId == Guid.Empty) throw new Exception("LanguageId not assigned. Must be LanguageId>0");
            
            AddOrUpdateMultiLang(fromView.LanguageId, entityName, entity.Id, contentLanguages);

            return contentLanguages.Count;
        }


        public long Delete<TEntity, TView>(AbstractDetails<TEntity, TView> fromView)
                where TEntity : IEntity, new() where TView : AbstractDetails<TEntity, TView>, new()
        {
            Dictionary<string, string> contentLanguages;
            var entity = fromView.ConvertToEntity(out contentLanguages);
            var entityName = entity.GetEntityName();

            if (fromView.Id == Guid.Empty || entity.Id == Guid.Empty) throw new Exception("Id not assigned. Must be Id>0");
            if (fromView.LanguageId == Guid.Empty) throw new Exception("LanguageId not assigned. Must be LanguageId>0");

            AddOrUpdateMultiLang(fromView.LanguageId, entityName, entity.Id, contentLanguages, true);

            return contentLanguages.Count;
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

        public void AddOrUpdateMultiLang(Guid languageId, string entityName, Guid entityId,
            Dictionary<string, string> contentLanguages, bool deleted = false)
        {
            if (entityId == Guid.Empty || languageId ==Guid.Empty)
                throw new Exception("Id and LanguageId not assigned. Must be entityId>0 and languageId>0");

            var inDb =
                _repository.SelectBy(new ExpressionWhere<Content>(
                    i => i.LanguageId.Equals(languageId) && i.EntityId == entityId && i.Entity.Equals(entityName))).ToArray();

            foreach (var ml in inDb)
            {
                if (deleted)
                {
                    _repository.TryDelete(ml);
                }
                else
                {
                    string val;
                    if (!contentLanguages.TryGetValue(ml.Field, out val)) continue;

                    ml.Value = val;
                    _repository.TryUpdate(ml);
                }
            }


            foreach (var data in contentLanguages)
            {
                if (inDb.Any(i => i.Field.Equals(data.Key, StringComparison.OrdinalIgnoreCase))) continue;

                _repository.TryInsert(new Content()
                {
                    EntityId = entityId,
                    Entity = entityName,
                    LanguageId = languageId,
                    Field = data.Key,
                    Value = data.Value
                });
            }
        }
    }
}
