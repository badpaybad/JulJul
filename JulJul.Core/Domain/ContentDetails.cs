using System;

namespace JulJul.Core.Domain
{
    public class ContentDetails : AbstractDetails<Content, ContentDetails>
    {
        public virtual Guid Id { get; set; } = Guid.NewGuid();
        public Guid EntityId { get; set; }
        public Guid LanguageId { get; set; }
        public string Entity { get; set; }
        public string Field { get; set; }
        public string Value { get; set; }
    }
}