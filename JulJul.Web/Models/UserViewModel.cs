using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JulJul.Core.Domain;

namespace JulJul.Web.Models
{
    public class UserViewModel
    {
        public UserDetails User { get; set; }

        public List<UserDetails> List { get; set; }

        public UserViewModel()
        {
            User=new UserDetails();
            List=new List<UserDetails>();
        }
    }
}