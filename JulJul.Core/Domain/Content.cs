﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JulJul.Core.Domain
{
    [Table("Content")]
    public class Content : AbstractEntity
    {
        public Guid EntityId { get; set; }
        public Guid LanguageId { get; set; }
        public string Entity { get; set; }
        public string Field { get; set; }
        public string Value { get; set; }
    }
}