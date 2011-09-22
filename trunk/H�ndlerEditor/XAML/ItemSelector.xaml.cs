using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HändlerEditor.Code;
using System.Threading;
using System.Threading.Tasks;

namespace HändlerEditor.XAML
{
    /// <summary>
    /// Interaction logic for ItemSelector.xaml
    /// </summary>
    public partial class ItemSelector : UserControl
    {
        public void StartUpdating()
        {
            if(!_running)
            {
                _running = true;
                _needUpdate = true;
                Thread t = new Thread(UpdateQuery) { IsBackground = true };
                t.Start();
            }
        }
        public void StopUpdating()
        {
            _running = false;
        }

        public event EventHandler OnItemSelected;
        public List<Item> SearchResult
        {
            get { return _results; }
            set { _results = value; }
        }
        private List<Item> _results;
        private bool _updating;
        private bool _needUpdate;
        private bool _running;
        private string _prevString;
        private TaskScheduler _sheduler;

        public Item Item
        {
            get;
            set;
        }
        public bool ItemChoosen { get; set; }
        public ItemSelector()
        {
            _sheduler = TaskScheduler.FromCurrentSynchronizationContext();
            Item = null;
            _results = new List<Item>(DataProvider.Items);
            InitializeComponent();
            _updating = false;
            _needUpdate = false;
            _running = false;
            _prevString = "";
            Items.SelectedIndex = 0;
        }
        private void SearchTextChanged(object sender, TextChangedEventArgs e)
        {
            _needUpdate = true;
            e.Handled = true;
        }

        private void UpdateQuery()
        {
            while (_running)
            {

                if (!_updating)
                    if (_needUpdate)
                    {
                        var t = new Thread(UpdateItems);
                        _updating = true;
                        t.Start();
                        _needUpdate = false;
                    }

                Thread.Sleep(500);
            }
        }
 
        private void UpdateItems()
        {
            _updating = true;

            var t = new Task<string>(() => tb.Text);
            t.Start(_sheduler);
            t.Wait();
            string text = t.Result;

            Regex r;
            try
            {
                r = new Regex(text);
            }
            catch
            {
                _updating = false;
                return;
            }

            if (!string.IsNullOrEmpty(text) && text.Contains(_prevString))
            {
                List<Item> toRem = (from i in _results
                                    where !(r.IsMatch(i.Name) || r.IsMatch(i.InxName))
                                    select i).ToList();

                foreach (Item item in toRem)
                    _results.Remove(item);
            }
            else
            {
                SearchResult.Clear();
                var toAdd =
                    DataProvider.Items.Where(i => (r.IsMatch(i.Name) || r.IsMatch(i.InxName))).ToList();
                SearchResult.AddRange(toAdd);
            }
            _prevString = text;
            var ta = new Task(() =>
                                  { 
                                      Items.Items.Refresh();
                                      if (Items.SelectedItem == null)
                                          Items.SelectedIndex = 0;
                                      foreach (var col in Items.Columns)
                                          col.Width = DataGridLength.SizeToCells;
                                  });
            ta.Start(_sheduler);
            _updating = false;
        }

        private void ItemsMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Items.SelectedItems.Count > 0)
            {
                ItemChoosen = true;
                this.Item = (Item)Items.SelectedItems[0];
                if (OnItemSelected != null)
                    OnItemSelected(this, new EventArgs());
            }
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            this.ItemChoosen = false;
            if (OnItemSelected != null)
                OnItemSelected(this, null);
        }

        private void tb_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    this.btCancel_Click(null, null);
                    e.Handled = true;
                    break;
                case Key.Enter:
                    ItemsMouseDoubleClick(null, null);
                    e.Handled = true;
                    break;
                case Key.Down:
                    Task t = new Task(() =>
                                          {
                                              if (Items.SelectedIndex - 1 < Items.Items.Count)
                                                  Items.SelectedIndex++;
                                              Items.ScrollIntoView(Items.SelectedItem);
                                          });
                    t.Start(_sheduler);
                    e.Handled = true;
                    break;
                case Key.Up:
                    t = new Task(() =>
                                     {
                                         if (Items.SelectedIndex > 0)
                                             Items.SelectedIndex--;
                                         Items.ScrollIntoView(Items.SelectedItem);
                                     });
                    t.Start(_sheduler);
                    e.Handled = true;
                    break;
                default:
                    break;
            }
        }
    }
}