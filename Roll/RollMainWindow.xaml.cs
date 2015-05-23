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
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Roll
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        XElement m_Xml;
        private static Random Ran;
        public MainWindow()
        {
            InitializeComponent();
            if (File.Exists("config.ini"))
            {
                m_Xml = XElement.Load("config.ini");
                this.RollNumber.Text = m_Xml.Attribute("RollNumber").Value;
                try
                {
                    int rollNum = System.Int32.Parse(this.RollNumber.Text);
                    for (int i = 1; i <= rollNum; i++)
                    {
                        RollList.Items.Add(new RollControl(RollList, m_Xml.Element("Index_" + i.ToString())));
                    }
                }
                catch
                {
                    MessageBox.Show("配置文件出错，恢复初始设置！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    RollList.Items.Clear();
                    this.RollNumber.Text = "1";
                    RollList.Items.Add(new RollControl(RollList, 1));
                    ToXML();
                }
            }
            else
            {
                this.RollNumber.Text = "1";
                RollList.Items.Add(new RollControl(RollList, 1));
                ToXML();
            }
            Ran = new Random();
        }

        private void ToXML()
        {
            m_Xml = new XElement("Config", 
                new XAttribute("RollNumber", RollNumber.Text));
            foreach (RollControl roll in RollList.Items)
            {
                m_Xml.Add(roll.ToXML());
            }
            m_Xml.Save("config.ini");
        }
        
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            int rollNumber;
            try
            {
                rollNumber = System.Int32.Parse(this.RollNumber.Text);
            }
            catch
            {
                MessageBox.Show("生成数量的内容非整数数字！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (rollNumber < 0)
            {
                MessageBox.Show("生成数量不能为负！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (rollNumber > 50)
            {
                MessageBox.Show("生成数量不能大于50！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (RollList.Items.Count > rollNumber)
            {
                while (rollNumber != RollList.Items.Count)
                {
                    RollList.Items.RemoveAt(rollNumber);
                }
            }
            else if (RollList.Items.Count < rollNumber)
            {
                for (int i = RollList.Items.Count + 1; i <= rollNumber; i++)
                {
                    RollList.Items.Add(new RollControl(RollList, i));
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            ToXML();
        }

        private void Generation_Click(object sender, RoutedEventArgs e)
        {
            foreach (RollControl roll in RollList.Items)
            {
                roll.Generation(Ran.Next());
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (RollList.Items.Count > 10)
            {
                RollList.Height = 310;
            }
            else
            {
                RollList.Height = Double.NaN;
            }
        }
        
    }
}
