using System;
using System.Linq;
using System.Reflection;
using Elektronik.UI.SettingsFields;

namespace Elektronik.Settings
{
    /// <summary> Class for storing settings. </summary>
    /// <remarks>
    /// You can inherit from this class for adding new fields.
    /// This class can be saved to file using ISettingsHistory,
    /// or rendered on UI using descendant of <see cref="Elektronik.UI.SettingsFields.SettingsFieldBase"/>.
    /// You can find list of renderable field types in <see cref="SettingsFieldsUiGenerator"/>.
    /// </remarks>
    [Serializable]
    public partial class SettingsBag
    {
        /// <summary> Time when this object was created. </summary>
        [Hide]
        public string ModificationTime;

        /// <summary> Validates content of settings. </summary>
        /// <remarks> Successful by default, override it if you need your own logic. </remarks>
        public virtual ValidationResult Validate()
        {
            return ValidationResult.Succeeded;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Join("\n", GetType()
                                       .GetFields(BindingFlags.Public | BindingFlags.Instance)
                                       .Where(f => Attribute.IsDefined(f, typeof(CheckForEqualsAttribute)))
                                       .Select(f => f.GetValue(this)?.ToString() ?? ""));
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SettingsBag) obj);
        }

        protected bool Equals(SettingsBag other)
        {
            return GetType()
                    .GetFields(BindingFlags.Public | BindingFlags.Instance)
                    .Where(f => Attribute.IsDefined(f, typeof(CheckForEqualsAttribute)))
                    .All(f => f.GetValue(this)?.Equals(f.GetValue(other)) ?? (f.GetValue(other) == null));
        }
    }
    
    public static class SettingsBagExt
    {
        /// <summary> Creates shallow copy of this object. </summary>
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