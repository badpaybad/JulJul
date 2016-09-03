using System;
using System.Collections.Generic;
using JulJul.Core;
using JulJul.Core.Domain;

namespace JulJul.Services
{
    public interface IContentServices
    {

        long CreateOrEdit<TEntity, TView>(TEntity entity, long languageId, Dictionary<string, string> contentLanguages)
            where TEntity : IEntity, new() where TView : AbstractDetails<TEntity, TView>, new();

        long CreateOrEdit<TEntity, TView>(AbstractDetails<TEntity, TView> fromView)
            where TEntity : IEntity, new() where TView : AbstractDetails<TEntity, TView>, new();

        long Delete<TEntity, TView>(AbstractDetails<TEntity, TView> fromView)
            where TEntity : IEntity, new() where TView : AbstractDetails<TEntity, TView>, new();

        List<Content> GetForEntity(string entityName, long entityId, long languageId);
        List<Content> GetForEntity(Type type, long entityId, long languageId);

        List<Content> GetForEntity(IEntity entity, long languageId);

        void AddOrUpdateMultiLang(long languageId, string entityName, long entityId,
            Dictionary<string, string> contentLanguages, bool deleted = false);
    }
}