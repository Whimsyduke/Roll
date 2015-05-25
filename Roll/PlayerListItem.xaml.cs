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
        MainWindow Main;
        ComboBox PlayerList;
        public PlayerListItem(MainWindow window, ComboBox playerList, string name)
        {
            InitializeComponent();
            PlayerName.Content = name;
            Main = window;
            PlayerList = playerList;
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Main.DnDToXML(false, this.PlayerName.Content.ToString());
            PlayerList.Items.Remove(this);
        }
    }
}
