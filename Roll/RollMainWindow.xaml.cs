﻿using System;
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
        const string FILE_NAME_LOG = "DnDRoll.log";
        const string FILE_NAME_CONFIG = "config.ini";
        XElement m_Xml;
        private static Random Ran;
        StreamWriter sw;
        
        public MainWindow()
        {
            InitializeComponent();
            if (File.Exists(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + FILE_NAME_CONFIG))
            {
                m_Xml = XElement.Load(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + FILE_NAME_CONFIG);
                try
                {
                    int openPageIndex = System.Int32.Parse(m_Xml.Attribute("OpenPage").Value);
                    if (openPageIndex != 0 && openPageIndex != 1) throw new Exception();
                    SetOpenPageIndex(openPageIndex);

                    XElement rollConfig = m_Xml.Element("RollPageConfig");
                    this.RollNumber.Text = rollConfig.Attribute("RollNumber").Value;
                    int rollNum = System.Int32.Parse(this.RollNumber.Text);
                    for (int i = 1; i <= rollNum; i++)
                    {
                        RollList.Items.Add(new RollControl(this, rollConfig.Element("Index_" + i.ToString())));
                    }
                    if (rollNum == 0)
                    {
                        RollList.Items.Add(new RollControl(this, 1));
                        this.RollNumber.Text = "1";
                    }
                    XElement dnDConfig = m_Xml.Element("DnDPageConfig");
                    foreach (XElement select in dnDConfig.Elements())
                    {
                        PlayerNameList.Items.Add(new PlayerListItem(this, select.Value));
                    }
                }
                catch
                {
                    MessageBox.Show("配置文件出错，恢复初始设置！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    RollList.Items.Clear();
                    this.RollNumber.Text = "1";
                    RollList.Items.Add(new RollControl(this, 1));
                }
            }
            else
            {
                this.RollNumber.Text = "1";
                RollList.Items.Add(new RollControl(this, 1));
            }
            Ran = new Random();
            try
            {
                if (!File.Exists(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + FILE_NAME_LOG))
                {
                    sw = File.CreateText(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + FILE_NAME_LOG);
                }
                else
                {
                    StreamReader sr = new StreamReader(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + FILE_NAME_LOG);
                    int i = 0;
                    string log = sr.ReadToEnd();
                    List<string> logList = log.Split('\n').ToList();
                    while (logList.Count != 0 && i < 20)
                    {
                        i++;
                        DnDLogTextBox.Text += logList.Last();
                        logList.RemoveAt(logList.Count - 1);
                    }
                    sr.Close();
                    sw = new StreamWriter(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + FILE_NAME_LOG, true);
                }
                sw.AutoFlush = true;
            }
            catch
            {
                MessageBox.Show("日志文件" + System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + FILE_NAME_LOG + "被占用！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
            string[] Args = Environment.GetCommandLineArgs();
            switch (Args.Length)
            {
                case 1:
                    return;
                case 2:
                    if (Args[1] == "-r" || Args[1] == "-R")
                    {
                        SetOpenPageIndex(0);
                        foreach (RollControl roll in RollList.Items)
                        {
                            roll.Generation(Ran.Next());
                        }
                    }
                    return;
                default:
                    MessageBox.Show("无效的参数！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
            }
        }

        private void RollToXML()
        {
            if (File.Exists(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + FILE_NAME_CONFIG))
            {
                m_Xml = XElement.Load(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + FILE_NAME_CONFIG);
                m_Xml.Element("RollPageConfig").Remove();
                XElement rollConfig = new XElement("RollPageConfig",
                    new XAttribute("RollNumber", RollNumber.Text));
                m_Xml.Add(rollConfig);
                foreach (RollControl roll in RollList.Items)
                {
                    rollConfig.Add(roll.ToXML());
                }
                m_Xml.Element("RollPageConfig").Remove();
                m_Xml.Add(rollConfig);
            }
            else
            {
                m_Xml = new XElement("Config",
                    new XAttribute("OpenPage", 0));
                XElement rollConfig = new XElement("RollPageConfig",
                    new XAttribute("RollNumber", RollNumber.Text));
                m_Xml.Add(rollConfig);
                foreach (RollControl roll in RollList.Items)
                {
                    rollConfig.Add(roll.ToXML());
                }
                XElement DndConfig = new XElement("DnDPageConfig");
                m_Xml.Add(DndConfig);
            }
            m_Xml.Save(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + FILE_NAME_CONFIG);
        }

        private void ConfigToXML()
        {
            if (File.Exists(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + FILE_NAME_CONFIG))
            {
                m_Xml = XElement.Load(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + FILE_NAME_CONFIG);
                m_Xml.Attribute("OpenPage").Value = GetOpenPageIndex().ToString();
            }
            else
            {
                m_Xml = new XElement("Config",
                    new XAttribute("OpenPage", GetOpenPageIndex().ToString()));
                XElement rollConfig = new XElement("RollPageConfig",
                    new XAttribute("RollNumber", 0));
                m_Xml.Add(rollConfig);
                XElement dnDConfig = new XElement("DnDPageConfig");
                m_Xml.Add(dnDConfig);
            }
            m_Xml.Save(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + FILE_NAME_CONFIG);
        }

        public void DnDToXML(bool add, string name)
        {

            if (File.Exists(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + FILE_NAME_CONFIG))
            {
                m_Xml = XElement.Load(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + FILE_NAME_CONFIG);
                XElement dnDConfig = m_Xml.Element("DnDPageConfig");
                if (add)
                {
                    dnDConfig.Add(new XElement("_" + name.GetHashCode().ToString(), name));
                }
                else
                {
                    dnDConfig.Element("_" + name.GetHashCode().ToString()).Remove();
                }
            }
            else
            {
                m_Xml = new XElement("Config",
                    new XAttribute("OpenPage", 0));
                XElement rollConfig = new XElement("RollPageConfig",
                    new XAttribute("RollNumber", 0));
                m_Xml.Add(rollConfig);
                XElement dnDConfig = new XElement("DnDPageConfig");
                if (add) dnDConfig.Add(new XElement("_" + name.GetHashCode().ToString(), name));
                m_Xml.Add(dnDConfig);
            }
            m_Xml.Save(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + FILE_NAME_CONFIG);
        }

        public void DnDCleanToXML()
        {

            if (File.Exists(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + FILE_NAME_CONFIG))
            {
                m_Xml = XElement.Load(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + FILE_NAME_CONFIG);
                XElement dnDConfig = m_Xml.Element("DnDPageConfig");
                dnDConfig.RemoveAll();
            }
            else
            {
                m_Xml = new XElement("Config",
                    new XAttribute("OpenPage", 0));
                XElement rollConfig = new XElement("RollPageConfig",
                    new XAttribute("RollNumber", 0));
                m_Xml.Add(rollConfig);
                XElement dnDConfig = new XElement("DnDPageConfig");
                m_Xml.Add(dnDConfig);
            }
            m_Xml.Save(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + FILE_NAME_CONFIG);
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
                    RollList.Items.Add(new RollControl(this, i));
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            RollToXML();
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
            if (RollList.Items.Count > 15)
            {
                RollList.Height = 454;
            }
            else
            {
                RollList.Height = Double.NaN;
            }
        }
        
        private int GetOpenPageIndex()
        {
            if (RollRadio.IsChecked == true) return 0;
            if (DnDRadio.IsChecked == true) return 1;
            return 0;
        }

        private void SetOpenPageIndex(int index)
        {
            MainTabControl.SelectedIndex = index;
            switch (index)
            {
                case 0:
                    RollRadio.IsChecked = true;
                    break;
                case 1:
                    DnDRadio.IsChecked = true;
                    break;
            }
        }

        private void AboutSave_Click(object sender, RoutedEventArgs e)
        {
            ConfigToXML();
        }

        private void NewPlayerNameTextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (NewPlayerNameTextBox.Text == "Add New Player ")
            {
                NewPlayerNameTextBox.Text = "";
                NewPlayerNameTextBox.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void NewPlayerNameTextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (NewPlayerNameTextBox.Text == "")
            {
                NewPlayerNameTextBox.Text = "Add New Player ";
                NewPlayerNameTextBox.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void ConfirmPlayerName_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 1; i < PlayerNameList.Items.Count; i++)
            {
                if ((PlayerNameList.Items[i] as PlayerListItem).PlayerName.Content.ToString() == NewPlayerNameTextBox.Text)
                {
                    MessageBox.Show("已经存在的人物名称！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            PlayerListItem item = new PlayerListItem(this, NewPlayerNameTextBox.Text);
            PlayerNameList.Items.Add(item);
            PlayerNameList.SelectedItem = item;

            DnDToXML(true, NewPlayerNameTextBox.Text);
            NewPlayerNameTextBox.Text = "Add New Player ";
        }

        private void ClosePlayerName_Click(object sender, RoutedEventArgs e)
        {
            PlayerListItem item = PlayerNameList.Items[PlayerNameList.SelectedIndex] as PlayerListItem;
            int i = PlayerNameList.Items.IndexOf(item);

            DnDToXML(false, item.PlayerName.Content.ToString());
            PlayerNameList.Items.Remove(item);
            if (PlayerNameList.Items.Count > i)
            {
                PlayerNameList.SelectedIndex = i;
            }
            else
            {
                PlayerNameList.SelectedIndex = i - 1;
            }
        }
        private void PlayerNameList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NewPlayerPanel == null) return;
            if (PlayerNameList.SelectedIndex == 0)
            {
                NewPlayerPanel.Visibility = System.Windows.Visibility.Visible;
                ClosePlayerName.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                NewPlayerPanel.Visibility = System.Windows.Visibility.Collapsed;
                ClosePlayerName.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void DnDRoll_Click(object sender, RoutedEventArgs e)
        {
            int rollTime, rollNum, additionalValue;
            try
            {
                rollTime = System.Int32.Parse(this.DndRollFirstBox.Text);
                rollNum = System.Int32.Parse(this.DndRollSecondBox.Text);
                additionalValue = System.Int32.Parse(this.AdditionalValue.Text);
                if (rollTime < 0 || rollNum < 0) throw new Exception();
            }
            catch
            {
                MessageBox.Show("骰子设置错误，请修改后重试！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (rollTime > 200 || rollNum > 200)
            {
                MessageBox.Show("骰子和骰子面数不能大于200，请修改后重试！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string playerName;
            if (PlayerNameList.SelectedIndex == 0)
            {
                playerName = "";
            }
            else
            {
                playerName = (PlayerNameList.SelectedItem as PlayerListItem).PlayerName.Content.ToString();
            }
            string rollValue = "[";
            int value = 0;
            Random ran = new Random();
            for (int i = rollTime; i > 0; i--)
            {
                int temp = ran.Next(0, rollNum + 1);
                while (temp == 0 || temp == rollNum + 1)
                {
                    temp = ran.Next(0, rollNum + 1);
                }
                value += temp;
                rollValue += temp + ",";
            }
            if (rollTime != 0)
            {
                rollValue = rollValue.Substring(0, rollValue.Count() - 1) + "]";
            }
            else
            {
                rollValue = "[]";
            }

            string additionalString;
            if (additionalValue < 0)
            {
                additionalString = "-" + System.Math.Abs(additionalValue);
            }
            else
            {
                additionalString = "+" + additionalValue;
            }
            value += additionalValue;
            string fullLog;
            string shortLog;
            if (additionalValue != 0)
            {
                fullLog = "[" + System.DateTime.Now.ToString("yyyy年MM月dd日 dddd HH时mm分ss秒") + "][" + playerName + "][" + ForTextBox.Text + "][" + rollTime + "d" + rollNum + additionalString + "] > " + value + rollValue;
                shortLog = "[" + System.DateTime.Now.ToString("HH:mm:ss") + "][" + playerName + "][" + ForTextBox.Text + "][" + rollTime + "d" + rollNum + additionalString + "] > " + value + "\n";
            }
            else
            {
                fullLog = "[" + System.DateTime.Now.ToString("yyyy年MM月dd日 dddd HH时mm分ss秒") + "][" + playerName + "][" + ForTextBox.Text + "][" + rollTime + "d" + rollNum + "] > " + value + rollValue;
                shortLog = "[" + System.DateTime.Now.ToString("HH:mm:ss") + "][" + playerName + "][" + ForTextBox.Text + "][" + rollTime + "d" + rollNum + "] > " + value + "\n";
            }
            if (FullRadio.IsChecked == true) DnDLogTextBox.Text = fullLog + "\n" + DnDLogTextBox.Text;
            if (ShortRadio.IsChecked == true) DnDLogTextBox.Text = shortLog + DnDLogTextBox.Text;

            sw.WriteLine(fullLog); 
        }

        private bool IsNumberic()
        {
            if (string.IsNullOrEmpty(this.AdditionalValue.Text))
            {
                MessageBox.Show("附加计数数值为空！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            try
            {
                System.Int32.Parse(this.AdditionalValue.Text);
            }
            catch
            {
                MessageBox.Show("附加计数数值非整数数字！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void Minus_Click(object sender, RoutedEventArgs e)
        {
            if (IsNumberic())
                this.AdditionalValue.Text = (System.Int32.Parse(this.AdditionalValue.Text) - 1).ToString();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (IsNumberic())
                this.AdditionalValue.Text = (System.Int32.Parse(this.AdditionalValue.Text) + 1).ToString();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (sw != null) sw.Close();
        }

        private void ClearLog_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确认是否清屏？\n清屏之前的内容依旧保存在log.txt中。", "提示", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                sw.WriteLine("----------------------------------------------");
                DnDLogTextBox.Text = "";
            }
        }

        private void DnDReset_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确认是否重置？重置会清除所有记录以及玩家名。\n此过程不可挽回。", "提示", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                sw.Close();
                DnDLogTextBox.Text = "";
                File.Delete(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + FILE_NAME_LOG);
                sw = File.CreateText(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + FILE_NAME_LOG);                
                while (PlayerNameList.Items.Count > 1)
                {
                    PlayerNameList.Items.RemoveAt(1);
                }
                DnDCleanToXML();
            }
        }

    }
}
