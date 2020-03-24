using System.Data.Entity;
using System.Data.SQLite;
using System.IO;
using System.Windows.Controls;

namespace WpfApp1
{
    public class Company
    {

        public string CompanyName { get; private set; }
        public string ShortName { get; private set; }
        public string Manager1Name { get; private set; }
        public string Manager1Phone { get; private set; }
        public string Manager1Email { get; private set; }
        public string CompanyEmail { get; private set; }
        public string CompanyPhone { get; private set; }
        public string Address { get; private set; }
        public string CompanyUrl { get; private set; }
        public Image logo { get; private set; }
        public string Country { get; private set; }
        public string City { get; private set; }
        public string AnketaUrl { get; private set; }



        public void SetItem(string fullname, string shortname, string managername, string managerphone, string manageremail, string email, string phone, string address, string companyurl, Image image, string country, string city, string anketaurl)
        {
            CompanyName = fullname;
            ShortName = shortname;
            Manager1Name = managername;
            Manager1Phone = managerphone;
            Manager1Email = manageremail;
            CompanyEmail = email;
            CompanyPhone = phone;
            Address = address;
            CompanyUrl = companyurl;
            logo = image;
            Country = country;
            City = city;
            AnketaUrl = anketaurl;
        }

    }

    public class SqliteContext : DbContext
    {
        public SqliteContext() : base("WpfApp1.Properties.Settings.SqliteConnectionString")
        {
        }

        public DbSet<Company> Companies { get; set; }

    }

    public class Database
    {
        public SQLiteConnection dbConnection;
        public Database()
        {
            dbConnection = new SQLiteConnection(@"Data Source = Companies.sqlite3");

            if (!File.Exists(@".\Companies.sqlite3"))
            {
                SQLiteConnection.CreateFile(@".\Companies.sqlite3");
                SQLiteCommand command = new SQLiteCommand("CREATE TABLE [Companies] ([Id] INTEGER NOT NULL" +
                    ", [CompanyName] text NOT NULL" +
                    ", [ShortName] text NULL" +
                    ", [CompanyEmail] text NULL" +
                    ", [logo] image NULL" +
                    ", [URL] text NULL" +
                    ", [Country] text NULL" +
                    ", [City] text NULL" +
                    ", [Address] text NULL" +
                    ", [Manager1Name] text NULL" +
                    ", [Manager1Phone] text NULL" +
                    ", [Manager1Email] text NULL" +
                    ", [CompanyPhone] text NULL" +
                    ", [AnketaUrl] text NULL" +
                    ", CONSTRAINT[sqlite_autoindex_Companies_1] PRIMARY KEY([Id]));", dbConnection);
                OpenConnection();
                command.ExecuteNonQuery();
                CloseConnection();
            }
        }
        public void OpenConnection()
        {
            if (dbConnection.State != System.Data.ConnectionState.Open)
            {
                dbConnection.Open();
            }
        }
        public void CloseConnection()
        {
            if (dbConnection.State != System.Data.ConnectionState.Closed)
            {
                dbConnection.Close();
            }
        }
    }
}
