using Elektronik.Common;
using Elektronik.Common.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;

namespace Elektronik.Online
{
    public class RecentIPListBoxItem : UIListBoxItem
    {
        public Text fullAddress;
        public Text dateTime;

        private string m_fullAddress = "";
        public string FullAddress
        {
            get { return m_fullAddress; }
            set
            {
                m_fullAddress = value;
                fullAddress.text = value;
                string[] tokens = value.Split(new[] { ':' });
                IP = tokens[0];
                Port = Int32.Parse(tokens[1]);
            }
        }
        public string IP { get; private set; }
        public int Port { get; private set; }
        public DateTime Time
        {
            get { return DateTime.Parse(dateTime.text); }
            set { dateTime.text = value.ToString("dd/MM/yyyy hh:mm:ss"); }
        }
    }
}
