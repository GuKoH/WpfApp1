using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
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
        public Image Logo { get; private set; }
        public string Country { get; private set; }
        public string City { get; private set; }
        public string AnketaUrl { get; private set; }
        public string Mark { get; private set; }

        public void SetMark(int mark) { Mark = mark.ToString(CultureInfo.CurrentCulture); }

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
            Logo = image;
            Country = country;
            City = city;
            AnketaUrl = anketaurl;
        }

    }

    public class Database
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1051:Не объявляйте видимые поля экземпляров", Justification = "<Ожидание>")]
        public SQLiteConnection DbConnection;
        public Database()
        {
            DbConnection = new SQLiteConnection(@"Data Source = Companies.sqlite3");
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
                    ", CONSTRAINT[sqlite_autoindex_Companies_1] PRIMARY KEY([Id]));", DbConnection);
                OpenConnection();
                command.ExecuteNonQuery();
                command.Dispose();


                //Cоздать таблицу Comments
                command = new SQLiteCommand("CREATE TABLE [Comments] ([Id] INTEGER NOT NULL, [CompanyName] TEXT NOT NULL, [Author] TEXT NULL, [Comment] TEXT NOT NULL, [Uid] INTEGER NOT NULL, [Time] TEXT NOT NULL, CONSTRAINT[PK_Comments] PRIMARY KEY([Id])); ", DbConnection);

                command.ExecuteNonQuery();
                command.Dispose();

                //Создать таблицу Marks
                command = new SQLiteCommand("CREATE TABLE [Marks] ([Id] INTEGER NOT NULL, [CompanyName] TEXT NOT NULL, [Mark] INTEGER NOT NULL, [uid] INTEGER NOT NULL, CONSTRAINT [PK_Marks] PRIMARY KEY ([Id])); ", DbConnection);

                command.ExecuteNonQuery();
                command.Dispose();

                //Создать таблицу Settings
                command = new SQLiteCommand("CREATE TABLE [Settings] ([Id] INTEGER NOT NULL, [Param] TEXT NOT NULL, [Value] INTEGER NULL, [TextValue] TEXT NULL, CONSTRAINT[PK_Settings] PRIMARY KEY([Id])); ", DbConnection);

                command.ExecuteNonQuery();
                command.Dispose();

                //Создать Uid
                command = new SQLiteCommand("INSERT INTO Settings (Param,Value) Values ('Uid',@value)", DbConnection);
                var uid = new Random();
                string i = uid.Next(10000000, 99999999).ToString(CultureInfo.CurrentCulture);
                command.Parameters.AddWithValue("value", i);
                command.ExecuteNonQuery();
                command.Dispose();
                CloseConnection();
            }
        }
        //Открытие соединения с локальной базой данных
        public void OpenConnection()
        {
            if (DbConnection.State != System.Data.ConnectionState.Open)
            {
                DbConnection.Open();
            }
        }

        //Закрытие соединения
        public void CloseConnection()
        {
            if (DbConnection.State != System.Data.ConnectionState.Closed)
            {
                DbConnection.Close();
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

        public Comment() { }
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
            command.Dispose();
        }

        public async Task<List<Comment>> GetCommentsAsync(string company, SqlConnection sqlConnection)
        {
            return await Task.Run(() => GetComments(company, sqlConnection)).ConfigureAwait(true);


        }

        public static List<Comment> GetComments(string company, SqlConnection sqlConnection)
        {

            List<Comment> commentsList = new List<Comment>();
            SqlCommand command = new SqlCommand($"SELECT * FROM Comments WHERE CompanyName = '{company}'", sqlConnection);
            SqlDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                Comment newComment = new Comment(dataReader["CompanyName"].ToString(), dataReader["Author"].ToString(), dataReader["Comment"].ToString(), int.Parse(dataReader["Uid"].ToString(), NumberStyles.Integer, CultureInfo.CurrentCulture), dataReader["Time"].ToString());
                commentsList.Add(newComment);
            }
            dataReader.Close();
            command.Dispose();
            dataReader.Dispose();
            return commentsList;
        }
    }

    public class MyMailItem
    {
        public string MailSubject { get; private set; }
        public string MailBody { get; private set; }
        public string MailDate { get; private set; }
        public string MailFrom { get; private set; }

        public void SetMail(string from, string subject, string body, string date)
        {
            MailSubject = subject;
            MailBody = body;
            MailDate = date;
            MailFrom = from;
        }
    }

    public class Marks
    {
        public string CompanyName { get; private set; }
        public int Mark { get; private set; }
        public int Uid { get; private set; }
        public SqlConnection SqlConnection { get; private set; }
        public Marks(string company, int mark, int uid, SqlConnection sqlConnectionString)
        {
            CompanyName = company;
            Mark = mark;
            Uid = uid;
            SqlConnection = sqlConnectionString;
        }

        public void AddMark()
        {
            SqlCommand commanda = new SqlCommand($"SELECT CompanyName,uid FROM Marks WHERE CompanyName='{CompanyName}' AND uid='{Uid}' ", SqlConnection);
            SqlDataReader sqlDataReader = commanda.ExecuteReader();
            int i = 0;
            while (sqlDataReader.Read())
            {
                i++;
            }

            if (i == 0)
            {
                SqlCommand command = new SqlCommand($"INSERT CompanyName,Mark,Uid INTO Marks VALUES ('{CompanyName}','{Mark}','{Uid}')", SqlConnection);
                command.ExecuteNonQuery();
                command.Dispose();

            }
            else if (i == 1)
            {
                SqlCommand command = new SqlCommand($"UPDATE Marks SET Mark = '{Mark}' WHERE Uid = '{Uid}')", SqlConnection);
                command.ExecuteNonQuery();
                command.Dispose();
            }
            commanda.Dispose();
        }

        public int RecieveMarks(string company)
        {


            int sum = 0;
            int i = 0;
            SqlCommand command = new SqlCommand($"SELECT Mark FROM Marks WHERE CompanyName='{CompanyName}'", SqlConnection);
            SqlDataReader sqlDataReader = command.ExecuteReader();

            while (sqlDataReader.Read())
            {
                sum += int.Parse(sqlDataReader["Mark"].ToString(), NumberStyles.Integer, CultureInfo.CurrentCulture); i++;
            }

            double result = sum / i;
            result = System.Math.Round(result);
            command.Dispose();
            return (int)result;
        }
    }
}
