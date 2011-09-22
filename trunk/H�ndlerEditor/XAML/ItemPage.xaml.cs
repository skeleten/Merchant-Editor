using System.Windows;

namespace HändlerEditor.XAML
{
    /// <summary>
    /// Interaction logic for ItemPage.xaml
    /// </summary>
    public partial class ItemPage
    {
        public ItemPage()
        {
            InitializeComponent();
        }

        private void BtAddClick(object sender, RoutedEventArgs e)
        {
            ItemRow r = new ItemRow();
            r.OnRemoveRequested += new ItemRow.OnRemoveRequestedDelegate(r_OnRemoveRequested);
            spItems.Children.Add(r);
        }

        internal void r_OnRemoveRequested(object sender)
        {
            spItems.Children.Remove((UIElement) sender);
        }

        private void BtRemClick(object sender, RoutedEventArgs e)
        {
            if(spItems.Children.Count > 0)
                spItems.Children.RemoveAt(spItems.Children.Count - 1);
        }
    }
}
