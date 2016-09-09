using System;
using System.Collections.Generic;
using JulJul.Core;
using JulJul.Core.Domain;

namespace JulJul.Services
{
    public interface IContentServices
    {

        long CreateOrEdit<TEntity, TView>(TEntity entity, Guid languageId, Dictionary<string, string> contentLanguages)
            where TEntity : IEntity, new() where TView : AbstractDetails<TEntity, TView>, new();

        long CreateOrEdit<TEntity, TView>(AbstractDetails<TEntity, TView> fromView)
            where TEntity : IEntity, new() where TView : AbstractDetails<TEntity, TView>, new();

        long Delete<TEntity, TView>(AbstractDetails<TEntity, TView> fromView)
            where TEntity : IEntity, new() where TView : AbstractDetails<TEntity, TView>, new();

        List<Content> GetForEntity(string entityName, Guid entityId, Guid languageId);
        List<Content> GetForEntity(Type type, Guid entityId, Guid languageId);

        List<Content> GetForEntity(IEntity entity, Guid languageId);

        void AddOrUpdateMultiLang(Guid languageId, string entityName, Guid entityId,
            Dictionary<string, string> contentLanguages, bool deleted = false);
    }
}