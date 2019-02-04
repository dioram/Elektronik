using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.Common.UI
{
    [RequireComponent(typeof(Button))]
    public class SpecialInfoListBoxItem : UIListBoxItem
    {
        private Button m_button;

        void Awake()
        {
            m_button = GetComponent<Button>();
        }

        SlamPoint? m_point;
        public SlamPoint? Point
        {
            get
            {
                return m_point;
            }
            set
            {
                m_point = value;
                m_obs = null;
                Text = string.Format("Id {0}. Point", m_point.Value.id);
            }
        }

        SlamObservation m_obs;
        public SlamObservation Observation
        {
            get
            {
                return m_obs;
            }
            set
            {
                m_obs = value;
                m_point = null;
                Text = string.Format("Id {0}. Observation", m_obs.id);
            }
        }
        public string Text { get; private set; }
    }

}

