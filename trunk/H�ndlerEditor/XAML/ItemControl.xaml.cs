using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace HändlerEditor.XAML
{
	/// <summary>
	/// Interaction logic for ItemControl.xaml
	/// </summary>
	public partial class ItemControl
	{
		private static Popup _openSelector = null;

		public ItemControl()
		{
			InitializeComponent();
			Selector.OnItemSelected += Selector_OnItemSelected;
		}

		void Selector_OnItemSelected(object sender, System.EventArgs e)
		{
			if (Selector.ItemChoosen)
				Item = Selector.Item;

			Selector.StopUpdating();
			_selectorPopup.IsOpen = false;
		}

		public static DependencyProperty ItemProperty = DependencyProperty.Register("Item", typeof(Code.Item), typeof(ItemControl));
		public static DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(BitmapImage), typeof(ItemControl));

		public Code.Item Item
		{
			get
			{
				return (Code.Item)GetValue(ItemProperty);
			}
			set
			
			{
				Dispatcher.BeginInvoke(DispatcherPriority.Background,
																	  (SendOrPostCallback) delegate
																							   {
																								   SetValue(
																									   ItemProperty,
																									   value);
																							   }, value);
				Icon = value == null ? Code.IconBuffer.Icons["rngitem000"][63] : value.Icon;
			}
		}

		private BitmapImage Icon
		{
			get
			{
				return (BitmapImage)GetValue(IconProperty);
			}
			set
			{
										  Dispatcher.BeginInvoke(DispatcherPriority.Background,
																	  (SendOrPostCallback) delegate
																							   {
																								   SetValue(
																									   IconProperty,
																									   value
																									   );
																							   }, value);
			}
		}
		private void MenuClearClick(object sender, RoutedEventArgs e)
		{
			Item = null;
		}

		private void ImageMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			OpenSelector();
		}
		private void MenuChooseClick(object sender, RoutedEventArgs e) {
			OpenSelector();
		}

		private void OpenSelector() {

			CloseOpenedSelector();
			Selector.ItemChoosen = false;
			Selector.StartUpdating();
			_openSelector = _selectorPopup;
			_selectorPopup.IsOpen = true;
			Selector.tb.Focus();
		}
		private static void CloseOpenedSelector() {
			if(_openSelector != null)
			{
				var openSelectorItemChooser = _openSelector.Child as ItemSelector;
				if(openSelectorItemChooser != null)
				{
					openSelectorItemChooser.ItemChoosen = false;
					openSelectorItemChooser.StopUpdating();
				}
				_openSelector.IsOpen = false;
			}
		}
	}
}
