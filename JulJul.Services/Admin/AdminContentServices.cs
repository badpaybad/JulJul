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

namespace JulJul.Services.Admin
{
    internal class AdminContentServices: IAdminServciesSubscribeChange
    {
        private IContentRepository _repository;
        public AdminContentServices(IContentRepository repository)
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

        public long CreateOrEdit<TEntity, TView>(AbstractDetails<TEntity, TView> fromView)
            where TEntity : IEntity, new() where TView : AbstractDetails<TEntity, TView>, new()
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


        public void AddOrUpdateMultiLang(Guid languageId, string entityName, Guid entityId,
            Dictionary<string, string> contentLanguages, bool deleted = false)
        {
            if (entityId == Guid.Empty || languageId == Guid.Empty)
                throw new Exception("Id and LanguageId not assigned. Must be entityId>0 and languageId>0");

            var inDb =
                _repository.SelectBy(new ExpressionWhere<Content>(
                    i => i.LanguageId.Equals(languageId) && i.EntityId == entityId && i.Entity.Equals(entityName))).ToArray();

            foreach (var ml in inDb)
            {
                if (deleted)
                {
                    DistributedServices.Instance.DbPublish(new DistributedDbCommand<Content>(ml, DistributedDbCommandType.Delete));
                    //_repository.TryDelete(ml);
                }
                else
                {
                    string val;
                    if (!contentLanguages.TryGetValue(ml.Field, out val)) continue;

                    ml.Value = val;
                    DistributedServices.Instance.DbPublish(new DistributedDbCommand<Content>(ml, DistributedDbCommandType.Update));
                    //_repository.TryUpdate(ml);
                }
            }


            foreach (var data in contentLanguages)
            {
                if (inDb.Any(i => i.Field.Equals(data.Key, StringComparison.OrdinalIgnoreCase))) continue;

                var content = new Content()
                {
                    EntityId = entityId,
                    Entity = entityName,
                    LanguageId = languageId,
                    Field = data.Key,
                    Value = data.Value
                };
                DistributedServices.Instance.DbPublish(new DistributedDbCommand<Content>(content, DistributedDbCommandType.Add));
                //_repository.TryInsert(content);
            }
        }

        public void RegisterSubscribeChange(DistributedServices distributedServices)
        {
            distributedServices.EntityDetailsSubcribe<Content,ContentDetails>((s, cmd) =>
            {
                switch (cmd.CommandType)
                {
                        case DistributedDbCommandType.Add:
                        CreateOrEdit(cmd.Data);
                        break;
                    case DistributedDbCommandType.Update:
                        CreateOrEdit(cmd.Data);
                        break;
                    case DistributedDbCommandType.Delete:
                        
                        break;
                }
            });
        }
    }
}
