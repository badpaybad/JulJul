using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JulJul.Core.Distributed;
using JulJul.Core.Domain;
using JulJul.Repository;

namespace JulJul.Services.Admin
{
    internal class AdminUserServices:AbstractAdminServcies<User,UserDetails>
    {
        private IUserRepository _repository;
        private AdminContentServices _contentServices;

        public AdminUserServices(IUserRepository repository, AdminContentServices contentServices)
        {
            _repository = repository;
            _contentServices = contentServices;
        }

        public override void Create(UserDetails userDetails)
        {
            Dictionary<string, string> contentFields;
            var entity = userDetails.ConvertToEntity(out contentFields);
            //_repository.TryInsert(entity);
            DistributedServices.Instance.DbPublish(new DistributedDbCommand<User>(entity, DistributedDbCommandType.Add));
            _contentServices.CreateOrEdit(userDetails);
        }

       public override void Update(UserDetails data)
       {
           throw new NotImplementedException();
       }

       public override void Delete(UserDetails data)
       {
           throw new NotImplementedException();
       }

       public void RegisterSubscribeChange(DistributedServices distributedServices)
       {
           
       }
    }
}
