using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Elektronik.Common.Settings
{
    [Serializable]
    public class SettingsBag : ISerializationCallbackReceiver
    {
        public DateTime ModificationTime;

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