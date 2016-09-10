using System;
using System.Data;
using Newtonsoft.Json;

namespace JulJul.Core.Distributed
{
    public class DistributedEntityDetailsCommand<T,TView> where T : IEntity, new() where TView : AbstractDetails<T, TView>
    {
        public CommandBehavior CommandBehavior { get; set; }
        public DistributedDbCommandType DbCommandType { get; set; }
        public TView Data { get; set; }

        public Type DataType
        {
            get
            {
                if (Data == null) return typeof(Nullable<>);
                return Data.GetType();
            }
        }

        public DistributedEntityDetailsCommand(TView data, DistributedDbCommandType cmdType, 
            CommandBehavior cmdBehavior = CommandBehavior.Queue)
        {
            Data = data;
            DbCommandType = cmdType;
            CommandBehavior = cmdBehavior;
        }

        public string ToJson()
        {
            if (Data == null) return string.Empty;
            return JsonConvert.SerializeObject(this);
        }

        public TView GetData()
        {
            if (Data == null) return default(TView);

            return (TView)Data;
        }

        public string GetChannelKey()
        {
            if (Data == null) throw new NoNullAllowedException();

            return DataType.FullName;
        }
    }
}