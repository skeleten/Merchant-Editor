using System;
using System.Windows;
using System.Windows.Controls;

namespace HändlerEditor.XAML
{
    /// <summary>
    /// Interaction logic for ItemRow.xaml
    /// </summary>
    public partial class ItemRow
    {
        public ItemRow()
        {
            InitializeComponent();

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

        private void BtRemoveClick(object sender, System.Windows.RoutedEventArgs e)
        {
            if (OnRemoveRequested != null)
                OnRemoveRequested(this);
        }

        public event OnRemoveRequestedDelegate OnRemoveRequested;
        public delegate void OnRemoveRequestedDelegate(object sender);

        private void UserControlMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btRemove.Visibility = Visibility.Visible;
            //this.Width += 38;
        }

        private void UserControlMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //this.Width -= 38;
            btRemove.Visibility = Visibility.Collapsed;
        }
    }
}
