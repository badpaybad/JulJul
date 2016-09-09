using System;
using System.Collections.Generic;
using JulJul.Core.Distributed;
using JulJul.Core.Domain;
using JulJul.Repository;

namespace JulJul.AdminBusiness.Admin
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
            DistriubtedServices.DbPublish(new DistributedDbCommand<User>(entity, DistributedDbCommandType.Add));
            _contentServices.CreateOrEdit(userDetails);
        }

       public override void Update(UserDetails userDetails)
       {
            Dictionary<string, string> contentFields;
            var entity = userDetails.ConvertToEntity(out contentFields);
            //_repository.TryInsert(entity);
            DistriubtedServices.DbPublish(new DistributedDbCommand<User>(entity, DistributedDbCommandType.Update));
            _contentServices.CreateOrEdit(userDetails);
        }

       public override void Delete(UserDetails userDetails)
       {
            Dictionary<string, string> contentFields;
            var entity = userDetails.ConvertToEntity(out contentFields);
            //_repository.TryInsert(entity);
            DistriubtedServices.DbPublish(new DistributedDbCommand<User>(entity, DistributedDbCommandType.Delete));
            _contentServices.CreateOrEdit(userDetails);
        }
        
    }
}
