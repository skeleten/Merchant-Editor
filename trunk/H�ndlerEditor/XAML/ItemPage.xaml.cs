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

        public void AddRow(object sender, RoutedEventArgs e)
        {
            var r = new ItemRow();
            r.OnRemoveRequested += r_OnRemoveRequested;
            spItems.Children.Add(r);
        }

        internal void r_OnRemoveRequested(object sender)
        {
            spItems.Children.Remove((UIElement) sender);
        }
    }
}
