using System;
using UnityEngine.UI;

namespace Elektronik.Common.UI
{
    public class UIRecentFileListBoxItem : UIListBoxItem
    {
        public Text path;
        public Text dateTime;

        public string Path
        {
            get => path.text;
            set => path.text = value;
        }

        public DateTime DateTime
        {
            get => DateTime.Parse(dateTime.text);
            set => dateTime.text = value.ToString("dd/MM/yyyy hh:mm:ss");
        }
    }
}