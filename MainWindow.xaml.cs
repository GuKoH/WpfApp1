using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfApp1.Core;
using Outlook = Microsoft.Office.Interop.Outlook;


namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {

        #region Переменные
        //Подключения к базам данных
        private static Database _localSqlConnection;
        private static SqlConnection _globalSqlConnection;
        private SQLiteDataAdapter DataAdapter;
        private static GlobDataSet.CompaniesDataTable _gCompaniesDt;

        public static GlobDataSet GlobalDataSet;

        //Для парсера
        private ParseWorker<string[]> _parser;
        private TaskParser<string[]> _taskParser;
        private List<string> _myList;
        private string[] _longListToTestComboVirtualization;
        //Для списка компаний
        private List<Company> _companiesRow;
        private List<Company> _selectedCompanies;

        private static BitmapImage _image;

        //Для почты
        private List<MyMailItem> _allMail;
        private List<MyMailItem> _selectedMail;
        //Для комментов
        private List<Comment> _allComments;
        private static int _intUid;
        #endregion


        public MainWindow()
        {
            InitializeComponent();
            
        }



        private async void DelEntrie_Click(object sender, RoutedEventArgs e)
        {

            DelEntrie();
            await CheckSyncEntries().ConfigureAwait(false);

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            CheckSyncEntries().ConfigureAwait(false);
            CompLogo.Source = _image;
            _localSqlConnection = new Database();
            _allComments = new List<Comment>();
        }

      

        private async void AddEntrie_Click(object sender, RoutedEventArgs e)
        {
            Company item = new Company();

            item.SetItem(company1.Text, "", managerName.Text, managerPhone.Text, managerEmail.Text, infoEmail1.Text, infoPhone1.Text, infoAddress1.Text, infoUrl1.Text, CompLogo, infoCity1.Text, infoCountry.Text, infoAnketa.Text);
            AddEntries(item);
            await CheckSyncEntries().ConfigureAwait(false);
            Info_Click(this, e);
            company1.Text = "";
            managerName.Text = "";
            managerPhone.Text = "";
            managerEmail.Text = "";
            infoEmail1.Text = "";
            infoPhone1.Text = "";
            infoAddress1.Text = "";
            infoUrl1.Text = "";
            infoCity1.Text = "";
            infoCountry.Text = "";
            infoAnketa.Text = "";
            _image = null;
            CompLogo.Source = _image;

        }

        private void AddCompany_Click(object sender, RoutedEventArgs e)
        {
            Инфо.Visibility = Visibility.Hidden;
            Почта.Visibility = Visibility.Hidden;
            Добавить.Visibility = Visibility.Visible;
            Отправить.Visibility = Visibility.Hidden;
            Парсер.Visibility = Visibility.Hidden;

        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            Инфо.Visibility = Visibility.Visible;
            Добавить.Visibility = Visibility.Hidden;
            Почта.Visibility = Visibility.Hidden;
            Отправить.Visibility = Visibility.Hidden;
            Парсер.Visibility = Visibility.Hidden;
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            Инфо.Visibility = Visibility.Hidden;
            Добавить.Visibility = Visibility.Hidden;
            Почта.Visibility = Visibility.Hidden;
            Отправить.Visibility = Visibility.Visible;
            Парсер.Visibility = Visibility.Hidden;
        }

        private void loadLogo_Click(object sender, RoutedEventArgs e)
        {
            _image = new BitmapImage();
            OpenFileDialog openDialog = new OpenFileDialog
            {
                Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*" //формат загружаемого файла
            }; //создание диалогового окна для выбора файла
            if (openDialog.ShowDialog().HasValue) 
            {
                try
                {
                    _image.BeginInit();
                    _image.UriSource = new Uri(openDialog.FileName, UriKind.Absolute);
                    _image.EndInit();


                    
                    CompLogo.Source = _image;
                    CompLogo.Stretch = Stretch.UniformToFill;
                    openDialog.Reset();

                }
                catch (Exception)
                {
                 
                }
            }
        }

        #region Работа с записями

        public void AddEntries(Company item) //Добавление записей
        {
            int i = _companiesRow.Where(w => w.CompanyName.Equals(item.CompanyName, StringComparison.CurrentCultureIgnoreCase)).Count();
            if (i == 0)
            {
                SQLiteCommand addCommand = new SQLiteCommand(@"INSERT INTO [Companies] (CompanyName,ShortName,Manager1Name,Manager1Phone,Manager1Email,CompanyEmail,CompanyPhone,Address,URL,Country,City,AnketaUrl) VALUES (@CompanyName,@ShortName,@Manager1Name,@Manager1Phone,@Manager1Email,@CompanyEmail,@CompanyPhone,@Address,@URL,@Country,@City,@AnketaUrl)", _localSqlConnection.DbConnection);
                addCommand.Parameters.AddWithValue("CompanyName", item.CompanyName);
                addCommand.Parameters.AddWithValue("ShortName", item.ShortName);
                addCommand.Parameters.AddWithValue("Manager1Name", item.Manager1Name);
                addCommand.Parameters.AddWithValue("Manager1Phone", item.Manager1Phone);
                addCommand.Parameters.AddWithValue("Manager1Email", item.Manager1Email);
                addCommand.Parameters.AddWithValue("CompanyEmail", item.CompanyEmail);
                addCommand.Parameters.AddWithValue("CompanyPhone", item.CompanyPhone);
                addCommand.Parameters.AddWithValue("Address", item.Address);
                addCommand.Parameters.AddWithValue("URL", item.CompanyUrl);
                addCommand.Parameters.AddWithValue("Country", item.Country);
                addCommand.Parameters.AddWithValue("City", item.City);
                addCommand.Parameters.AddWithValue("AnketaUrl", item.AnketaUrl);
                _localSqlConnection.OpenConnection();
                addCommand.ExecuteNonQuery();
                _localSqlConnection.CloseConnection();
                addCommand.Dispose();
            }
            
            
        }

        public async void DelEntrie()
        {


            SQLiteCommand command = new SQLiteCommand("DELETE FROM [Companies] WHERE CompanyName=@name", _localSqlConnection.DbConnection);
            command.Parameters.AddWithValue("name", company.Text);
            _localSqlConnection.OpenConnection();
            command.ExecuteNonQuery();
            _localSqlConnection.CloseConnection();
            await CheckSyncEntries().ConfigureAwait(false);
            command.Dispose();
        }

        #endregion

        #region Почта
        private void SendMail_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Outlook._Application app = new Outlook.Application();
                Outlook.MailItem mail = (Outlook.MailItem)app.CreateItem(Outlook.OlItemType.olMailItem);
                mail.To = textTo.Text;
                mail.Body = textBody.Text;
                mail.Subject = textSubject.Text;
                mail.Importance = Outlook.OlImportance.olImportanceNormal;
                mail.Send();
                MessageBox.Show("Ваше сообщение успешно отправлено!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async void RecieveMail_Click(object sender, RoutedEventArgs e)
        {
            Инфо.Visibility = Visibility.Hidden;
            Добавить.Visibility = Visibility.Hidden;
            Почта.Visibility = Visibility.Visible;
            Отправить.Visibility = Visibility.Hidden;
            Парсер.Visibility = Visibility.Hidden;
            _allMail = new List<MyMailItem>();
            _selectedMail = new List<MyMailItem>();
            CancellationTokenSource ctSource = new CancellationTokenSource();
            var token = ctSource.Token;
            Outlook.MAPIFolder inbox = await Task.Factory.StartNew <Outlook.MAPIFolder>(
                                             () => CheckMail(),token,
                                             TaskCreationOptions.LongRunning,TaskScheduler.Default).ConfigureAwait(true);
            foreach (Outlook.MailItem item in inbox.Items)
            {
                MyMailItem mailItem = new MyMailItem();

                mailItem.SetMail(item.SenderEmailAddress, item.Subject, item.Body, item.SentOn.ToString(CultureInfo.InvariantCulture));
                _allMail.Add(mailItem);

            }
            _selectedMail = _allMail.Where(p => p.MailFrom.Contains(textFrom.Text)).ToList();
            MailBox.ItemsSource = _selectedMail;
            ctSource.Dispose();
            
        }

        public static Outlook.MAPIFolder CheckMail()
        {

            Outlook._Application app = new Outlook.Application();
            Outlook._NameSpace ns = app.GetNamespace("MAPI");
            Outlook.MAPIFolder inbox = ns.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderInbox);
            //_ns.SendAndReceive(true);
            return inbox;



        }

        private void textFrom_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Почта.IsVisible)
            {
                try
                {

                    _selectedMail = _allMail.Where(p => p.MailFrom.Contains(textFrom.Text)).ToList();
                    MailBox.ItemsSource = _selectedMail;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,ex.Source,MessageBoxButton.OK,MessageBoxImage.Warning);
                }
            }
        }
        #endregion

        #region Парсер
        private void btParser_Click(object sender, RoutedEventArgs e)
        {
            Инфо.Visibility = Visibility.Hidden;
            Почта.Visibility = Visibility.Hidden;
            Добавить.Visibility = Visibility.Hidden;
            Отправить.Visibility = Visibility.Hidden;
            Парсер.Visibility = Visibility.Visible;
            _parser = new ParseWorker<string[]>(new targetParser());
            _parser.OnComplited += ParserOnComplited;
            _parser.OnNewData += ParserOnNewData;
            _taskParser = new TaskParser<string[]>(new taskParser());
            _taskParser.OnComplitedTask += TaskParser_OnComplitedTask;
            _taskParser.OnNewTask += TaskParser_OnNewTask;
            _myList = new List<string>();
            _longListToTestComboVirtualization = new string[100];

            for (int i = 0; i < 100; i++)
            {
                _longListToTestComboVirtualization[i] = (i + 1).ToString(CultureInfo.CurrentCulture);
            }

            StartPage.ItemsSource = _longListToTestComboVirtualization;
            EndPage.ItemsSource = _longListToTestComboVirtualization;
        }

        private void TaskParser_OnNewTask(object arg1, string[] list)
        {
            _myList.AddRange(list);
            Progress.Value = Progress.Value + 1;

        }

        private void TaskParser_OnComplitedTask(object obj)
        {
            ParserBox.ItemsSource = _myList;
            ParserBox.ItemsSource.GetEnumerator();
            MessageBox.Show("Все страницы добавлены в задачу");
            _parser.Settings = new targetParserSettings(_taskParser.Settings.BaseUrl, _myList.ToArray(), _myList.Count);
            _parser.Start();


        }

        public static void ParserOnComplited(object o)
        {
          
            MessageBox.Show("Работа завершена!");
        }

        public static string StripHtml(string htmlText)
        {
            string decode = HttpUtility.HtmlDecode(htmlText);
            return decode;
        }

        public static string Obrezat(string str)
        {
            
            char[] myChar = str.ToCharArray();
            int i = 0;
            try
            {

                while (!myChar[i].Equals('>'))
                {
                    myChar[i] = '\0';
                    i++;
                }
                myChar[i] = '\0';
                while (!myChar[i].Equals('<'))
                {
                    i++;
                }
                while (!myChar[i].Equals('>'))
                {
                    myChar[i] = '\0';
                    i++;

                }
                myChar[i] = '\0';
                while (!myChar[i].Equals('>'))
                {
                    myChar[i] = '\0';
                    i++;

                }
                myChar[i] = '\0';
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
               
            
            string result = new string(myChar);


           
            while (result.Contains("\0"))
            {
                result = result.Replace("\0", "");
            }

            return result;
        }

        public void ParserOnNewData(object o, string[] str, string[] name)
        {
            Company parcerItem = new Company();
            string result = "";
            int a = 0, b = 0, d = 0;
            for (int i = 0; i < str.Length; i++)
            {
                switch (str[i])
                {
                    case "Адрес":
                        a = i;
                        txCity.Text = str[i + 1];
                        break;
                    case "Телефон":
                        b = i;
                        txTel.Text = str[i + 1];
                        break;
                    case "E-mail":

                        result = StripHtml(Obrezat(str[i + 1]));
                        txEmail.Text = result;
                        break;
                    case "url":
                        d = i;
                        txUrl.Text = str[i + 1];
                        break;

                }

            }
            txName.Text = name[0];
          


            parcerItem.SetItem(name[0], "", "", "", "", result, str[b + 1], str[a + 1], str[d + 1], null, "", "", "");


            AddEntries(parcerItem);
            Progress.Value = Progress.Value + 1;

        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {

            _myList.Clear();
            int taskProgress = int.Parse(EndPage.Text, NumberStyles.Integer, CultureInfo.CurrentCulture) - int.Parse(EndPage.Text, NumberStyles.Integer, CultureInfo.CurrentCulture) + 1 + (int.Parse(EndPage.Text, NumberStyles.Integer, CultureInfo.CurrentCulture) - int.Parse(EndPage.Text, NumberStyles.Integer, CultureInfo.CurrentCulture) * 10);
            Progress.Maximum = taskProgress;
            ParserResult parserResult = new ParserResult
            {
                Owner = this
            };
            parserResult.InitializeComponent();
            Progress.Value = 0;
            ParserBox.ItemsSource = "";
            _taskParser.Settings = new TaskParserSettings(txWeb.Text, txWeb1.Text, int.Parse(StartPage.Text, NumberStyles.Integer, CultureInfo.CurrentCulture), int.Parse(EndPage.Text, NumberStyles.Integer, CultureInfo.CurrentCulture));
            _taskParser.StartTask();
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            _parser.Stop();
        }

        #endregion

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {

           
            _selectedCompanies = _companiesRow.Where(w => w.CompanyName.Contains(SearchBox.Text)).ToList();
            myListBox.ItemsSource = _selectedCompanies;
        }

        #region Работа с базой данных

        ///<summary>
        ///Подключение к локальной базе данных
        ///</summary>
        private async Task LocalSqlConnection()
        {
           

            SQLiteConnection dbConnection = _localSqlConnection.DbConnection;


            try
            {
                await dbConnection.OpenAsync().ConfigureAwait(true);
                LocalSqlStatus.Foreground = Brushes.Green;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                LocalSqlStatus.Foreground = Brushes.Red;
            }

            
            DataAdapter = new SQLiteDataAdapter();
            SQLiteCommand command = new SQLiteCommand("Select * FROM Companies", dbConnection);
            _localSqlConnection.OpenConnection();
            SQLiteDataReader sqlReader = command.ExecuteReader();
            _companiesRow = new List<Company>();
            if (sqlReader.FieldCount > 0)
            {
                while (sqlReader.Read())
                {
                    string fullname = sqlReader["CompanyName"].ToString();
                    string shortname = sqlReader["ShortName"].ToString();
                    string managername = sqlReader["Manager1Name"].ToString();
                    string managerphone = sqlReader["Manager1Phone"].ToString();
                    string manageremail = sqlReader["Manager1Email"].ToString();
                    string email = sqlReader["CompanyEmail"].ToString();
                    string phone = sqlReader["CompanyPhone"].ToString();
                    string address = sqlReader["Address"].ToString();
                    string companyweb = sqlReader["URL"].ToString();
                    string country = sqlReader["Country"].ToString();
                    string city = sqlReader["City"].ToString();
                    string anketaurl = sqlReader["AnketaUrl"].ToString();

                    Company newClassItem = new Company();
                    newClassItem.SetItem(fullname, shortname, managername, managerphone, manageremail, email, phone, address, companyweb, null, country, city, anketaurl);
                    _companiesRow.Add(newClassItem);

                }
            }
            command.Dispose();
            sqlReader.Dispose();
            command = new SQLiteCommand("SELECT Value FROM Settings WHERE Param='Uid'", _localSqlConnection.DbConnection);
            sqlReader = command.ExecuteReader();
            while (sqlReader.Read())
            {

                _intUid = int.Parse(sqlReader["Value"].ToString(), CultureInfo.CurrentCulture);
            }

            _selectedCompanies = _companiesRow;
            myListBox.ItemsSource = _selectedCompanies;

            //companiesTableAdapter.Fill(companiesDT);

            _localSqlConnection.CloseConnection();
            command.Dispose();
            sqlReader.Dispose();
        }

        ///<summary>
        ///Подключение к глобальной базе данных
        ///</summary>
        private async Task GlobalSqlConnection()
        {
            _globalSqlConnection = new SqlConnection(@"Data Source=marinedb.ddns.net;User ID=myUser;Password=111222;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");


            try
            {
                await _globalSqlConnection.OpenAsync().ConfigureAwait(true);
                GlobalSqlStatus.Foreground = Brushes.Green;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                GlobalSqlStatus.Foreground = Brushes.Red;
            }

            GlobalDataSet = new GlobDataSet();
            _gCompaniesDt = new GlobDataSet.CompaniesDataTable();
            GlobDataSetTableAdapters.CompaniesTableAdapter gCompaniesTa = new GlobDataSetTableAdapters.CompaniesTableAdapter();
            gCompaniesTa.Fill(_gCompaniesDt);
            glCount.Text = _gCompaniesDt.Rows.Count.ToString(CultureInfo.CurrentCulture);
            gCompaniesTa.Dispose();
        }

        /// Кнопка синхронизации баз данных
        private void SyncDB_Click(object sender, RoutedEventArgs e)
        {
            SyncronizeDb();
        }

        /// <summary>
        /// Проверка синхронности Баз данных
        /// </summary>
        private async Task CheckSyncEntries()
        {
            await GlobalSqlConnection().ConfigureAwait(true);
            await LocalSqlConnection().ConfigureAwait(true);
            DataAdapter = new SQLiteDataAdapter();
            SQLiteCommand command = new SQLiteCommand("Select * FROM Companies", _localSqlConnection.DbConnection);
            _localSqlConnection.OpenConnection();
            SQLiteDataReader reader = command.ExecuteReader();
            int i = 0;
            while (reader.Read())
            { i++; }
            lcCount.Text = i.ToString(CultureInfo.CurrentCulture);
            _localSqlConnection.CloseConnection();
            uidTx.Text = _intUid.ToString(CultureInfo.CurrentCulture);
            if (int.Parse(glCount.Text,NumberStyles.AllowLeadingWhite ,CultureInfo.CurrentCulture) != int.Parse(lcCount.Text, NumberStyles.AllowLeadingWhite, CultureInfo.CurrentCulture))
            {
                SyncStatus.Text = "Необходимо синхронизировать базы данных";
                SyncDB.Visibility = Visibility.Visible;
            }
            else
            {
                SyncStatus.Text = "Базы данных синхронны.";
                SyncDB.Visibility = Visibility.Hidden;
            }
            command.Dispose();
        }


        private async void SyncronizeDb()
        {
            SQLiteCommand lCommand = new SQLiteCommand("Select * from Companies", _localSqlConnection.DbConnection);
            _localSqlConnection.OpenConnection();
            SQLiteDataReader localReader = lCommand.ExecuteReader();
            // DataTableReader LocalReader = companiesDT.CreateDataReader();
            DataTableReader globalReader = _gCompaniesDt.CreateDataReader();
            while (await localReader.ReadAsync().ConfigureAwait(true))
            {
                int i = _gCompaniesDt.Select($"CompanyName = '{localReader["CompanyName"]}'").Length;
                if (i == 0)
                {
                    SqlCommand gAddRow = new SqlCommand("INSERT INTO [Companies] (CompanyName,ShortName,Manager1Name,Manager1Phone,Manager1Email,CompanyEmail,CompanyPhone,Address,URL,Country,City,AnketaUrl) VALUES (@CompanyName,@ShortName,@Manager1Name,@Manager1Phone,@Manager1Email,@CompanyEmail,@CompanyPhone,@Address,@URL,@Country,@City,@AnketaUrl)", _globalSqlConnection);
                    gAddRow.Parameters.AddWithValue("CompanyName", localReader["CompanyName"]);
                    gAddRow.Parameters.AddWithValue("ShortName", localReader["ShortName"]);
                    gAddRow.Parameters.AddWithValue("Manager1Name", localReader["Manager1Name"]);
                    gAddRow.Parameters.AddWithValue("Manager1Phone", localReader["Manager1Phone"]);
                    gAddRow.Parameters.AddWithValue("Manager1Email", localReader["Manager1Email"]);
                    gAddRow.Parameters.AddWithValue("CompanyEmail", localReader["CompanyEmail"]);
                    gAddRow.Parameters.AddWithValue("CompanyPhone", localReader["CompanyPhone"]);
                    gAddRow.Parameters.AddWithValue("Address", localReader["Address"]);
                    gAddRow.Parameters.AddWithValue("URL", localReader["URL"]);
                    gAddRow.Parameters.AddWithValue("Country", localReader["Country"]);
                    gAddRow.Parameters.AddWithValue("City", localReader["City"]);
                    gAddRow.Parameters.AddWithValue("AnketaUrl", localReader["AnketaUrl"]);
                    // G_AddRow.Parameters.Add(new SqlParameter("@logo", LocalReader["logo"]));
                    await gAddRow.ExecuteNonQueryAsync().ConfigureAwait(true);
                    gAddRow.Dispose();
                }
            }
            lCommand.Dispose();
            while (await globalReader.ReadAsync().ConfigureAwait(true))
            {
                int i = _companiesRow.Count(w => w.CompanyName.Equals(globalReader["CompanyName"].ToString(),StringComparison.CurrentCulture));
                if (i == 0)
                {
                    SQLiteCommand lAddRow = new SQLiteCommand("INSERT INTO [Companies] (CompanyName,ShortName,Manager1Name,Manager1Phone,Manager1Email,CompanyEmail,CompanyPhone,Address,URL,Country,City,AnketaUrl) VALUES (@CompanyName,@ShortName,@Manager1Name,@Manager1Phone,@Manager1Email,@CompanyEmail,@CompanyPhone,@Address,@URL,@Country,@City,@AnketaUrl)", _localSqlConnection.DbConnection);
                    lAddRow.Parameters.AddWithValue("CompanyName", globalReader["CompanyName"]);
                    lAddRow.Parameters.AddWithValue("ShortName", globalReader["ShortName"]);
                    lAddRow.Parameters.AddWithValue("Manager1Name", globalReader["Manager1Name"]);
                    lAddRow.Parameters.AddWithValue("Manager1Phone", globalReader["Manager1Phone"]);
                    lAddRow.Parameters.AddWithValue("Manager1Email", globalReader["Manager1Email"]);
                    lAddRow.Parameters.AddWithValue("CompanyEmail", globalReader["CompanyEmail"]);
                    lAddRow.Parameters.AddWithValue("CompanyPhone", globalReader["CompanyPhone"]);
                    lAddRow.Parameters.AddWithValue("Address", globalReader["Address"]);
                    lAddRow.Parameters.AddWithValue("URL", globalReader["URL"]);
                    lAddRow.Parameters.AddWithValue("Country", globalReader["Country"]);
                    lAddRow.Parameters.AddWithValue("City", globalReader["City"]);
                    lAddRow.Parameters.AddWithValue("AnketaUrl", globalReader["AnketaUrl"]);
                    // L_AddRow.Parameters.Add(new SQLiteParameter("@logo", GlobalReader["logo"]));
                    await lAddRow.ExecuteNonQueryAsync().ConfigureAwait(true);
                    lAddRow.Dispose();
                }
            }
            
            _localSqlConnection.CloseConnection();
            await CheckSyncEntries().ConfigureAwait(true);
        }



        #endregion

        #region Комментарии

        private void AddComment_Click(object sender, RoutedEventArgs e)
        {
            string time = DateTime.UtcNow.ToString(CultureInfo.CurrentCulture);
            Comment newComment = new Comment(company.Text, AuthorTX.Text, CommentTX.Text, _intUid, time);
            newComment.AddComment(_globalSqlConnection);
            _allComments.Add(newComment);
            CommentBox.ItemsSource = _allComments;
        }

        private async void company_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Company item = (Company)myListBox.SelectedItem;
            Comment comment = new Comment();
            _allComments = await comment.GetCommentsAsync(item.CompanyName, _globalSqlConnection).ConfigureAwait(true);
            CommentBox.ItemsSource = _allComments;
        }
        #endregion
    }
}
