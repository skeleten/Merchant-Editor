using System;
using System.Windows.Media.Imaging;

namespace HändlerEditor.Code {
    [Serializable]
    public class Item {
        public uint Id { get; set; }
        public string InxName { get; set; }
        public string Name { get; set; }
        public string IconFile { get; set; }
        public uint IconIndex { get; set; }
        public uint DemandLevel { get; set; }

        public BitmapImage Icon {
            get { return IconBuffer.Icons.ContainsKey(IconFile) ? IconBuffer.Icons[IconFile][(int) IconIndex] : null; }
        }
    }
}