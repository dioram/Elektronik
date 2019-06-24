using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Elektronik.Common
{
    [Serializable]
    public abstract class Setting : IComparable<Setting>
    {
        public string Name { get; }

        private Type Type { get; }

        [NonSerialized]
        bool m_isModified;
        public bool IsModified
        {
            get => m_isModified;
            protected set => m_isModified = value;
        }

        public Guid UniqueId { get; }
        public DateTime ModificationTime { get; protected set; }
        public Setting<T> As<T>(bool throwException = true)
        {
            if (throwException && !(this is Setting<T>))
            {
                throw new InvalidCastException($"Cannot cast Setting<{Type.Name}> to Setting<{nameof(T)}>");
            }
            return this as Setting<T>;
        }
        public int CompareTo(Setting other) => UniqueId.CompareTo(other);
        public Setting(string name, Type type)
        {
            IsModified = false;
            Name = name;
            Type = type;
            UniqueId = Guid.NewGuid();
            ModificationTime = DateTime.Now;
        }
    }

    [Serializable]
    public class Setting<T> : Setting
    {
        private T m_value;
        public T Value
        {
            get => m_value;
            set
            {
                m_value = value;
                IsModified = true;
                ModificationTime = DateTime.Now;
            }
        }
        public Setting(string name, T value) : base(name, typeof(T)) => Value = value;
        public static implicit operator T(Setting<T> setting) => setting.Value;
        public override string ToString() => Value.ToString();
    }
}