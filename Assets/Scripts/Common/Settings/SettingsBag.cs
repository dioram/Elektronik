using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace Elektronik.Common.Settings
{
    [Serializable]
    public class SettingsBag : IComparable<SettingsBag>, ISerializable
    {
        public static Mode Mode { get; set; }
        public static SettingsBag Current { get; set; }

        private Dictionary<string, Setting> _settings;
        public Guid UniqueId { get; private set; }
        public DateTime ModificationTime { get; private set; }
        public SettingsBag()
        {
            UniqueId = Guid.NewGuid();
            _settings = new Dictionary<string, Setting>();
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
            _settings = (Dictionary<string, Setting>)info.GetValue("Settings", typeof(Dictionary<string, Setting>));
        }

        public int CompareTo(SettingsBag other) => UniqueId.CompareTo(other.UniqueId);

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ModificationTime", ModificationTime.ToString(CultureInfo.CurrentCulture));
            info.AddValue("UniqueId", UniqueId);
            info.AddValue("Settings", _settings);
        }

        public Setting this[string name]
        {
            get => _settings[name];
            private set => _settings[name] = value;
        }

        public bool TryGetValue(string name, out Setting setting)
        {
            return _settings.TryGetValue(name, out setting);
        }
    }
}