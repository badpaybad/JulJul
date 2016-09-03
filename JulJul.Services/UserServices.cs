using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JulJul.Core.Domain;
using JulJul.Repository;

namespace JulJul.Services
{
  public  class UserServices
  {
      private IUserRepository _repository;
      private IContentServices _contentServices;

      public  UserServices(IUserRepository repository, IContentServices contentServices)
      {
          _repository = repository;
          _contentServices = contentServices;
      }

      public void Create(UserDetails userDetails)
      {
          Dictionary<string, string> contentFields;
          var entity= userDetails.ConvertToEntity(out contentFields);
          _repository.TryInsert(entity);
          _contentServices.CreateOrEdit(userDetails);
      }

      public IEnumerable<UserDetails> GetAll(long languageId)
      {
          var temp = _repository.SelectAll()
              .Select(i => new UserDetails().BindEntityAndContent(i, _contentServices.GetForEntity(i, languageId),languageId));

          return temp;
      } 
  }
}
