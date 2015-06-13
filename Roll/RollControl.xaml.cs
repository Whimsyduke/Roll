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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;

namespace Roll
{
    /// <summary>
    /// RollControl.xaml 的交互逻辑
    /// </summary>
    public partial class RollControl : UserControl
    {
        MainWindow Main;
        public RollControl(MainWindow window, int index)
        {
            InitializeComponent();
            this.Index.Content = index;
            Main = window;
        }

        public RollControl(MainWindow window, XElement xml)
        {
            InitializeComponent();
            this.Index.Content = xml.Attribute("Index").Value;
            this.MinValue.Text = xml.Attribute("MinValue").Value;
            this.MaxValue.Text = xml.Attribute("MaxValue").Value;
            Main = window;
        }

        private void Minus_Click(object sender, RoutedEventArgs e)
        {
            if (IsNumberic())
                this.CountValue.Text = (System.Int32.Parse(this.CountValue.Text) - 1).ToString();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (IsNumberic())
                this.CountValue.Text = (System.Int32.Parse(this.CountValue.Text) + 1).ToString();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            this.CountValue.Text = "0";
        }
        
        private bool IsNumberic()
        {
            if (string.IsNullOrEmpty(this.CountValue.Text))
            {
                MessageBox.Show("第" + this.Index.Content + "行计数数值为空！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            try
            {
                System.Int32.Parse(this.CountValue.Text);
            }
            catch
            {
                MessageBox.Show("第" + this.Index.Content + "行计数数值非整数数字！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        public XElement ToXML()
        {
            try
            {
                System.Int32.Parse(this.MinValue.Text);
                System.Int32.Parse(this.MaxValue.Text);
            }
            catch
            {
                MessageBox.Show("第" + this.Index.Content + "行配置无效！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            XElement root = new XElement("Index_" + Index.Content.ToString(),
                new XAttribute("Index", this.Index.Content),
                new XAttribute ("MinValue", this.MinValue.Text),
                new XAttribute ("MaxValue", this.MaxValue.Text)
                );
            return root;
        }

        public void Generation(int seed)
        {
            int min, max;
            try
            {
                min = System.Int32.Parse(this.MinValue.Text) - 1;
                max = System.Int32.Parse(this.MaxValue.Text) + 1;
            }
            catch
            {
                MessageBox.Show("第" + this.Index.Content + "行最小值最大值无效！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (min > max - 2)
            {
                MessageBox.Show("第" + this.Index.Content + "行设置的最小值大于最大值！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Random ran = new Random(seed);
            int value = ran.Next(min, max);
            while (value == max || value == min)
            {
                value = ran.Next(min, max);
            }
            RanValue.Content = value;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Main.RollList.Items.Remove(this);
            int i = 1;
            foreach (RollControl select in Main.RollList.Items)
            {
                select.Index.Content = i;
                i++;
            }
            Main.RollNumber.Text = (i - 1).ToString();
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            int index = Main.RollList.Items.IndexOf(this);
            Main.RollList.Items.Insert(index, new RollControl(Main, index));
            int i = 1;
            foreach (RollControl roll in Main.RollList.Items)
            {
                roll.Index.Content = i;
                i++;
            } 
            Main.RollNumber.Text = (i - 1).ToString();
        }
    }
}
