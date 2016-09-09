using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JulJul.Core.Distributed;
using JulJul.Core.Domain;
using JulJul.Repository;

namespace JulJul.Services
{
  public  class UserServices
  {
      private IUserRepository _repository;
      private ContentServices _contentServices;

      public  UserServices(IUserRepository repository, ContentServices contentServices)
      {
          _repository = repository;
          _contentServices = contentServices;
      }


      public IEnumerable<UserDetails> GetAll(Guid languageId)
      {
          var temp = _repository.SelectAll()
              .Select(i => new UserDetails().BindEntityAndContent(i, _contentServices.GetForEntity(i, languageId),languageId));

          return temp;
      } 
  }
}
