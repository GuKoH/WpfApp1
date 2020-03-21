using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Data;
using System.Data.SqlClient;

namespace WpfApp1
{
    public class CompanyItem
    {

        public string FullName { get; private set; }
        public string ShortName { get; private set; }
        public string ManagerName { get; private set; }
        public string ManagerPhone { get; private set; }
        public string ManagerEmail { get; private set; }
        public string Email { get; private set; }
        public string Phone { get; private set; }
        public string Address { get; private set; }
        public string CompanyWeb { get; private set; }
        public Image Logo { get; private set; }
        public string Country { get; private set; }
        public string City { get; private set; }
        public string AnketaUrl { get; private set; }



        public void SetItem(string fullname, string shortname, string managername, string managerphone, string manageremail, string email, string phone, string address, string companyweb, Image image, string country, string city, string anketaurl)
        {
            FullName = fullname;
            ShortName = shortname;
            ManagerName = managername;
            ManagerPhone = managerphone;
            ManagerEmail = manageremail;
            Email = email;
            Phone = phone;
            Address = address;
            CompanyWeb = companyweb;
            Logo = image;
            Country = country;
            City = city;
            AnketaUrl = anketaurl;

        }
    }
}
