using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HändlerEditor.Code;

namespace HändlerEditor.XAML {
	/// <summary>
	/// Interaction logic for ItemSelector.xaml
	/// </summary>
	public partial class ItemSelector {
		#region Shortcuts
		
		public static readonly RoutedCommand SelectNextItemCommand = new RoutedCommand();
		public static readonly RoutedCommand SelectPrevItemCommand = new RoutedCommand();
		public static readonly RoutedCommand CloseCommand = new RoutedCommand();
		public static readonly RoutedCommand ItemSelectCommand = new RoutedCommand();

		#endregion

		#region .ctor

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

			tb.CommandBindings.Add(new CommandBinding(SelectNextItemCommand, SelectNextItemExecuted));
			tb.CommandBindings.Add(new CommandBinding(SelectPrevItemCommand, SelectPrevItemExecuted));
			tb.CommandBindings.Add(new CommandBinding(CloseCommand, CloseExecuted));
			tb.CommandBindings.Add(new CommandBinding(ItemSelectCommand, ItemSelectExecuted));
		}

		#endregion

		#region Variables

		private readonly TaskFactory _factory;

		private readonly List<Item> _results;
		private readonly TaskScheduler _sheduler;
		private bool _needUpdate;
		private string _prevString;
		private bool _running;
		private bool _updating;

		public List<Item> SearchResult {
			get { return _results; }
		}

		#endregion

		#region Propertys
		
		public Item Item { get; private set; }
		public bool ItemChoosen { get; set; }

		#endregion

		#region Methods

		public static void InitializeShortcuts()
		{
			SelectNextItemCommand.InputGestures.Add(new KeyGesture(Key.Down));
			SelectPrevItemCommand.InputGestures.Add(new KeyGesture(Key.Up));
			CloseCommand.InputGestures.Add(new KeyGesture(Key.Escape));
			ItemSelectCommand.InputGestures.Add(new KeyGesture(Key.Enter));
		}

		public void StartUpdating()
		{
			if (!_running)
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

		private void UpdateQuery(){
			while (_running) {
				if (!_updating)
					if (_needUpdate) {
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

			string text = GetSearchString();

			Regex r;
			if(!TryCreateRegex(out r, text))
				return;

			if (NeedsToRemoveItems(text))
				RemoveItems(r);
			else
				AddItems(r);

			_prevString = text;
			_factory.StartNew(UpdateGrid);
			_updating = false;
		}

		private void AddItems(Regex r) {
			SearchResult.Clear();
			List<Item> toAdd =
				DataProvider.Items.Where(i => Matches(r, i)).ToList();
			SearchResult.AddRange(toAdd);
		}

		private void RemoveItems(Regex r) {
			List<Item> toRem = (from i in _results
			                    where !Matches(r, i)
			                    select i).ToList();

			foreach (Item item in toRem)
				_results.Remove(item);
		}

		private static bool Matches(Regex r, Item i)
		{
			return r.IsMatch(i.InxName)
				|| r.IsMatch(i.Name)
				|| r.IsMatch(i.DemandLevel.ToString());
		}

		private void UpdateGrid()
		{
			// Update the item list.
			Items.Items.Refresh();
			if (Items.SelectedItem == null)
				Items.SelectedIndex = 0;
			foreach (DataGridColumn column in Items.Columns)
				column.Width = DataGridLength.SizeToCells;
		}

		private bool TryCreateRegex(out Regex r, string text) {
			r = null;
			try {
				r = new Regex(text);
				return true;
			} catch {
				_updating = false;
				return false;
			}
		}

		private string GetSearchString() {
			Task<string> t = new Task<string>(() => tb.Text);
			t.Start(_sheduler);
			t.Wait();
			return t.Result;
		}

		private bool NeedsToRemoveItems(string searchString) {
			return !string.IsNullOrEmpty(searchString)
			       && searchString.Contains(_prevString);
		}

		#endregion

		#region Events

		public event EventHandler OnItemSelected;

		#endregion
	}


	/// <summary>
	/// UI Logic.
	/// </summary>
	public partial class ItemSelector {
		private void SearchTextChanged(object sender, TextChangedEventArgs e) {
			_needUpdate = true;
			e.Handled = true;
		}

		private void ItemsMouseDoubleClick(object sender, MouseButtonEventArgs e) {
			if (Items.SelectedItems.Count > 0) {
				ItemChoosen = true;
				Item = (Item) Items.SelectedItems[0];
				if (OnItemSelected != null)
					OnItemSelected(this, new EventArgs());
			}
		}

		private void BtCancelClick(object sender, RoutedEventArgs e) {
			ItemChoosen = false;
			if (OnItemSelected != null)
				OnItemSelected(this, null);
		}

		private void SearchTextKeyDown(object sender, KeyEventArgs e) {
			switch (e.Key) {
					// Select Item
				case Key.Enter:
					ItemSelectExecuted(null, null);
					e.Handled = true;
					break;
					// Close
				case Key.Escape:
					CloseExecuted(null, null);
					e.Handled = true;
					break;
					// Select Next Item:
				case Key.Down:
					SelectNextItemExecuted(null, null);
					e.Handled = true;
					break;
					// Select previous item:
				case Key.Up:
					SelectPrevItemExecuted(null, null);
					e.Handled = true;
					break;
				default:
					e.Handled = false;
					break;
			}
		}

		private void SelectNextItemExecuted(object sender, ExecutedRoutedEventArgs e) {
			if (Items.Items.Count > 0)
				Items.SelectedIndex = (Items.SelectedIndex + 1)%Items.Items.Count;
			Items.ScrollIntoView(Items.SelectedItem);
		}

		private void SelectPrevItemExecuted(object sender, ExecutedRoutedEventArgs e) {
			if (Items.Items.Count > 0)
				Items.SelectedIndex = (Items.SelectedIndex - 1)%Items.Items.Count;
			Items.ScrollIntoView(Items.SelectedItem);
		}

		private void CloseExecuted(object sender, ExecutedRoutedEventArgs e) {
			ItemChoosen = false;
			if (OnItemSelected != null)
				OnItemSelected(this, new EventArgs());
		}

		private void ItemSelectExecuted(object sender, ExecutedRoutedEventArgs e) {
			if (Items.SelectedItems.Count > 0) {
				ItemChoosen = true;
				Item = (Item) Items.SelectedItems[0];
				if (OnItemSelected != null)
					OnItemSelected(this, new EventArgs());
			}
		}
	}
}