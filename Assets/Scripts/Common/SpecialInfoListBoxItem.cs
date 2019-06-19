using System;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.Common.UI
{
    public class SpecialInfoListBoxItem : UIListBoxItem
    {
        public Text uitext;

        public void SetObject(int id, string type, Vector3 position, string msg)
        {
            ID = id;
            Type = type;
            Position = position;
            Text = String.Format("ID {0}. {1}.", id, type);
            Message = msg;
        }
        public int ID { get; private set; }
        public string Type { get; private set; }
        public Vector3 Position { get; private set; }

        public string Text
        {
            get { return uitext.text; }
            private set { uitext.text = value; }
        }

        public string Message { get; private set; }
    }

}

