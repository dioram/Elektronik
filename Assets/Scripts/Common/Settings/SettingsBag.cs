using System;
using System.Linq;
using System.Reflection;

namespace Elektronik.Common.Settings
{
    [Serializable]
    public class SettingsBag
    {
        public string ModificationTime;

        public virtual bool Validate()
        {
            return true;
        }

        public override string ToString()
        {
            return string.Join("\n", GetType()
                                       .GetFields(BindingFlags.Public | BindingFlags.Instance)
                                       .Where(f => Attribute.IsDefined(f, typeof(CheckForEqualsAttribute)))
                                       .Select(f => f.GetValue(this).ToString()));
        }

        protected bool Equals(SettingsBag other)
        {
            return GetType()
                    .GetFields(BindingFlags.Public | BindingFlags.Instance)
                    .Where(f => Attribute.IsDefined(f, typeof(CheckForEqualsAttribute)))
                    .All(f => f.GetValue(this).Equals(f.GetValue(other)));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SettingsBag) obj);
        }
    }
}