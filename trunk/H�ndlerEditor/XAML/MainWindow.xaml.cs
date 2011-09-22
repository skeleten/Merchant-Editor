using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Windows.Input;
using System.Xml;
using HändlerEditor.Code;
using Microsoft.Win32;

namespace HändlerEditor.XAML
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Shortcuts
        public static readonly RoutedCommand SaveCommand =      new RoutedCommand();
        public static readonly RoutedCommand OpenCommand =      new RoutedCommand();
        public static readonly RoutedCommand AddTabCommand =    new RoutedCommand();
        public static readonly RoutedCommand RemoveTabCommand = new RoutedCommand();
        public static readonly RoutedCommand CycleTabsCommand = new RoutedCommand();
        public static readonly RoutedCommand ExportCommand =    new RoutedCommand();
        public static readonly RoutedCommand ImportCommand =    new RoutedCommand();
        public static readonly RoutedCommand RenameTabCommand = new RoutedCommand();
        public static readonly RoutedCommand AddRowCommand =    new RoutedCommand();
        #endregion

        public MainWindow()
        {
            //Settings.Load();
            //DataProvider.Initialize();
            //IconBuffer.Initialize(Settings.IconPath);

            InitializeComponent();
            InitializeShortcuts();
        }

        private static void InitializeShortcuts()
        {
            SaveCommand.InputGestures.Add(new KeyGesture(       Key.S,      ModifierKeys.Control));
            OpenCommand.InputGestures.Add(new KeyGesture(       Key.O,      ModifierKeys.Control));
            AddTabCommand.InputGestures.Add(new KeyGesture(     Key.A,      ModifierKeys.Control));
            RemoveTabCommand.InputGestures.Add(new KeyGesture(  Key.D,      ModifierKeys.Control));
            CycleTabsCommand.InputGestures.Add(new KeyGesture(  Key.Tab,    ModifierKeys.Control));
            ExportCommand.InputGestures.Add(new KeyGesture(     Key.E,      ModifierKeys.Control));
            ImportCommand.InputGestures.Add(new KeyGesture(     Key.I,      ModifierKeys.Control));
            RenameTabCommand.InputGestures.Add(new KeyGesture(  Key.R,      ModifierKeys.Control));
            AddRowCommand.InputGestures.Add(new KeyGesture(     Key.Q,      ModifierKeys.Control));
        }

        private void BtAddClick(object sender, RoutedEventArgs e)
        {
            var ti = new TabItem
                             {
                                 Header = string.Format("tab{0:00}", tcTabs.Items.Count),
                                 Content = (new ItemPage())
                             };
            tcTabs.Items.Add(ti);
        }

        private void BtRemoveClick(object sender, RoutedEventArgs e)
        {
            if(tcTabs.Items.Count > 0)
                tcTabs.Items.RemoveAt(tcTabs.Items.Count - 1);
        }

        private void MenuOpenClick(object sender, RoutedEventArgs e)
        {
            bool thrownUnkItemMsg = false;

            var diag = new OpenFileDialog
                                        {
                                            Filter = "Merchant TXT-Files (*.txt)|*.txt",
                                            Multiselect = false
                                        };

            if(diag.ShowDialog() != true)
                return;

            tcTabs.Items.Clear();

            var tab = new ShineTableFile();
            tab.Open(diag.FileName);
            foreach (ShineTableFile.ShineTable t in tab.Tables)
            {
                var i = new TabItem
                                {
                                    Header = t.Name,
                                };
                var p = new ItemPage();
                i.Content = p;

                foreach (DataRow row in t.Content.Rows)
                {
                    var r = new ItemRow();
                    r.OnRemoveRequested += p.r_OnRemoveRequested;

                    for (int it = 0; it < 6; it++)
                        try
                        {
                            DataRow row1 = row;
                            int it1 = it;
                            try
                            {
                                if(((string)row1[it1 + 1]) == "-")
                                {
                                    r[it] = null;
                                    continue;
                                }
                                r[it] = DataProvider.Items.Single(item => item.InxName == (string)row1[it1 + 1]);
                            }
                            catch (InvalidOperationException)
                            {
                                if(!thrownUnkItemMsg)
                                {
                                    thrownUnkItemMsg = true;
                                    if((new Message("File contains unkown items.\nContinue? (Unkown items wont get displayed)", Message.VisibleButtons.YesNo).ShowDialog()) == Message.VisibleButtons.No)
                                    {
                                        tcTabs.Items.Clear();
                                        return;
                                    }
                                    continue;
                                }
                            }
                        }
                        catch (Exception)
                        {
                            continue;
                        }

                    p.spItems.Children.Add(r);
                }

                tcTabs.Items.Add(i);
            }

        }

        private void MenuSaveClick(object sender, RoutedEventArgs e)
        {
            var diag = new SaveFileDialog
                                        {
                                            Filter = "Merchant TXTs (*.txt) | *.txt"
                                        };

            if(diag.ShowDialog() != true)
                return;

            var st = new ShineTableFile();
            StaticValues.AddFileHeader(st);

            for (int i = 0; i < tcTabs.Items.Count; i++)
            {
                var tab = new ShineTableFile.ShineTable
                                           {
                                               ColumnNameLine = StaticValues.ColumnLine,
                                               NameLine = StaticValues.GetNameLine(i),
                                               TypeLine = StaticValues.TypeLine,
                                               Content = StaticValues.DataTableTemplate
                                           };

                var t = (TabItem)tcTabs.Items[i];
                var p = (ItemPage)t.Content;
                for (int it = 0; it < p.spItems.Children.Count; it++)
                {
                    var r = (ItemRow)p.spItems.Children[it];
                    DataRow row = tab.Content.NewRow();

                    row["Rec"] = it.ToString();

                    for (int a = 0; a < 6; a++)
                    {
                        if (r[a] == null)
                            row[a + 1] = "-";
                        else
                            row[a + 1] = r[a].InxName;
                    }

                    tab.Content.Rows.Add(row);
                }
                st.AddTable(tab);
            }
            st.Save(diag.FileName);
        }

        private void MenuCloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuSavePresetClick(object sender, RoutedEventArgs e)
        {
            var doc = new XmlDocument();
            XmlElement root = doc.CreateElement("Preset");
            doc.AppendChild(root);

            foreach(TabItem item in tcTabs.Items)
            {
                XmlElement tab = doc.CreateElement("Tab");
                tab.SetAttribute("Name", (string)item.Header);
                root.AppendChild(tab);
                var ip = (ItemPage) item.Content;
                foreach (ItemRow ir in ip.spItems.Children)
                {
                    XmlElement itemRow = doc.CreateElement("ItemRow");

                    if (ir.item0.Item != null)
                        itemRow.SetAttribute("Item0", ir.item0.Item.InxName);
                    if (ir.item1.Item != null)
                        itemRow.SetAttribute("Item1", ir.item1.Item.InxName);
                    if (ir.item2.Item != null)
                        itemRow.SetAttribute("Item2", ir.item2.Item.InxName);
                    if (ir.item3.Item != null)
                        itemRow.SetAttribute("Item3", ir.item3.Item.InxName);
                    if (ir.item4.Item != null)
                        itemRow.SetAttribute("Item4", ir.item4.Item.InxName);
                    if (ir.item5.Item != null)
                        itemRow.SetAttribute("Item5", ir.item5.Item.InxName);

                    tab.AppendChild(itemRow);
                }
            }

            var diag = new SaveFileDialog {Filter = "XML Preset Files (*.xml)|*.xml"};

            if(diag.ShowDialog(this) != true)
                return;

            doc.Save(diag.FileName);
        }

        private void MenuLoadPresetClick(object sender, RoutedEventArgs e)
        {
            var diag = new OpenFileDialog
                                      {
                                          Filter = "XML Preset Files (*.xml)|*.xml"
                                      };
            if (diag.ShowDialog(this) != true)
                return;

            tcTabs.Items.Clear();

            var doc = new XmlDocument();
            doc.Load(diag.FileName);

            XmlElement root = null;
            foreach(XmlElement n in doc.ChildNodes)
            {
                if(n.Name == "Preset")
                    root = n;
            }

            if(root == null)
                return;

            foreach(XmlElement tab in root.ChildNodes)
            {
                var ip = new ItemPage();
                var tb = new TabItem
                                 {
                                     Header = tab.GetAttribute("Name"),
                                     Content = ip
                                 };
                foreach(XmlElement itemRow in tab.ChildNodes)
                {
                    var ir = new ItemRow();
                    ir.OnRemoveRequested += ip.r_OnRemoveRequested;
                    var row = itemRow;                    

                    if(itemRow.GetAttribute("Item0") != "")
                        ir.item0.Item =
                            DataProvider.Items.FirstOrDefault(i => i.InxName == row.GetAttribute("Item0"));
                    if(itemRow.GetAttribute("Item1") != "")
                        ir.item1.Item =
                            DataProvider.Items.FirstOrDefault(i => i.InxName == row.GetAttribute("Item1"));
                    if(itemRow.GetAttribute("Item2") != "")
                        ir.item2.Item =
                            DataProvider.Items.FirstOrDefault(i => i.InxName == row.GetAttribute("Item2"));
                    if(itemRow.GetAttribute("Item3") != "")
                        ir.item3.Item =
                            DataProvider.Items.FirstOrDefault(i => i.InxName == row.GetAttribute("Item3"));
                    if(itemRow.GetAttribute("Item4") != "")
                        ir.item4.Item =
                            DataProvider.Items.FirstOrDefault(i => i.InxName == row.GetAttribute("Item4"));
                    if(itemRow.GetAttribute("Item5") != "")
                        ir.item5.Item =
                            DataProvider.Items.FirstOrDefault(i => i.InxName == row.GetAttribute("Item5"));
                    ip.spItems.Children.Add(ir);
                }

                tcTabs.Items.Add(tb);
            }
            tcTabs.Items.Refresh();
        }

        private void MenuRenameTabClick(object sender, RoutedEventArgs e)
        {
            if (tcTabs.Items.Count <= 0)
                return;

            var activeTab = (TabItem) tcTabs.SelectedItem;
            if (activeTab == null)
                return;

            if(tcTabs.Items.Count == 0)
                return;

            var tabItem = (TabItem)tcTabs.Items[tcTabs.SelectedIndex];
            var window = new RenameTabWindow {tbNewTabName = {Text = (string) tabItem.Header}};
            window.tbNewTabName.SelectAll();
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Closed += WindowClosed;
            window.ShowDialog();
        }

        void WindowClosed(object sender, EventArgs e)
        {
            var window = (RenameTabWindow) sender;
            if(!window.IsNameSelected)
                return;

            var tabItem = (TabItem) tcTabs.Items[tcTabs.SelectedIndex];
            tabItem.Header = window.NewName;
        }

        private void SaveCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MenuSaveClick(null, null);
            e.Handled = true;
        }

        private void OpenCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MenuOpenClick(null, null);
            e.Handled = true;
        }

        private void AddTabCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            BtAddClick(null, null);
            e.Handled = true;
        }

        private void RemoveTabExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            BtRemoveClick(null, null);
            e.Handled = true;
        }

        private void CycleTabsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if(tcTabs.Items.Count <= 0)
                return;
            tcTabs.SelectedIndex = (tcTabs.SelectedIndex + 1)%tcTabs.Items.Count;
            e.Handled = true;
        }

        private void ExportExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MenuSavePresetClick(null, null);
            e.Handled = true;
        }

        private void ImportExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MenuLoadPresetClick(null, null);
            e.Handled = true;
        }

        private void RenameTabExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MenuRenameTabClick(null, null);
            e.Handled = true;
        }

        private void AddRowCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if(tcTabs.Items.Count <= 0)
                return;

            var activeTab = (TabItem) tcTabs.SelectedItem;
            if (activeTab == null)
                return;
            var activeItemPage = activeTab.Content as ItemPage;
            if(activeItemPage == null)
                return;
            activeItemPage.AddRow(null, null);
        }
    }
}
