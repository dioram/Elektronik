using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Elektronik.Common
{
    [Serializable]
    public class SettingsBag : Dictionary<string, Setting>, IComparable<SettingsBag>
    {
        public static Mode Mode { get; set; }
        public static SettingsBag Current { get; set; }
        public Guid UniqueId { get; }
        public DateTime ModificationTime { get; private set; }
        public SettingsBag() => UniqueId = Guid.NewGuid();
        public void Change<T>(string name, T value)
        {
            if (TryGetValue(name, out Setting setting))
            {
                setting.As<T>().Value = value;
            }
            else
            {
                this[name] = new Setting<T>(name, value);
            }
            ModificationTime = DateTime.Now;
        }

        protected SettingsBag(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ModificationTime = (DateTime)info.GetValue("ModificationTime", typeof(DateTime));
            UniqueId = (Guid)info.GetValue("UniqueId", typeof(Guid));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ModificationTime", ModificationTime);
            info.AddValue("UniqueId", UniqueId);
        }

        public int CompareTo(SettingsBag other) => UniqueId.CompareTo(other.UniqueId);
    }
}