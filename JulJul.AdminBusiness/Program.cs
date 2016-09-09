using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JulJul.AdminBusiness.Admin;
using JulJul.Repository;
using JulJul.Services;

namespace JulJul.AdminBusiness
{
    class Program
    {
        static void Main(string[] args)
        {
            RepositoryEngine.Boot();

            AdminServicesEngine.Boot();

            Console.ReadLine();
        }
    }
}