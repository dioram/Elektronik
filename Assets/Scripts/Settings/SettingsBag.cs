using System;
using System.Linq;
using System.Reflection;

namespace Elektronik.Settings
{
    [Serializable]
    public class SettingsBag
    {
        [NotShow]
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
                                       .Select(f => f.GetValue(this)?.ToString() ?? ""));
        }

        protected bool Equals(SettingsBag other)
        {
            return GetType()
                    .GetFields(BindingFlags.Public | BindingFlags.Instance)
                    .Where(f => Attribute.IsDefined(f, typeof(CheckForEqualsAttribute)))
                    .All(f => f.GetValue(this)?.Equals(f.GetValue(other)) ?? (f.GetValue(other) == null ? true : false));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SettingsBag) obj);
        }
    }
    
    public static class SettingsBagExt
    {
        public static T Clone<T>(this T source) where T : SettingsBag
        {
            T result = (T)Activator.CreateInstance(source.GetType());
            var fields = result.GetType()
                    .GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo f in fields)
            {
                f.SetValue(result, f.GetValue(source));
            }
            return result;
        }
    }
}