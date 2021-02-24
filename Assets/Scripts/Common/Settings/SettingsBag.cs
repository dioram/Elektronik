using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Elektronik.Common.Settings
{
    [Serializable]
    public class SettingsBag : ISerializationCallbackReceiver
    {
        public DateTime ModificationTime;

        public static void CreateCurrent(Type settingsType)
        {
            RemoveCurrent(settingsType);
            Currents.Add((SettingsBag) Activator.CreateInstance(settingsType));
        }

        public static SettingsBag GetCurrent(Type settingsType)
        {
            var t = Currents.Where(s => s.GetType() == settingsType).ToList();
            if (t.Any())
            {
                return t.First();
            }

            var tmp = (SettingsBag) Activator.CreateInstance(settingsType);
            Currents.Add(tmp);
            return tmp;
        }
        
        public static T GetCurrent<T>() where T: SettingsBag, new()
        {
            var t = Currents.OfType<T>().ToList();
            if (t.Any())
            {
                return t.First();
            }

            var tmp = new T();
            Currents.Add(tmp);
            return tmp;
        }

        public static void SetCurrent<T>(T newSettings) where T : SettingsBag
        {
            RemoveCurrent(typeof(T));
            Currents.Add(newSettings);
        }

        public static void RemoveCurrent(Type settingsType)
        {
            var t = Currents.Where(s => s.GetType() == settingsType);
            Currents.RemoveAll(s => t.Contains(s));
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
                   .Select(f => f.GetValue(this).Equals(f.GetValue(other)))
                   .All(b => b);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SettingsBag) obj);
        }

        public void OnBeforeSerialize()
        {
            ModificationTime = DateTime.Now;
        }

        public void OnAfterDeserialize()
        {
            // Do nothing
        }

        private static readonly List<SettingsBag> Currents = new List<SettingsBag>();
    }
}