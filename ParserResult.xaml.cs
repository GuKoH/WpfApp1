using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для ParserResult.xaml
    /// </summary>
    public partial class ParserResult : Window
    {
        public ParserResult()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Owner.Close();
        }
    }

    internal class ParserFormItem
    {
        public string CompName { private set; get; }
        public string Status { private set; get; }
        public string StatusStr { private set; get; }
        public void SetItem(string name, string status)
        {
            CompName = name;
            if (status == "OK")
            {
                Status = "Assets/ok_256.png";
                StatusStr = "Успешно добавлено!";
            }
            else if (status == "Exist")
            {
                Status = "Assets/info_256.png";
                StatusStr = "Уже существует!";
            }
        }
    }
}
