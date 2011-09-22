using System.Xml;

namespace HändlerEditor.Code
{
    public static class Settings
    {
        public static void Load()
        {
            var doc = new XmlDocument();
            doc.Load("Settings.xml");
            foreach (XmlNode n in doc.ChildNodes)
                ParseXmlNode(n);
        }
        private static void ParseXmlNode(XmlNode n)
        {
            switch (n.Name)
            {
                    // rekrusive search for the root node
                case "Settings":
                    foreach (XmlNode node in n.ChildNodes)
                        ParseXmlNode(node);
                    break;
                    // path to the icon files
                case "IconPath":
                    IconPath = n.InnerText;
                    break;
                    // path to the shn files
                case "DataPath":
                    DataPath = n.InnerText;
                    break;
                    // interval to update the search in ItemSelector
                case "UpdateInterval":
                    UpdateInterval = int.Parse(n.InnerText);
                    break;
                default:
                    break;
            }
        }

        public static string IconPath { get; private set; }
        public static string DataPath { get; private set; }
        public static int UpdateInterval { get; private set; }
    }
}
