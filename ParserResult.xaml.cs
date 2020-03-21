using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            this.Owner.Close();
        }
    }

    class ParserFormItem
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
