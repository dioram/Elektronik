using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Elektronik.Common.Settings
{
    [Serializable]
    public class SettingsBag : IComparable<SettingsBag>, ISerializable
    {
        public static Mode Mode { get; set; }
        public static SettingsBag Current { get; set; }

        private Dictionary<string, Setting> m_settings;
        public Guid UniqueId { get; private set; }
        public DateTime ModificationTime { get; private set; }
        public SettingsBag()
        {
            UniqueId = Guid.NewGuid();
            m_settings = new Dictionary<string, Setting>();
        }
        public void Change<T>(string name, T value)
        {
            this[name] = Setting.Create(name, value);
            ModificationTime = DateTime.Now;
        }

        protected SettingsBag(SerializationInfo info, StreamingContext context)
        {
            ModificationTime = DateTime.Parse(info.GetString("ModificationTime"));
            UniqueId = (Guid)info.GetValue("UniqueId", typeof(Guid));
            m_settings = (Dictionary<string, Setting>)info.GetValue("Settings", typeof(Dictionary<string, Setting>));
        }

        public int CompareTo(SettingsBag other) => UniqueId.CompareTo(other.UniqueId);

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ModificationTime", ModificationTime.ToString());
            info.AddValue("UniqueId", UniqueId);
            info.AddValue("Settings", m_settings);
        }

        public Setting this[string name]
        {
            get => m_settings[name];
            private set => m_settings[name] = value;
        }

        public bool TryGetValue(string name, out Setting setting)
        {
            return m_settings.TryGetValue(name, out setting);
        }
    }
}