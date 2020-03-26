using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
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
        private Database localSqlConnection;
        private SqlConnection globalSqlConnection;
        private SQLiteDataAdapter dataAdapter;
        private GlobDataSet.CompaniesDataTable G_CompaniesDT;
        private GlobDataSet GlobalDataSet;
        //Для парсера
        private ParseWorker<string[]> Parser;
        private TaskParser<string[]> TaskParser;
        private List<string> myList;
        private string[] LongListToTestComboVirtualization;
        //Для списка компаний
        private List<Company> CompaniesRow;
        private List<Company> SelectedCompanies;
                    
        private BitmapImage image;
        private Company myNewItem;
        //Для почты
        private List<myMailItem> AllMail;
        private List<myMailItem> SelectedMail;
        //Для комментов
        private List<Comment> AllComments;
        private int Uid;
        #endregion


        public MainWindow()
        {
            InitializeComponent();

        }

        

        private void DelEntrie_Click(object sender, RoutedEventArgs e)
        {

            DelEntrie();
            CheckSyncEntries();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            CheckSyncEntries();
            CompLogo.Source = image;
            localSqlConnection = new Database();
            AllComments = new List<Comment>();
        }

        //public void MyDataRefresh()
        //{
        //    #region Старый код
        //    //SqlDataReader SqlReader = null;
        //    //SqlCommand command = new SqlCommand("SELECT * FROM [Companies]", localSqlConnection);

        //    //try
        //    //{
        //    //    SqlReader = command.ExecuteReader();

        //    //    while (SqlReader.Read())
        //    //    {
        //    //        string fullname = SqlReader["CompanyName"].ToString();
        //    //        string shortname = SqlReader["ShortName"].ToString();
        //    //        string managername = SqlReader["Manager1Name"].ToString();
        //    //        string managerphone = SqlReader["Manager1Phone"].ToString();
        //    //        string manageremail = SqlReader["Manager1Email"].ToString();
        //    //        string email = SqlReader["CompanyEmail"].ToString();
        //    //        string phone = SqlReader["CompanyPhone"].ToString();
        //    //        string address = SqlReader["Address"].ToString();
        //    //        string companyweb = SqlReader["URL"].ToString();
        //    //        Image image = new Image();
        //    //        string country = SqlReader["Country"].ToString();
        //    //        string city = SqlReader["City"].ToString();
        //    //        string anketaurl = SqlReader["AnketaUrl"].ToString();

        //    //        CompanyItem newClassItem = new CompanyItem();
        //    //        newClassItem.SetItem(fullname, shortname, managername, managerphone, manageremail,  email,  phone,  address,  companyweb, image,  country,  city,  anketaurl);
        //    // myListBox.Items.Add(newClassItem);
        //    //    }
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    MessageBox.Show(ex.Message.ToString(), ex.Source.ToString());
        //    //}
        //    //finally
        //    //{
        //    //    if (SqlReader != null)
        //    //    {
        //    //        SqlReader.Close();
        //    //    }
        //    //}
        //    #endregion

        //    //companiesTableAdapter.Fill(companiesDT);

        //    SQLiteCommand SelectCom = new SQLiteCommand("SELECT CompanyName,CompanyEmail,CompanyPhone FROM Companies",localSqlConnection.dbConnection);
        //    dataAdapter.SelectCommand = SelectCom;
        //    localSqlConnection.OpenConnection();
        //    SQLiteDataReader reader = dataAdapter.SelectCommand.ExecuteReader();
        //    localSqlConnection.CloseConnection();
        //    CheckSyncEntries();

        //}

        private void AddEntrie_Click(object sender, RoutedEventArgs e)
        {
            Company item = new Company();

            item.SetItem(company1.Text, "", managerName.Text, managerPhone.Text, managerEmail.Text, infoEmail1.Text, infoPhone1.Text, infoAddress1.Text, infoUrl1.Text, CompLogo, infoCity1.Text, infoCountry.Text, infoAnketa.Text);
            AddEntries(item);
            CheckSyncEntries();
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
            image = null;
            CompLogo.Source = image;

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
            string imPath = "";
            image = new BitmapImage();
            OpenFileDialog open_dialog = new OpenFileDialog
            {
                Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*" //формат загружаемого файла
            }; //создание диалогового окна для выбора файла
            if (open_dialog.ShowDialog().HasValue) //если в окне была нажата кнопка "ОК"
            {
                try
                {

                    imPath = open_dialog.FileName.ToString();
                    image.BeginInit();
                    image.UriSource = new Uri(open_dialog.FileName, UriKind.Absolute);
                    image.EndInit();


                    //image.Source = (ImageSource)open_dialog.FileName;
                    //вместо pictureBox1 укажите pictureBox, в который нужно загрузить изображение 
                    //this.pictureBox1.Size = image.Size;
                    CompLogo.Source = image;
                    CompLogo.Stretch = Stretch.UniformToFill;
                    open_dialog.Reset();

                }
                catch
                {
                    //DialogResult rezult = MessageBox.Show("Невозможно открыть выбранный файл",
                    //"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #region Работа с записями

        public void AddEntries(Company item) //Добавление записей
        {
            int i = CompaniesRow.Where(w => w.CompanyName.Equals(item.CompanyName)).Count();
            if (i == 0)
            {
                SQLiteCommand AddCommand = new SQLiteCommand(@"INSERT INTO [Companies] (CompanyName,ShortName,Manager1Name,Manager1Phone,Manager1Email,CompanyEmail,CompanyPhone,Address,URL,Country,City,AnketaUrl) VALUES (@CompanyName,@ShortName,@Manager1Name,@Manager1Phone,@Manager1Email,@CompanyEmail,@CompanyPhone,@Address,@URL,@Country,@City,@AnketaUrl)", localSqlConnection.dbConnection);
                AddCommand.Parameters.AddWithValue("CompanyName", item.CompanyName);
                AddCommand.Parameters.AddWithValue("ShortName", item.ShortName);
                AddCommand.Parameters.AddWithValue("Manager1Name", item.Manager1Name);
                AddCommand.Parameters.AddWithValue("Manager1Phone", item.Manager1Phone);
                AddCommand.Parameters.AddWithValue("Manager1Email", item.Manager1Email);
                AddCommand.Parameters.AddWithValue("CompanyEmail", item.CompanyEmail);
                AddCommand.Parameters.AddWithValue("CompanyPhone", item.CompanyPhone);
                AddCommand.Parameters.AddWithValue("Address", item.Address);
                AddCommand.Parameters.AddWithValue("URL", item.CompanyUrl);
                AddCommand.Parameters.AddWithValue("Country", item.Country);
                AddCommand.Parameters.AddWithValue("City", item.City);
                AddCommand.Parameters.AddWithValue("AnketaUrl", item.AnketaUrl);
                localSqlConnection.OpenConnection();
                AddCommand.ExecuteNonQuery();
                localSqlConnection.CloseConnection();
            }
            //if (item.logo != null)
            //{
            //    byte[] myLogo = null;
            //    FileStream fileStream = new FileStream(new Uri(item.logo.Source.ToString()).LocalPath, FileMode.Open, FileAccess.Read);
            //    BinaryReader binaryReader = new BinaryReader(fileStream);
            //    myLogo = binaryReader.ReadBytes((int)fileStream.Length);
            //    AddCommand = new SQLiteCommand($"UPDATE Companies SET logo = @logo where CompanyName = @name", localSqlConnection.dbConnection);
            //    AddCommand.Parameters.Add(new SQLiteParameter("@logo", myLogo));
            //    AddCommand.Parameters.AddWithValue("name", item.CompanyName);
            //    AddCommand.ExecuteNonQuery();
            //}

            #region Код для LocalDB
            // //DataSet1TableAdapters.CompaniesTableAdapter companiesNameMatchAdapter = new DataSet1TableAdapters.CompaniesTableAdapter();
            // SQLiteDataAdapter companiesNameMatchAdapter = new SQLiteDataAdapter();
            // companiesNameMatchAdapter.SelectCommand.CommandText = $"Select CompanyName from Companies where CompanyName = {item.CompanyName}";
            // companiesNameMatchAdapter.SelectCommand.Connection = localSqlConnection.dbConnection;

            // // DataSet1.CompaniesDataTable dt = new DataSet1.CompaniesDataTable();
            // DataTable dt = new DataTable();
            // companiesNameMatchAdapter.Fill(dt);
            // DataTableReader CheckMatch = dt.CreateDataReader();
            //// companiesNameMatchAdapter.CheckName(dt, item.CompanyName);
            // int i = 0;
            // while (CheckMatch.Read())

            // {
            //     i++;
            // }
            // if (i == 0)
            // {


            //     try
            //     {


            //         SQLiteCommand command = new SQLiteCommand("INSERT INTO [Companies] (CompanyName,ShortName,Manager1Name,Manager1Phone,Manager1Email,CompanyEmail,CompanyPhone,Address,URL,Country,City,AnketaUrl) VALUES (@CompanyName,@ShortName,@Manager1Name,@Manager1Phone,@Manager1Email,@CompanyEmail,@CompanyPhone,@Address,@URL,@Country,@City,@AnketaUrl)", localSqlConnection.dbConnection);
            //         command.Parameters.AddWithValue("CompanyName", item.CompanyName);
            //         command.Parameters.AddWithValue("ShortName", item.ShortName);
            //         command.Parameters.AddWithValue("Manager1Name", item.Manager1Name);
            //         command.Parameters.AddWithValue("Manager1Phone", item.Manager1Phone);
            //         command.Parameters.AddWithValue("Manager1Email", item.Manager1Email);
            //         command.Parameters.AddWithValue("CompanyEmail", item.CompanyEmail);
            //         command.Parameters.AddWithValue("CompanyPhone", item.CompanyPhone);
            //         command.Parameters.AddWithValue("Address", item.Address);
            //         command.Parameters.AddWithValue("URL", item.CompanyUrl);
            //         command.Parameters.AddWithValue("Country", item.Country);
            //         command.Parameters.AddWithValue("City", item.City);
            //         command.Parameters.AddWithValue("AnketaUrl", item.AnketaUrl);
            //         command.ExecuteNonQuery();

            //         if (item.logo != null)
            //         {
            //             byte[] myLogo = null;
            //             FileStream fileStream = new FileStream(new Uri(item.logo.Source.ToString()).LocalPath, FileMode.Open, FileAccess.Read);
            //             BinaryReader binaryReader = new BinaryReader(fileStream);
            //             myLogo = binaryReader.ReadBytes((int)fileStream.Length);
            //             command = new SQLiteCommand($"UPDATE Companies SET logo = @logo where CompanyName = @name", localSqlConnection.dbConnection);
            //             command.Parameters.Add(new SqlParameter("@logo", myLogo));
            //             command.Parameters.AddWithValue("name", item.CompanyName);
            //             command.ExecuteNonQuery();
            //         }


            //     }
            //     catch (Exception ex)
            //     {
            //         MessageBox.Show(ex.Message);
            //     }
            // }
            // else
            // {
            //     //  MessageBox.Show("Запись уже существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
            // }
            // CheckMatch.Close();
            // MyDataRefresh();
            #endregion
        }

        public async void DelEntrie()
        {


            SQLiteCommand command = new SQLiteCommand("DELETE FROM [Companies] WHERE CompanyName=@name", localSqlConnection.dbConnection);
            command.Parameters.AddWithValue("name", company.Text);
            localSqlConnection.OpenConnection();
            command.ExecuteNonQuery();
            localSqlConnection.CloseConnection();
            await CheckSyncEntries();
        }

        #endregion

        #region Почта
        private void SendMail_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Outlook._Application _app = new Outlook.Application();
                Outlook.MailItem mail = (Outlook.MailItem)_app.CreateItem(Outlook.OlItemType.olMailItem);
                mail.To = textTo.Text;
                mail.Body = textBody.Text;
                mail.Subject = textSubject.Text;
                mail.Importance = Outlook.OlImportance.olImportanceNormal;
                mail.Send();
                MessageBox.Show("Ваше сообщение успешно отправлено!", "Отправлено");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async void RecieveMail_Click(object sender, RoutedEventArgs e)
        {
            Инфо.Visibility = Visibility.Hidden;
            Добавить.Visibility = Visibility.Hidden;
            Почта.Visibility = Visibility.Visible;
            Отправить.Visibility = Visibility.Hidden;
            Парсер.Visibility = Visibility.Hidden;
            AllMail = new List<myMailItem>();
            SelectedMail = new List<myMailItem>();
            Outlook.MAPIFolder inbox = await Task.Factory.StartNew<Outlook.MAPIFolder>(
                                             () => CheckMail(),
                                             TaskCreationOptions.LongRunning);
            foreach (Outlook.MailItem item in inbox.Items)
            {
                myMailItem MailItem = new myMailItem();
                
                MailItem.SetMail(item.SenderEmailAddress, item.Subject, item.Body, item.SentOn.ToString());
                AllMail.Add(MailItem);
                
                //if (MailItem.mailFrom == textFrom.Text)
                //{
                //    MailBox.Items.Add(MailItem);
                //}
            }
            SelectedMail = AllMail.Where(p => p.mailFrom.Contains(textFrom.Text)).ToList();
            MailBox.ItemsSource = SelectedMail;
        }

        public static Outlook.MAPIFolder CheckMail()
        {

            Outlook._Application _app = new Outlook.Application();
            Outlook._NameSpace _ns = _app.GetNamespace("MAPI");
            Outlook.MAPIFolder inbox = _ns.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderInbox);
            //_ns.SendAndReceive(true);
            return inbox;



        }

        private void textFrom_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Почта.IsVisible)
            {
                try
                {

                    SelectedMail = AllMail.Where(p => p.mailFrom.Contains(textFrom.Text)).ToList();
                    MailBox.ItemsSource = SelectedMail;
                }
                catch (Exception ex)
                {

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
            Parser = new ParseWorker<string[]>(new targetParser());
            Parser.OnComplited += Parser_OnComplited;
            Parser.OnNewData += Parser_OnNewData;
            TaskParser = new TaskParser<string[]>(new taskParser());
            TaskParser.OnComplitedTask += TaskParser_OnComplitedTask;
            TaskParser.OnNewTask += TaskParser_OnNewTask;
            myList = new List<string>();
            LongListToTestComboVirtualization = new string[100];

            for (int i = 0; i < 100; i++)
            {
                LongListToTestComboVirtualization[i] = (i + 1).ToString();
            }

            StartPage.ItemsSource = LongListToTestComboVirtualization;
            EndPage.ItemsSource = LongListToTestComboVirtualization;
        }

        private void TaskParser_OnNewTask(object arg1, string[] list)
        {
            myList.AddRange(list);
            Progress.Value = Progress.Value + 1;

        }

        private void TaskParser_OnComplitedTask(object obj)
        {
            ParserBox.ItemsSource = myList;
            ParserBox.ItemsSource.GetEnumerator();
            MessageBox.Show("Все страницы добавлены в задачу", "Готово!");
            Parser.Settings = new targetParserSettings(TaskParser.Settings.BaseUrl, myList.ToArray(), myList.Count());
            Parser.Start();


        }

        public void Parser_OnComplited(object o)
        {
            //parserResult.Show();
            MessageBox.Show("Работа завершена!");
        }

        public static string StripHTML(string HTMLText)
        {
            string decode = HttpUtility.HtmlDecode(HTMLText);
            return decode;
        }

        public string Obrezat(string str)
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
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            string result = new string(myChar);


            //  result.Substring(result.IndexOf(!null));
            while (result.Contains("\0"))
            {
                result = result.Replace("\0", "");
            }

            return result;
        }

        public void Parser_OnNewData(object o, string[] str, string[] name)
        {
            Company parcerItem = new Company();
            string result = "";
            int a = 0, b = 0, d = 0;
            for (int i = 0; i < str.Count(); i++)
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

                        result = StripHTML(Obrezat(str[i + 1]));
                        txEmail.Text = result;
                        break;
                    case "url":
                        d = i;
                        txUrl.Text = str[i + 1];
                        break;

                }

            }
            txName.Text = name[0];
            //txTel.Text = str[1];
            //string result = Obrezat(str[2]);

            //result = StripHTML(result);
            //txEmail.Text = result;
            //txUrl.Text = str[3];
            //txCity.Text = str[0];


            parcerItem.SetItem(name[0], "", "", "", "", result, str[b + 1], str[a + 1], str[d + 1], null, "", "", "");


            AddEntries(parcerItem);
            Progress.Value = Progress.Value + (double)1;

        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {

            myList.Clear();
            int TaskProgress = int.Parse(EndPage.Text) - int.Parse(EndPage.Text) + 1 + (int.Parse(EndPage.Text) - int.Parse(EndPage.Text) * 10);
            Progress.Maximum = TaskProgress;
            ParserResult parserResult = new ParserResult
            {
                Owner = this
            };
            parserResult.InitializeComponent();
            Progress.Value = 0;
            ParserBox.ItemsSource = "";
            TaskParser.Settings = new TaskParserSettings(txWeb.Text, txWeb1.Text, int.Parse(StartPage.Text), int.Parse(EndPage.Text));
            TaskParser.StartTask();
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            Parser.Stop();
        }

        #endregion

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            //dataView.RowFilter = $"CompanyName LIKE '%{SearchBox.Text}%' AND CompanyName NOT LIKE ''";
            SelectedCompanies = CompaniesRow.Where(w => w.CompanyName.Contains(SearchBox.Text)).ToList();
            myListBox.ItemsSource = SelectedCompanies;
        }

        #region Работа с базой данных

        ///<summary>
        ///Подключение к локальной базе данных
        ///</summary>
        private async Task LocalSqlConnection()
        {
            // localSqlConnection = new SqlConnection(@"data source=D:\Projects\CPP\VisStudio\WpfApp1\Companies.db;Password=2268728");

            SQLiteConnection dbConnection = localSqlConnection.dbConnection;


            try
            {
                await dbConnection.OpenAsync();
                LocalSqlStatus.Foreground = Brushes.Green;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                LocalSqlStatus.Foreground = Brushes.Red;
            }



            //LocalDataSet = new DataSet1();
            //companiesDT = new DataSet1.CompaniesDataTable();
            //companiesTableAdapter = new DataSet1TableAdapters.CompaniesTableAdapter();



            //dataAdapter.SelectCommand = command;


            #region создание таблицы
            //companiesDT = new DataTable();
            //companiesDT.Columns.Add("CompanyName");
            //companiesDT.Columns.Add("ShortName");
            //companiesDT.Columns.Add("Manager1Name");
            //companiesDT.Columns.Add("Manager1Phone");
            //companiesDT.Columns.Add("Manager1Email");
            //companiesDT.Columns.Add("CompanyEmail");
            //companiesDT.Columns.Add("CompanyPhone");
            //companiesDT.Columns.Add("Address");
            //companiesDT.Columns.Add("URL");
            //companiesDT.Columns.Add("logo");
            //companiesDT.Columns.Add("Country");
            //companiesDT.Columns.Add("City");
            //companiesDT.Columns.Add("AnketaUrl");
            #endregion


            dataAdapter = new SQLiteDataAdapter();
            SQLiteCommand command = new SQLiteCommand("Select * FROM Companies", dbConnection);
            localSqlConnection.OpenConnection();
            SQLiteDataReader SqlReader = command.ExecuteReader();
            CompaniesRow = new List<Company>();
            if (SqlReader.FieldCount > 0)
            {
                while (SqlReader.Read())
                {
                    string fullname = SqlReader["CompanyName"].ToString();
                    string shortname = SqlReader["ShortName"].ToString();
                    string managername = SqlReader["Manager1Name"].ToString();
                    string managerphone = SqlReader["Manager1Phone"].ToString();
                    string manageremail = SqlReader["Manager1Email"].ToString();
                    string email = SqlReader["CompanyEmail"].ToString();
                    string phone = SqlReader["CompanyPhone"].ToString();
                    string address = SqlReader["Address"].ToString();
                    string companyweb = SqlReader["URL"].ToString();
                    string country = SqlReader["Country"].ToString();
                    string city = SqlReader["City"].ToString();
                    string anketaurl = SqlReader["AnketaUrl"].ToString();

                    Company newClassItem = new Company();
                    newClassItem.SetItem(fullname, shortname, managername, managerphone, manageremail, email, phone, address, companyweb, null, country, city, anketaurl);
                    CompaniesRow.Add(newClassItem);

                }
            }

            command = new SQLiteCommand("SELECT Value FROM Settings WHERE Param='Uid'", localSqlConnection.dbConnection);
            SqlReader = command.ExecuteReader();
            while (SqlReader.Read())
            {
               
                Uid = int.Parse(SqlReader["Value"].ToString());
            }
           
            SelectedCompanies = CompaniesRow;
            myListBox.ItemsSource = SelectedCompanies;

            //companiesTableAdapter.Fill(companiesDT);

            localSqlConnection.CloseConnection();
        }

        ///<summary>
        ///Подключение к глобальной базе данных
        ///</summary>
        private async Task GlobalSqlConnection()
        {
            globalSqlConnection = new SqlConnection(@"Data Source=marinedb.ddns.net;User ID=myUser;Password=111222;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");


            try
            {
                await globalSqlConnection.OpenAsync();
                GlobalSqlStatus.Foreground = Brushes.Green;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                GlobalSqlStatus.Foreground = Brushes.Red;
            }

            GlobalDataSet = new GlobDataSet();
            G_CompaniesDT = new GlobDataSet.CompaniesDataTable();
            GlobDataSetTableAdapters.CompaniesTableAdapter G_CompaniesTA = new GlobDataSetTableAdapters.CompaniesTableAdapter();
            G_CompaniesTA.Fill(G_CompaniesDT);
            glCount.Text = G_CompaniesDT.Rows.Count.ToString();

        }

        /// Кнопка синхронизации баз данных
        private void SyncDB_Click(object sender, RoutedEventArgs e)
        {
            SyncronizeDB();
        }

        /// <summary>
        /// Проверка синхронности Баз данных
        /// </summary>
        private async Task CheckSyncEntries()
        {
            await GlobalSqlConnection();
            await LocalSqlConnection();
            dataAdapter = new SQLiteDataAdapter();
            SQLiteCommand command = new SQLiteCommand("Select * FROM Companies", localSqlConnection.dbConnection);
            localSqlConnection.OpenConnection();
            SQLiteDataReader reader = command.ExecuteReader();
            int i = 0;
            while (reader.Read())
            { i++; }
            lcCount.Text = i.ToString();
            localSqlConnection.CloseConnection();
            uidTx.Text = Uid.ToString();
            if (int.Parse(glCount.Text) != int.Parse(lcCount.Text))
            {
                SyncStatus.Text = "Необходимо синхронизировать базы данных";
                SyncDB.Visibility = Visibility.Visible;
            }
            else
            {
                SyncStatus.Text = "Базы данных синхронны.";
                SyncDB.Visibility = Visibility.Hidden;
            }

        }


        private async void SyncronizeDB()
        {
            SQLiteCommand L_Command = new SQLiteCommand("Select * from Companies", localSqlConnection.dbConnection);
            localSqlConnection.OpenConnection();
            SQLiteDataReader LocalReader = L_Command.ExecuteReader();
            // DataTableReader LocalReader = companiesDT.CreateDataReader();
            DataTableReader GlobalReader = G_CompaniesDT.CreateDataReader();
            while (await LocalReader.ReadAsync())
            {
                int i = G_CompaniesDT.Select($"CompanyName = '{LocalReader["CompanyName"].ToString()}'").Count();
                if (i == 0)
                {
                    SqlCommand G_AddRow = new SqlCommand("INSERT INTO [Companies] (CompanyName,ShortName,Manager1Name,Manager1Phone,Manager1Email,CompanyEmail,CompanyPhone,Address,URL,Country,City,AnketaUrl) VALUES (@CompanyName,@ShortName,@Manager1Name,@Manager1Phone,@Manager1Email,@CompanyEmail,@CompanyPhone,@Address,@URL,@Country,@City,@AnketaUrl)", globalSqlConnection);
                    G_AddRow.Parameters.AddWithValue("CompanyName", LocalReader["CompanyName"]);
                    G_AddRow.Parameters.AddWithValue("ShortName", LocalReader["ShortName"]);
                    G_AddRow.Parameters.AddWithValue("Manager1Name", LocalReader["Manager1Name"]);
                    G_AddRow.Parameters.AddWithValue("Manager1Phone", LocalReader["Manager1Phone"]);
                    G_AddRow.Parameters.AddWithValue("Manager1Email", LocalReader["Manager1Email"]);
                    G_AddRow.Parameters.AddWithValue("CompanyEmail", LocalReader["CompanyEmail"]);
                    G_AddRow.Parameters.AddWithValue("CompanyPhone", LocalReader["CompanyPhone"]);
                    G_AddRow.Parameters.AddWithValue("Address", LocalReader["Address"]);
                    G_AddRow.Parameters.AddWithValue("URL", LocalReader["URL"]);
                    G_AddRow.Parameters.AddWithValue("Country", LocalReader["Country"]);
                    G_AddRow.Parameters.AddWithValue("City", LocalReader["City"]);
                    G_AddRow.Parameters.AddWithValue("AnketaUrl", LocalReader["AnketaUrl"]);
                    // G_AddRow.Parameters.Add(new SqlParameter("@logo", LocalReader["logo"]));
                    await G_AddRow.ExecuteNonQueryAsync();
                }
            }
            while (await GlobalReader.ReadAsync())
            {
                int i = CompaniesRow.Where(w => w.CompanyName.Equals(GlobalReader["CompanyName"].ToString())).Count();
                if (i == 0)
                {
                    SQLiteCommand L_AddRow = new SQLiteCommand("INSERT INTO [Companies] (CompanyName,ShortName,Manager1Name,Manager1Phone,Manager1Email,CompanyEmail,CompanyPhone,Address,URL,Country,City,AnketaUrl) VALUES (@CompanyName,@ShortName,@Manager1Name,@Manager1Phone,@Manager1Email,@CompanyEmail,@CompanyPhone,@Address,@URL,@Country,@City,@AnketaUrl)", localSqlConnection.dbConnection);
                    L_AddRow.Parameters.AddWithValue("CompanyName", GlobalReader["CompanyName"]);
                    L_AddRow.Parameters.AddWithValue("ShortName", GlobalReader["ShortName"]);
                    L_AddRow.Parameters.AddWithValue("Manager1Name", GlobalReader["Manager1Name"]);
                    L_AddRow.Parameters.AddWithValue("Manager1Phone", GlobalReader["Manager1Phone"]);
                    L_AddRow.Parameters.AddWithValue("Manager1Email", GlobalReader["Manager1Email"]);
                    L_AddRow.Parameters.AddWithValue("CompanyEmail", GlobalReader["CompanyEmail"]);
                    L_AddRow.Parameters.AddWithValue("CompanyPhone", GlobalReader["CompanyPhone"]);
                    L_AddRow.Parameters.AddWithValue("Address", GlobalReader["Address"]);
                    L_AddRow.Parameters.AddWithValue("URL", GlobalReader["URL"]);
                    L_AddRow.Parameters.AddWithValue("Country", GlobalReader["Country"]);
                    L_AddRow.Parameters.AddWithValue("City", GlobalReader["City"]);
                    L_AddRow.Parameters.AddWithValue("AnketaUrl", GlobalReader["AnketaUrl"]);
                    // L_AddRow.Parameters.Add(new SQLiteParameter("@logo", GlobalReader["logo"]));
                    await L_AddRow.ExecuteNonQueryAsync();
                }
            }
            localSqlConnection.CloseConnection();
            await CheckSyncEntries();
        }



        #endregion

        private void AddComment_Click(object sender, RoutedEventArgs e)
        {
            string time = System.DateTime.UtcNow.ToString();
            Comment NewComment = new Comment(company.Text,AuthorTX.Text,CommentTX.Text,Uid,time);
            NewComment.AddComment(globalSqlConnection);
            AllComments.Add(NewComment);
            CommentBox.ItemsSource = AllComments;
        }

        private async void company_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Company item = (Company)myListBox.SelectedItem;
            Comment comment = new Comment(item.CompanyName, "","",0,"");
            AllComments = await comment.GetCommentsAsync(item.CompanyName, globalSqlConnection);
            CommentBox.ItemsSource = AllComments;
        }
    }
}
