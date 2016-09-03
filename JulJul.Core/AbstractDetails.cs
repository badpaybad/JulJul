using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JulJul.Core.Domain;

namespace JulJul.Core
{
    public abstract class AbstractDetails<T,TView> where T :IEntity,new() where TView:AbstractDetails<T,TView>
    {
        public virtual long Id { get; set; }
        public long LanguageId { get; set; }

        public T ConvertToEntity()
        {
            var entity = new T();

            var props = entity.GetType().GetProperties();

            var details = this.GetType().GetProperties();

            foreach (var dn in details)
            {
                var value = dn.GetValue(this);

                foreach (var pi in props)
                {
                    if (!pi.Name.Equals(dn.Name, StringComparison.OrdinalIgnoreCase)) continue;

                    var propertyType = pi.PropertyType;
                    if (propertyType.IsGenericType &&
                        propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        propertyType = propertyType.GetGenericArguments()[0];
                    }


                    pi.SetValue(entity, Convert.ChangeType(value, propertyType), null);
                    break;
                }
            }

            return entity;
        }

        public Dictionary<string, string> GetContentFields()
        {
            var contentFields = new Dictionary<string, string>();

            var props = typeof(T).GetProperties();

            var details = this.GetType().GetProperties();

            foreach (var dn in details)
            {
                var isInside = false;
                var value = dn.GetValue(this);

                foreach (var pi in props)
                {
                    if (!pi.Name.Equals(dn.Name, StringComparison.OrdinalIgnoreCase)) continue;

                    isInside = true;

                    break;
                }

                if (isInside) continue;

                if (!dn.PropertyType.Name.Equals("string", StringComparison.OrdinalIgnoreCase)) continue;

                contentFields[dn.Name] = value == null ? string.Empty : value.ToString();
            }

            return contentFields;
        }

        public T ConvertToEntity(out Dictionary<string, string> contentFields)
        {
            contentFields = new Dictionary<string, string>();
            var entity = new T();

            var props = entity.GetType().GetProperties();

            var details = this.GetType().GetProperties();

            foreach (var dn in details)
            {
                var isInside = false;
                var value = dn.GetValue(this);

                foreach (var pi in props)
                {
                    if (!pi.Name.Equals(dn.Name, StringComparison.OrdinalIgnoreCase)) continue;

                    var propertyType = pi.PropertyType;
                    if (propertyType.IsGenericType &&
                        propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        propertyType = propertyType.GetGenericArguments()[0];
                    }

                    isInside = true;
                    pi.SetValue(entity, Convert.ChangeType(value, propertyType), null);
                    break;
                }

                if (isInside) continue;

                if (!dn.PropertyType.Name.Equals("string", StringComparison.OrdinalIgnoreCase)) continue;

                contentFields[dn.Name] = value == null ? string.Empty : value.ToString();
            }

            return entity;
        }

        public string EntityName()
        {
            return typeof(T).Name;
        }

        public TView BindContent(List<Content> langs) 
        {
            if (langs == null || langs.Count == 0) return (TView)this;

            var details = this.GetType().GetProperties();

            var temp = langs.ToDictionary(k => k.Field, v => v.Value);

            foreach (var dn in details)
            {
                if (!dn.PropertyType.Name.Equals("string", StringComparison.OrdinalIgnoreCase)) continue;

                string val = string.Empty;
                if (temp.TryGetValue(dn.Name, out val))
                {
                    dn.SetValue(this, Convert.ChangeType(val, dn.PropertyType), null);
                }
            }

            return (TView)this;
        }

        public TView BindEntityAndContent(IEntity entity, List<Content> langs, long languageId)
        {
            langs = langs ?? new List<Content>();

            var props = entity.GetType().GetProperties();

            var details = this.GetType().GetProperties();

            this.LanguageId = languageId;

            var temp = langs.ToDictionary(k => k.Field, v => v.Value);
            foreach (var dp in details)
            {
                var isInside = false;
                foreach (var ep in props)
                {
                    var value = ep.GetValue(entity);

                    var propertyType = dp.PropertyType;
                    if (propertyType.IsGenericType &&
                        propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        propertyType = propertyType.GetGenericArguments()[0];
                    }

                    if (!ep.Name.Equals(dp.Name, StringComparison.OrdinalIgnoreCase)) continue;

                    dp.SetValue(this, Convert.ChangeType(value, propertyType), null);

                    isInside = true;

                    break;
                }

                if (isInside) continue;
                if (!dp.PropertyType.Name.Equals("string", StringComparison.OrdinalIgnoreCase)) continue;

                string val = string.Empty;
                if (temp.TryGetValue(dp.Name, out val))
                {
                    dp.SetValue(this, Convert.ChangeType(val, dp.PropertyType), null);
                }
            }

            var obj = (TView)this;
            return obj;
        }

    }
}