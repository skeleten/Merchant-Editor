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
    public partial class ItemSelector
    {
        public static RoutedCommand SelectNextItemCommand = new RoutedCommand();
        public static RoutedCommand SelectPrevItemCommand = new RoutedCommand();
        public static RoutedCommand CloseCommand = new RoutedCommand();
        public static RoutedCommand ItemSelectCommand = new RoutedCommand();

        public static void InitializeShortcuts()
        {
            SelectNextItemCommand.InputGestures.Add(new KeyGesture(Key.Down));
            SelectPrevItemCommand.InputGestures.Add(new KeyGesture(Key.Up));
            CloseCommand.InputGestures.Add(new KeyGesture(Key.Escape));
            ItemSelectCommand.InputGestures.Add(new KeyGesture(Key.Enter));
        }

        public void StartUpdating()
        {
            if(!_running)
            {
                _running = true;
                _needUpdate = true;
                _factory.StartNew(() => _results.AddRange(DataProvider.Items));
                Task.Factory.StartNew(UpdateQuery);
            }
        }
        public void StopUpdating()
        {
            _running = false;
            _factory.StartNew(() => _results.Clear());
        }

        public event EventHandler OnItemSelected;
        public List<Item> SearchResult
        {
            get { return _results; }
        }
        private List<Item> _results;
        private bool _updating;
        private bool _needUpdate;
        private bool _running;
        private string _prevString;
        private readonly TaskScheduler _sheduler;
        private readonly TaskFactory _factory;

        public Item Item
        {
            get; private set;
        }
        public bool ItemChoosen { get; set; }
        public ItemSelector()
        {
            _sheduler = TaskScheduler.FromCurrentSynchronizationContext();
            Item = null;
            _results = new List<Item>();
            InitializeShortcuts();
            InitializeComponent();
            _updating = false;
            _needUpdate = false;
            _running = false;
            _prevString = "";
            Items.SelectedIndex = 0;
            _factory = new TaskFactory(_sheduler);

            tb.CommandBindings.Add(new CommandBinding(SelectNextItemCommand, this.SelectNextItemExecuted));
            tb.CommandBindings.Add(new CommandBinding(SelectPrevItemCommand, this.SelectPrevItemExecuted));
            tb.CommandBindings.Add(new CommandBinding(CloseCommand, this.CloseExecuted));
            tb.CommandBindings.Add(new CommandBinding(ItemSelectCommand, this.ItemSelectExecuted));
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
                        Task.Factory.StartNew(UpdateItems);
                        _updating = true;
                        _needUpdate = false;
                    }

                Thread.Sleep(Settings.UpdateInterval);
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
            _factory.StartNew(() =>
                                  {
                                      Items.Items.Refresh();
                                      if (Items.SelectedItem == null)
                                          Items.SelectedIndex = 0;
                                      foreach (var column in Items.Columns)
                                          column.Width = DataGridLength.SizeToCells;
                                  });
            _updating = false;
        }

        private void ItemsMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Items.SelectedItems.Count > 0)
            {
                ItemChoosen = true;
                Item = (Item)Items.SelectedItems[0];
                if (OnItemSelected != null)
                    OnItemSelected(this, new EventArgs());
            }
        }

        private void BtCancelClick(object sender, RoutedEventArgs e)
        {
            ItemChoosen = false;
            if (OnItemSelected != null)
                OnItemSelected(this, null);
        }

        private void SearchTextKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                    // Select Item
                case Key.Enter:
                    this.ItemSelectExecuted(null, null);
                    e.Handled = true;
                    break;
                    // Close
                case Key.Escape:
                    this.CloseExecuted(null, null);
                    e.Handled = true;
                    break;
                    // Select Next Item:
                case Key.Down:
                    this.SelectNextItemExecuted(null, null);
                    e.Handled = true;
                    break;
                    // Select previous item:
                case Key.Up:
                    this.SelectPrevItemExecuted(null, null);
                    e.Handled = true;
                    break;
                default:
                    e.Handled = false;
                    break;
            }
        }

        private void SelectNextItemExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if(Items.Items.Count > 0)
            {
                Items.SelectedIndex = (Items.SelectedIndex + 1)%Items.Items.Count;
            }
            Items.ScrollIntoView(Items.SelectedItem);
        }

        private void SelectPrevItemExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (Items.Items.Count > 0)
            {
                Items.SelectedIndex = (Items.SelectedIndex - 1) % Items.Items.Count;
            }
            Items.ScrollIntoView(Items.SelectedItem);
        }

        private void CloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ItemChoosen = false;
            if(OnItemSelected != null)
                OnItemSelected(this, new EventArgs());
        }

        private void ItemSelectExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (Items.SelectedItems.Count > 0)
            {
                ItemChoosen = true;
                Item = (Item)Items.SelectedItems[0];
                if (OnItemSelected != null)
                    OnItemSelected(this, new EventArgs());
            }
        }
    }
}