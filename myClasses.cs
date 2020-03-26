using System.Data.Entity;
using System.Data.SQLite;
using System.IO;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public string Mark { get; private set; }

        public void SetMark(int mark) { Mark = mark.ToString();}

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

    public class Database
    {
        public SQLiteConnection dbConnection;
        public Database()
        {
            dbConnection = new SQLiteConnection(@"Data Source = Companies.sqlite3");
            //Если базы данных не существует
            if (!File.Exists(@".\Companies.sqlite3"))
            {
                //создать базу данных
                SQLiteConnection.CreateFile(@".\Companies.sqlite3");
                //создать таблицу Companies
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
                

                //Cоздать таблицу Comments
                command = new SQLiteCommand("CREATE TABLE [Comments] ([Id] INTEGER NOT NULL, [CompanyName] TEXT NOT NULL, [Author] TEXT NULL, [Comment] TEXT NOT NULL, [Uid] INTEGER NOT NULL, [Time] TEXT NOT NULL, CONSTRAINT[PK_Comments] PRIMARY KEY([Id])); ", dbConnection);
                
                command.ExecuteNonQuery();
              

                //Создать таблицу Marks
                command = new SQLiteCommand("CREATE TABLE [Marks] ([Id] INTEGER NOT NULL, [CompanyName] TEXT NOT NULL, [Mark] INTEGER NOT NULL, [uid] INTEGER NOT NULL, CONSTRAINT [PK_Marks] PRIMARY KEY ([Id])); ", dbConnection);
                
                command.ExecuteNonQuery();
                

                //Создать таблицу Settings
                command = new SQLiteCommand("CREATE TABLE [Settings] ([Id] INTEGER NOT NULL, [Param] TEXT NOT NULL, [Value] INTEGER NULL, [TextValue] TEXT NULL, CONSTRAINT[PK_Settings] PRIMARY KEY([Id])); ", dbConnection);
                
                command.ExecuteNonQuery();
                

                //Создать Uid
                command = new SQLiteCommand("INSERT INTO Settings (Param,Value) Values ('Uid',@value)", dbConnection);
                System.Random uid = new System.Random();
                string i = uid.Next(10000000, 99999999).ToString();
                command.Parameters.AddWithValue("value", i);
                command.ExecuteNonQuery();

                CloseConnection();
            }
        }
        //Открытие соединения с локальной базой данных
        public void OpenConnection()
        {
            if (dbConnection.State != System.Data.ConnectionState.Open)
            {
                dbConnection.Open();
            }
        }

        //Закрытие соединения
        public void CloseConnection()
        {
            if (dbConnection.State != System.Data.ConnectionState.Closed)
            {
                dbConnection.Close();
            }
        }
    }

    public class Comment
    {

        public string CompanyName { get; private set; }
        public string Author { get; private set; }
        public string CommentText { get; private set; }
        public int Uid { get; private set; }
        public string Time { get; private set; }

        public Comment(string company, string author, string comment, int uid, string time)
        {
            CompanyName = company;
            Author = author;
            CommentText = comment;
            Uid = uid;
            Time = time;
        }

        public void AddComment(SqlConnection sqlConnection)
        {

            SqlCommand command = new SqlCommand($"INSERT INTO Comments (CompanyName,Author,Comment,Uid,Time) VALUES ('{CompanyName}','{Author}','{CommentText}','{Uid}','{Time}')", sqlConnection);
            command.ExecuteNonQuery();
        }

        public async Task<List<Comment>> GetCommentsAsync(string company, SqlConnection sqlConnection)
        {
            return await Task.Run(() => GetComments(company, sqlConnection));


        }

        public List<Comment> GetComments(string company, SqlConnection sqlConnection)
        {

            List<Comment> CommentsList = new List<Comment>();
            SqlCommand command = new SqlCommand($"SELECT * FROM Comments WHERE CompanyName = '{company}'", sqlConnection);
            SqlDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                Comment NewComment = new Comment(dataReader["CompanyName"].ToString(), dataReader["Author"].ToString(), dataReader["Comment"].ToString(), int.Parse(dataReader["Uid"].ToString()), dataReader["Time"].ToString());
                CommentsList.Add(NewComment);
            }
            dataReader.Close();

            return CommentsList;
        }
    }

    public class myMailItem
    {
        public string mailSubject { get; private set; }
        public string mailBody { get; private set; }
        public string mailDate { get; private set; }
        public string mailFrom { get; private set; }

        public void SetMail(string From, string Subject, string Body, string Date)
        {
            mailSubject = Subject;
            mailBody = Body;
            mailDate = Date;
            mailFrom = From;
        }
    }

    public class Marks
    {
        public string CompanyName { get; private set; }
        public int Mark { get; private set; }
        public int Uid { get; private set; }
        public SqlConnection sqlConnection { get; private set; }
        public Marks(string company,int mark,int uid, SqlConnection sqlConnectionString)
        {
            CompanyName = company;
            Mark = mark;
            Uid = uid;
            sqlConnection = sqlConnectionString;
        }

        public void AddMark()
        {
            SqlCommand commanda = new SqlCommand($"SELECT CompanyName,uid FROM Marks WHERE CompanyName='{CompanyName}' AND uid='{Uid.ToString()}' ", sqlConnection);
            SqlDataReader sqlDataReader = commanda.ExecuteReader();
            int i = 0;
            while (sqlDataReader.Read()) i++;
            if (i == 0)
            {
                SqlCommand command = new SqlCommand($"INSERT CompanyName,Mark,Uid INTO Marks VALUES ('{CompanyName}','{Mark}','{Uid.ToString()}')", sqlConnection);
                command.ExecuteReader();
            } 
            else if (i == 1)
            {
                SqlCommand command = new SqlCommand($"UPDATE Marks SET Mark = '{Mark}' WHERE Uid = '{Uid.ToString()}')", sqlConnection);
                command.ExecuteReader();
            }
        }

        public int RecieveMarks(string company)
        {
            List<Marks> marks = new List<Marks>();
            
            int sum=0;
            int i = 0;
            SqlCommand command = new SqlCommand($"SELECT Mark FROM Marks WHERE CompanyName='{CompanyName}'", sqlConnection);
            SqlDataReader sqlDataReader = command.ExecuteReader();
            
            while (sqlDataReader.Read())
            {
                sum += int.Parse(sqlDataReader["Mark"].ToString()); i++;
            }
            double result = sum / i;
            result = System.Math.Round(result);
            
            return (int)result;
        }
    }
}
