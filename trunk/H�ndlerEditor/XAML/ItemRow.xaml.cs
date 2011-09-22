using System;
using System.Windows;
using System.Threading.Tasks;
using System.Threading;

namespace HändlerEditor.XAML
{
    /// <summary>
    /// Interaction logic for ItemRow.xaml
    /// </summary>
    public partial class ItemRow
    {
        private readonly TaskScheduler _sheduler;
        public ItemRow()
        {
            InitializeComponent();
            _sheduler = TaskScheduler.FromCurrentSynchronizationContext();

            for (int i = 0; i < 6; i++)
                this[i] = null;
        }

        public Code.Item this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return item0.Item;
                    case 1:
                        return item1.Item;
                    case 2:
                        return item2.Item;
                    case 3:
                        return item3.Item;
                    case 4:
                        return item4.Item;
                    case 5:
                        return item5.Item;
                    default:
                        throw new ArgumentException("index");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        item0.Item = value;
                        break;
                    case 1:
                        item1.Item = value;
                        break;
                    case 2:
                        item2.Item = value;
                        break;
                    case 3:
                        item3.Item = value;
                        break;
                    case 4:
                        item4.Item = value;
                        break;
                    case 5:
                        item5.Item = value;
                        break;
                    default:
                        throw new ArgumentException("index");
                }
            }
        }

        private void BtRemoveClick(object sender, RoutedEventArgs e)
        {
            // Request remove.
            // This event is usally caught by a ItemPage.
            if (OnRemoveRequested != null)
                OnRemoveRequested(this);
        }

        public event OnRemoveRequestedDelegate OnRemoveRequested;
        public delegate void OnRemoveRequestedDelegate(object sender);

        private void UserControlMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // As the event is called by the MainThread, here is no invocation or TaskSheduler needed.
            btRemove.Visibility = Visibility.Visible;
        }

        private void UserControlMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // Hide the remove-button after 250ms
            var startHide = new Task(() => Thread.Sleep(250));
            startHide.ContinueWith(task => btRemove.Visibility = Visibility.Collapsed, _sheduler);
            startHide.Start();
        }
    }
}
