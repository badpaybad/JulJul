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
            ,string btnAdd,string btnSave,string btnDelete,string btnAddRandom)
        {
            if (!string.IsNullOrEmpty(btnAdd))
            {
                model.User.Id = Guid.NewGuid();
                var cmd = new DistributedEntityDetailsCommand<User, UserDetails>(
                    model.User, DistributedDbCommandType.Add, CommandBehavior.Queue);
                ServicesEngine.DistributedServices.EntityDetailsPublish(cmd);
            }
            else if (!string.IsNullOrEmpty(btnSave))
            {
                var cmd = new DistributedEntityDetailsCommand<User, UserDetails>(
                    model.User, DistributedDbCommandType.Update);
                ServicesEngine.DistributedServices.EntityDetailsPublish(cmd);
            }
            else if (!string.IsNullOrEmpty(btnDelete))
            {
                var cmd = new DistributedEntityDetailsCommand<User, UserDetails>(
                    model.User, DistributedDbCommandType.Delete);
                ServicesEngine.DistributedServices.EntityDetailsPublish(cmd);
            }else if (!string.IsNullOrEmpty(btnAddRandom))
            {
                var rnd=new Random();
                model.User.Id = Guid.NewGuid();
                var next = rnd.Next();
                model.User.Fullname = "Fullname_" + next;
                model.User.Username = "Username_" + next;
                model.User.Password = "Username_" + next;
                var cmd = new DistributedEntityDetailsCommand<User, UserDetails>(
                    model.User, DistributedDbCommandType.Add, CommandBehavior.Queue);
                ServicesEngine.DistributedServices.EntityDetailsPublish(cmd);
            }
            model.List = _userServices.GetAll(LanguageId).ToList();
            return View(model);
        }
    }
}