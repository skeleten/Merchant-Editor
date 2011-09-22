using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data;
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
        public MainWindow()
        {
            //Settings.Load();
            //DataProvider.Initialize();
            //IconBuffer.Initialize(Settings.IconPath);

            InitializeComponent();
        }

        private void BtAddClick(object sender, RoutedEventArgs e)
        {
            TabItem ti = new TabItem
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
            OpenFileDialog diag = new OpenFileDialog
                                        {
                                            Filter = "Merchant TXT-Files (*.txt)|*.txt",
                                            Multiselect = false
                                        };

            if(diag.ShowDialog() != true)
                return;

            tcTabs.Items.Clear();

            ShineTableFile tab = new ShineTableFile();
            tab.Open(diag.FileName);
            foreach (ShineTableFile.ShineTable t in tab.Tables)
            {
                TabItem i = new TabItem
                                {
                                    Header = t.Name,
                                };
                ItemPage p = new ItemPage();
                i.Content = p;

                foreach (DataRow row in t.Content.Rows)
                {
                    ItemRow r = new ItemRow();
                    r.OnRemoveRequested += p.r_OnRemoveRequested;

                    for (int it = 0; it < 6; it++)
                        try
                        {
                            r[it] = DataProvider.Items.Single(item => item.InxName == (string)row[it + 1]);
                        }
                        catch (Exception)
                        {
                            // Throw message?
                            continue;
                        }

                    p.spItems.Children.Add(r);
                }

                tcTabs.Items.Add(i);
            }

            // TODO: Open and Load File
        }

        private void MenuSaveClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog diag = new SaveFileDialog
                                        {
                                            Filter = "Merchant TXTs (*.txt) | *.txt"
                                        };

            if(diag.ShowDialog() != true)
                return;

            ShineTableFile st = new ShineTableFile();
            StaticValues.AddFileHeader(st);

            for (int i = 0; i < tcTabs.Items.Count; i++)
            {
                ShineTableFile.ShineTable tab = new ShineTableFile.ShineTable
                                           {
                                               ColumnNameLine = StaticValues.ColumnLine,
                                               NameLine = StaticValues.GetNameLine(i),
                                               TypeLine = StaticValues.TypeLine,
                                               Content = StaticValues.DataTableTemplate
                                           };

                TabItem t = (TabItem)tcTabs.Items[i];
                ItemPage p = (ItemPage)t.Content;
                for (int it = 0; it < p.spItems.Children.Count; it++)
                {
                    ItemRow r = (ItemRow)p.spItems.Children[it];
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
            OpenFileDialog diag = new OpenFileDialog
                                      {
                                          Filter = "XML Preset Files (*.xml)|*.xml"
                                      };
            if (diag.ShowDialog(this) != true)
                return;

            tcTabs.Items.Clear();

            XmlDocument doc = new XmlDocument();
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
                ItemPage ip = new ItemPage();
                TabItem tb = new TabItem
                                 {
                                     Header = tab.GetAttribute("Name"),
                                     Content = ip
                                 };
                foreach(XmlElement itemRow in tab.ChildNodes)
                {
                    ItemRow ir = new ItemRow();
                    ir.OnRemoveRequested += ip.r_OnRemoveRequested;

                    if(itemRow.GetAttribute("Item0") != "")
                        ir.item0.Item =
                            DataProvider.Items.FirstOrDefault(i => i.InxName == itemRow.GetAttribute("Item0"));
                    if(itemRow.GetAttribute("Item1") != "")
                        ir.item1.Item =
                            DataProvider.Items.FirstOrDefault(i => i.InxName == itemRow.GetAttribute("Item1"));
                    if(itemRow.GetAttribute("Item2") != "")
                        ir.item2.Item =
                            DataProvider.Items.FirstOrDefault(i => i.InxName == itemRow.GetAttribute("Item2"));
                    if(itemRow.GetAttribute("Item3") != "")
                        ir.item3.Item =
                            DataProvider.Items.FirstOrDefault(i => i.InxName == itemRow.GetAttribute("Item3"));
                    if(itemRow.GetAttribute("Item4") != "")
                        ir.item4.Item =
                            DataProvider.Items.FirstOrDefault(i => i.InxName == itemRow.GetAttribute("Item4"));
                    if(itemRow.GetAttribute("Item5") != "")
                        ir.item5.Item =
                            DataProvider.Items.FirstOrDefault(i => i.InxName == itemRow.GetAttribute("Item5"));
                    ip.spItems.Children.Add(ir);
                }

                tcTabs.Items.Add(tb);
            }
            tcTabs.Items.Refresh();
        }

        private void MenuRenameTabClick(object sender, RoutedEventArgs e)
        {
            if(tcTabs.Items.Count == 0)
                return;

            var window = new RenameTabWindow();
            window.Closed += new EventHandler(window_Closed);
            window.ShowDialog();
        }

        void window_Closed(object sender, EventArgs e)
        {
            var window = (RenameTabWindow) sender;
            if(!window.IsNameSelected)
                return;

            var tabItem = (TabItem) tcTabs.Items[tcTabs.SelectedIndex];
            tabItem.Header = window.NewName;
        }
    }
}
