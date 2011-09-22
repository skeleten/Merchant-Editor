using System.Windows;
using HändlerEditor.Code;

namespace HändlerEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() : base()
        {
             Settings.Load();
             IconBuffer.Initialize(Settings.IconPath);
             DataProvider.Initialize();
        }
    }
}
