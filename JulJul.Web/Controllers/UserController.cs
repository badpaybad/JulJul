using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using JulJul.Core.Distributed;
using JulJul.Core.Domain;
using JulJul.Services;
using JulJul.Web.Models;

namespace JulJul.Web.Controllers
{
    public class UserController : Controller
    {
        Guid LanguageId=Guid.Parse("{63877600-820F-42B8-A573-7250AEEF0617}");
        private UserServices _userServices;
        public UserController()
        {
            _userServices = ServicesEngine.Resolve<UserServices>();
        }
        // GET: User
        public ActionResult Index()
        {
            var model=new UserViewModel();
            model.User.LanguageId = LanguageId;
            model.List = _userServices.GetAll(LanguageId).ToList();
            
            return View(model);
        }

     

        [HttpPost]
        public ActionResult Index(UserViewModel model
            ,string btnAdd,string btnSave,string btnDelete)
        {
            var cmd = new DistributedEntityDetailsCommand<User,UserDetails>(model.User,DistributedDbCommandType.Add);

            ServicesEngine.DistributedServices.EntityDetailsPublish(cmd);

            model.List = _userServices.GetAll(LanguageId).ToList();
            return View(model);
        }
    }
}