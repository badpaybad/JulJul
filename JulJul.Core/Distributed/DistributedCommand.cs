﻿using System;
using System.Data;
using Newtonsoft.Json;

namespace JulJul.Core.Distributed
{
    public class DistributedCommand<T> where T:class 
    {
        public CommandBehavior CommandBehavior { get; set; }
        public T Data { get; set; }
        public Type DataType
        {
            get
            {
                if (Data == null) return typeof(Nullable<>);
                return Data.GetType();
            }
        }

        public DistributedCommand(T data, CommandBehavior cmdBehavior= CommandBehavior.Queue)
        {
            Data = data;
            CommandBehavior = cmdBehavior;
        }

        public string ToJson()
        {
            if (Data == null) return string.Empty;
            return JsonConvert.SerializeObject(this);
        }

        public T GetData()
        {
            if (Data == null) return default(T);

            return (T)Data;
        }

        public string GetChannelKey()
        {
            if (Data == null) throw new NoNullAllowedException();

            return DataType.FullName;
        }
    }
}