﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using JulJul.Core;
using JulJul.Core.Domain;
using JulJul.Core.Expressions;
using JulJul.Core.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace JulJul.UnitTest
{
   

    [TestClass]
    public class AbstractRepositoryTest
    {
        [TestMethod]
        public void Add()
        {
            var repo=new UserRepository();
            repo.TryInsert(new User() { Username = "badpaybad", Password = "123456"});
        }
        [TestMethod]
        public void SelectWithOrder()
        {
            var repo=new UserRepository();

            long total;
          var xxx=  repo.SelectBy(new ExpressionWhere<User>(i=>i.Id>0), 
           new ExpressionOrderBy<User,string>(i=>i.Username,true) , 0,10,out total);

            Console.WriteLine(JsonConvert.SerializeObject(xxx));
        }

        [TestMethod]
        public void SelectViewWithOrder()
        {
            var repo = new UserRepository();

            long total;
            var xxx = repo.SelectViewBy(new ExpressionSelection<User, Account>(i=>new Account() {})
                , new ExpressionOrderBy<User, long>(i => i.Id, true), new ExpressionWhere<User>(i=>i.Id>0),0,1,out total );

            Console.WriteLine(JsonConvert.SerializeObject(xxx));
            Console.WriteLine(JsonConvert.SerializeObject(total));
        }
        [TestMethod]
        public void PagingViewOrder()
        {
            var repo = new UserRepository();

            long total;
            var xxx = repo.Paging(new ExpressionViewPaging<User, Account,  string>(
                take:1, order:i=>i.Password, where:i=>i.Id>0,
                isDesc:true, skip:1, selection:i=>new Account()
                {
                    MatKhau=i.Password,
                    TenDangNhap=i.Username
                }
                ), out total);

            Console.WriteLine(JsonConvert.SerializeObject(xxx));
            Console.WriteLine(JsonConvert.SerializeObject(total));
        }
    }

    public class Account
    {
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
    }
}
