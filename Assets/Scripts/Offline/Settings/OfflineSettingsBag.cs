using System;
using System.Runtime.Serialization;
using Elektronik.Common.Settings;

namespace Elektronik.Offline.Settings
{
    [Serializable]
    public class OfflineSettingsBag : SettingsBag
    {
        private static class SettingName
        {
            public const string FilePath = "FilePath";
            public const string ImagePath = "ImagePath";
            public const string Scale = "Scale";
        }

        public static OfflineSettingsBag GetCurrent()
        {
            return (OfflineSettingsBag) Current;
        }
        
        public string FilePath
        {
            get => this[SettingName.FilePath].As<string>();
            set => this[SettingName.FilePath] = Setting.Create(SettingName.FilePath, value);
        }
        
        public string ImagePath
        {
            get => this[SettingName.ImagePath].As<string>();
            set => this[SettingName.ImagePath] = Setting.Create(SettingName.ImagePath, value);
        }
        
        public float Scale
        {
            get => this[SettingName.Scale].As<float>();
            set => this[SettingName.Scale] = Setting.Create(SettingName.Scale, value);
        }

        public OfflineSettingsBag() : base()
        {
            
        }
        
        public OfflineSettingsBag(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            
        }
        
        protected override bool Equals(SettingsBag other)
        {
            if (other.GetType() != this.GetType()) return false;
            var obj = (OfflineSettingsBag) other;
            if (!obj.ContainsKey(SettingName.FilePath) || !obj.ContainsKey(SettingName.ImagePath)) return false;
            return FilePath == obj.FilePath && ImagePath == obj.ImagePath;
        }
    }
}