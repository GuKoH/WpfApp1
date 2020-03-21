using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Outlook = Microsoft.Office.Interop.Outlook;
using WpfApp1.Core;
using System.Web;
using Microsoft.Win32;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        #region Переменные
        SqlConnection localSqlConnection;
        SqlConnection globalSqlConnection;
        ParseWorker<string[]> Parser;
        TaskParser<string[]> TaskParser;
        List<string> myList;
        ParserResult parserResult;
        char[] myChar2;
        string[] LongListToTestComboVirtualization;
        DataTable dt;
        DataSet1.CompaniesDataTable companiesDT;
        DataSet1TableAdapters.CompaniesTableAdapter companiesTableAdapter;
        DataView dataView;
        DataSet1 dataSet1;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            Parser = new ParseWorker<string[]>(new targetParser());
            Parser.OnComplited += Parser_OnComplited;
            Parser.OnNewData += Parser_OnNewData;
            TaskParser = new TaskParser<string[]>(new taskParser());
            TaskParser.OnComplitedTask += TaskParser_OnComplitedTask;
            TaskParser.OnNewTask += TaskParser_OnNewTask;
            myList = new List<string>();
           
            GlobalSqlStatus.Foreground = Brushes.Red;


        }

       
        public CompanyItem myNewItem;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DelEntrie();
            MyDataRefresh();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            localSqlConnection = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MyLocalDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            try
            {
                localSqlConnection.Open();
                LocalSqlStatus.Foreground = Brushes.Green;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                LocalSqlStatus.Foreground = Brushes.Red;
            }

            globalSqlConnection = new SqlConnection(@"Data Source=marinedb.ddns.net;User ID=myUser;Password=111222;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            try
            {
                globalSqlConnection.Open();
                GlobalSqlStatus.Foreground = Brushes.Green;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                GlobalSqlStatus.Foreground = Brushes.Red;
            }


            LongListToTestComboVirtualization = new string[100];
            int i = 0;
            for (i = 0; i < 100; i++) LongListToTestComboVirtualization[i] = ((int)(i + 1)).ToString();
            StartPage.ItemsSource = LongListToTestComboVirtualization;
            EndPage.ItemsSource = LongListToTestComboVirtualization;


            dataSet1 = new DataSet1();
            companiesDT = new DataSet1.CompaniesDataTable();
            companiesTableAdapter = new DataSet1TableAdapters.CompaniesTableAdapter();
            dataView = companiesDT.DefaultView;
            companiesTableAdapter.Fill(companiesDT);
            myListBox.ItemsSource = companiesDT;
            dataView.RowFilter = $"CompanyName NOT LIKE ''";



        }

        public void MyDataRefresh()
        {

            //SqlDataReader SqlReader = null;
            //SqlCommand command = new SqlCommand("SELECT * FROM [Companies]", localSqlConnection);

            //try
            //{
            //    SqlReader = command.ExecuteReader();

            //    while (SqlReader.Read())
            //    {
            //        string fullname = SqlReader["CompanyName"].ToString();
            //        string shortname = SqlReader["ShortName"].ToString();
            //        string managername = SqlReader["Manager1Name"].ToString();
            //        string managerphone = SqlReader["Manager1Phone"].ToString();
            //        string manageremail = SqlReader["Manager1Email"].ToString();
            //        string email = SqlReader["CompanyEmail"].ToString();
            //        string phone = SqlReader["CompanyPhone"].ToString();
            //        string address = SqlReader["Address"].ToString();
            //        string companyweb = SqlReader["URL"].ToString();
            //        Image image = new Image();
            //        string country = SqlReader["Country"].ToString();
            //        string city = SqlReader["City"].ToString();
            //        string anketaurl = SqlReader["AnketaUrl"].ToString();

            //        CompanyItem newClassItem = new CompanyItem();
            //        newClassItem.SetItem(fullname, shortname, managername, managerphone, manageremail,  email,  phone,  address,  companyweb, image,  country,  city,  anketaurl);
            // myListBox.Items.Add(newClassItem);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message.ToString(), ex.Source.ToString());
            //}
            //finally
            //{
            //    if (SqlReader != null)
            //    {
            //        SqlReader.Close();
            //    }
            //}
            companiesTableAdapter.Fill(companiesDT);

        }


        public void AddEntries(CompanyItem item) //Добавление записей
        {
            DataSet1TableAdapters.CompaniesTableAdapter companiesNameMatchAdapter = new DataSet1TableAdapters.CompaniesTableAdapter();
            
            DataSet1.CompaniesDataTable dt = new DataSet1.CompaniesDataTable();
            DataTableReader CheckMatch = dt.CreateDataReader();
            companiesNameMatchAdapter.CheckName(dt, item.FullName);
            int i = 0;
            while (CheckMatch.Read())
                
            {
                i++;
            }
            if (i == 0)
            {

                
                try
                {
                    SqlCommand command = new SqlCommand("INSERT INTO [Companies] (CompanyName,ShortName,Manager1Name,Manager1Phone,Manager1Email,CompanyEmail,CompanyPhone,Address,URL,Country,City,AnketaUrl) VALUES (@CompanyName,@ShortName,@Manager1Name,@Manager1Phone,@Manager1Email,@CompanyEmail,@CompanyPhone,@Address,@URL,@Country,@City,@AnketaUrl)", localSqlConnection);
                    command.Parameters.AddWithValue("CompanyName", item.FullName);
                    command.Parameters.AddWithValue("ShortName", item.ShortName);
                    command.Parameters.AddWithValue("Manager1Name", item.ManagerName);
                    command.Parameters.AddWithValue("Manager1Phone", item.ManagerPhone);
                    command.Parameters.AddWithValue("Manager1Email", item.ManagerEmail);
                    command.Parameters.AddWithValue("CompanyEmail", item.Email);
                    command.Parameters.AddWithValue("CompanyPhone", item.Phone);
                    command.Parameters.AddWithValue("Address", item.Address);
                    command.Parameters.AddWithValue("URL", item.CompanyWeb);
                    command.Parameters.AddWithValue("Country", item.Country);
                    command.Parameters.AddWithValue("City", item.City);
                    command.Parameters.AddWithValue("AnketaUrl", item.AnketaUrl);
                    command.ExecuteNonQuery();
                    //   command = new SqlCommand($"UPDATE Companies SET logo = (select * from OPENROWSET(BULK {item.Logo}, single_blob) as image) where CompanyName = {item.FullName}");
                    //    command.ExecuteNonQuery();
                    // ParserFormItem item2 = new ParserFormItem();
                    // item2.SetItem(item.CompName, "OK");
                    // parserResult.parresBox.Items.Add(item2);
                }
                catch (Exception ex)
                { }
            }
            else
            {
              //  MessageBox.Show("Запись уже существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            CheckMatch.Close();
            MyDataRefresh();
        }

        public void DelEntrie()
        {


            SqlCommand command = new SqlCommand("DELETE FROM [Companies] WHERE CompanyName=@name", localSqlConnection);
            command.Parameters.AddWithValue("name", company.Text);
            command.ExecuteNonQuery();
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CompanyItem item = new CompanyItem();
            item.SetItem(company1.Text,"", managerName.Text,managerPhone.Text,managerEmail.Text, infoEmail1.Text, infoPhone1.Text, infoAddress1.Text, infoUrl1.Text, CompLogo,infoCity1.Text,infoCountry.Text,infoAnketa.Text);
            AddEntries(item);
            MyDataRefresh();
            Button_Click_3(this, e);
            company1.Text = "";
            managerName.Text ="";
            managerPhone.Text ="";
            managerEmail.Text =""; 
            infoEmail1.Text =""; 
            infoPhone1.Text =""; 
            infoAddress1.Text =""; 
            infoUrl1.Text =""; 
            CompLogo = null;
            infoCity1.Text ="";
            infoCountry.Text ="";
            infoAnketa.Text = "";
         
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Инфо.Visibility = Visibility.Hidden;
            Почта.Visibility = Visibility.Hidden;
            Добавить.Visibility = Visibility.Visible;
            Отправить.Visibility = Visibility.Hidden;
            Парсер.Visibility = Visibility.Hidden;

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Инфо.Visibility = Visibility.Visible;
            Добавить.Visibility = Visibility.Hidden;
            Почта.Visibility = Visibility.Hidden;
            Отправить.Visibility = Visibility.Hidden;
            Парсер.Visibility = Visibility.Hidden;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            Инфо.Visibility = Visibility.Hidden;
            Добавить.Visibility = Visibility.Hidden;
            Почта.Visibility = Visibility.Hidden;
            Отправить.Visibility = Visibility.Visible;
            Парсер.Visibility = Visibility.Hidden;
        }

        #region Почта
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            try
            {
                Outlook._Application _app = new Outlook.Application();
                Outlook.MailItem mail = (Outlook.MailItem)_app.CreateItem(Outlook.OlItemType.olMailItem);
                mail.To = textTo.Text;
                mail.Body = textBody.Text;
                mail.Subject = textSubject.Text;
                mail.Importance = Outlook.OlImportance.olImportanceNormal;
                ((Outlook.MailItem)mail).Send();
                MessageBox.Show("Ваше сообщение успешно отправлено!", "Отправлено");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public Outlook.Table RecieveMail(string From)
        {
            Outlook._Application _app = new Outlook.Application();
            Outlook._NameSpace _ns = _app.GetNamespace("MAPI");
            Outlook.MAPIFolder inbox = _ns.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderInbox);
            //_ns.SendAndReceive(true);
            Outlook.Table mailTable = inbox.GetTable(From, Outlook.OlTableContents.olUserItems);
            return mailTable;
        }

        private void RecieveMail_Click(object sender, RoutedEventArgs e)
        {
            Инфо.Visibility = Visibility.Hidden;
            Добавить.Visibility = Visibility.Hidden;
            Почта.Visibility = Visibility.Visible;
            Отправить.Visibility = Visibility.Hidden;
            Парсер.Visibility = Visibility.Hidden;

            MailBox.Items.Clear();

            Outlook._Application _app = new Outlook.Application();
            Outlook._NameSpace _ns = _app.GetNamespace("MAPI");
            Outlook.MAPIFolder inbox = _ns.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderInbox);
            //_ns.SendAndReceive(true);
            foreach (Outlook.MailItem item in inbox.Items)
            {
                myMailItem MailItem = new myMailItem();
                MailItem.SetMail(item.SenderEmailAddress, item.Subject,item.Body,item.SentOn.ToString());
                if (MailItem.mailFrom == textFrom.Text) MailBox.Items.Add(MailItem);
            }
            




        }


        public class myMailItem
        {
            public string mailSubject { get; private set; }
            public string mailBody { get; private set; }
            public string mailDate { get; private set; }
            public string mailFrom { get; private set; }

            public void SetMail(string From,string Subject,string Body,string Date)
            {
                mailSubject = Subject;
                mailBody = Body;
                mailDate = Date;
                mailFrom = From;
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
          while(result.Contains("\0"))
            result = result.Replace("\0","");
            return result;
        }

        public void Parser_OnNewData(object o, string[] str, string[] name)
        {
            CompanyItem parcerItem = new CompanyItem();
            string result="";
            int a=0, b=0, c=0, d=0, e=0;
            for (int i=0;i < str.Count();i++)
            {
               switch (str[i])
               {
                    case "Адрес":
                        a = i;
                        txCity.Text = str[i+1];
                        break;
                    case "Телефон":
                        b = i;
                        txTel.Text = str[i+1];
                        break;
                    case "E-mail":
                        c = i;
                        result = StripHTML(Obrezat(str[i + 1]));
                        txEmail.Text = result;
                        break;
                    case "url":
                        d = i;
                        txUrl.Text = str[i+1];
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
            
            
            parcerItem.SetItem(name[0],"", "", "", "", result,str[b+1],str[a+1],str[d+1],null, "", "", "");


            AddEntries(parcerItem);
            Progress.Value = Progress.Value + 1;

        }
        
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
           
            myList.Clear();
            int TaskProgress = int.Parse(EndPage.Text) - int.Parse(EndPage.Text) + 1 + (int.Parse(EndPage.Text) - int.Parse(EndPage.Text) * 10);
            Progress.Maximum = TaskProgress;
            ParserResult parserResult = new ParserResult();
            parserResult.Owner = this;
            parserResult.InitializeComponent();
            Progress.Value = 0;
            ParserBox.ItemsSource = "";
            //korabelParser.Settings = new TaskParserSettings(txWeb.Text, txWeb1.Text, int.Parse(StartPage.Text), int.Parse(EndPage.Text));
            //korabelParser.StartTask();
            TaskParser.Settings = new TaskParserSettings(txWeb.Text, txWeb1.Text, int.Parse(StartPage.Text), int.Parse(EndPage.Text));
            TaskParser.StartTask();
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            Parser.Stop();
        }

        #endregion

        private void loadLogo_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage image = new BitmapImage();
            OpenFileDialog open_dialog = new OpenFileDialog(); //создание диалогового окна для выбора файла
            open_dialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*"; //формат загружаемого файла
            if (open_dialog.ShowDialog() == DialogResult) //если в окне была нажата кнопка "ОК"
            {
                try
                {
                    

                    image.BeginInit();
                    image.UriSource = new Uri(open_dialog.FileName, UriKind.Absolute);
                    image.EndInit();    

                   
                    //image.Source = (ImageSource)open_dialog.FileName;
                    //вместо pictureBox1 укажите pictureBox, в который нужно загрузить изображение 
                    //this.pictureBox1.Size = image.Size;
                    CompLogo.Source = image;
                    CompLogo.InvalidateVisual();
                }
                catch
                {
                    //DialogResult rezult = MessageBox.Show("Невозможно открыть выбранный файл",
                    //"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
            dataView.RowFilter = $"CompanyName LIKE '%{SearchBox.Text}%' AND CompanyName NOT LIKE ''";
        }
    }
}
