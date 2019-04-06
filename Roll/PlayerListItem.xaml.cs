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

namespace Roll
{
    /// <summary>
    /// PlayerListItem.xaml 的交互逻辑
    /// </summary>
    public partial class PlayerListItem : ComboBoxItem
    {
        public PlayerListItem(MainWindow window, string name)
        {
            InitializeComponent();
            PlayerName.Content = name;
            Main = window;
        }

        public MainWindow Main { get; set; }

        public void Delete_Click(object sender, RoutedEventArgs e)
        {
            Main.DnDToXML(false, this.PlayerName.Content.ToString());
            int i = Main.PlayerNameList.Items.IndexOf(this);
            Main.PlayerNameList.Items.Remove(this);
            if (Main.PlayerNameList.Items.Count > i)
            {
                Main.PlayerNameList.SelectedIndex = i;
            }
            else
            {
                Main.PlayerNameList.SelectedIndex = i - 1;
            }
        }
    }
}
