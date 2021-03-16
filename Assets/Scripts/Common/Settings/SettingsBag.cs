﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace Elektronik.Common.Settings
{
    [Serializable]
    public abstract class SettingsBag : IComparable<SettingsBag>, ISerializable
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
            ModificationTime = DateTime.Now;
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

        public bool ContainsKey(string key) => _settings.ContainsKey(key);

        public Setting this[string name]
        {
            get => _settings[name];
            protected set => _settings[name] = value;
        }

        public bool TryGetValue(string name, out Setting setting)
        {
            return _settings.TryGetValue(name, out setting);
        }

        protected abstract bool Equals(SettingsBag other);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SettingsBag) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (_settings != null ? _settings.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ UniqueId.GetHashCode();
                hashCode = (hashCode * 397) ^ ModificationTime.GetHashCode();
                return hashCode;
            }
        }
    }
}