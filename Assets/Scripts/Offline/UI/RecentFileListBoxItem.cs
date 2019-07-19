using Elektronik.Common.UI;
using System;
using UnityEngine.UI;

namespace Elektronik.Offline.UI
{
    public class RecentFileListBoxItem : UIListBoxItem
    {
        public Text path;
        public Text dateTime;

        public string Path
        {
            get { return path.text; }
            set { path.text = value; }
        }
        public DateTime DateTime
        {
            get { return DateTime.Parse(dateTime.text); }
            set { dateTime.text = value.ToString("dd/MM/yyyy hh:mm:ss"); }
        }


    }
}