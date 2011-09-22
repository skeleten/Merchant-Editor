using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Threading.Tasks;

namespace HändlerEditor.XAML
{
    /// <summary>
    /// Interaction logic for ItemControl.xaml
    /// </summary>
    public partial class ItemControl
    {
        private TaskScheduler _sheduler;
        public ItemControl()
        {
            InitializeComponent();
            Selector.OnItemSelected += new System.EventHandler(Selector_OnItemSelected);
            _sheduler = TaskScheduler.FromCurrentSynchronizationContext();
        }

        void Selector_OnItemSelected(object sender, System.EventArgs e)
        {
            if(Selector.ItemChoosen)
            {
                this.Item = Selector.Item;
            }

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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                                                      (SendOrPostCallback) delegate
                                                                                               {
                                                                                                   SetValue(
                                                                                                       ItemProperty,
                                                                                                       value);
                                                                                               }, value);
                Icon = value == null ? Code.IconBuffer.Icons["haircolor"][63] : value.Icon;
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
                                          this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
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
        private void MenuChooseClick(object sender, RoutedEventArgs e)
        {
            Selector.ItemChoosen = false;
            Selector.StartUpdating();
            //Selector.Start();
            _selectorPopup.IsOpen = true;
            Selector.tb.Focus();
        }

        private void Image_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MenuChooseClick(null, null);
        }
    }
}
