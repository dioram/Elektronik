using System.Runtime.Serialization;
using Elektronik.Common.Settings;

namespace Elektronik.Online.Settings
{
    public class OnlineSettingsBag : SettingsBag
    {
        private static class SettingName
        {
            public const string IPAddress = "IPAddress";
            public const string Port = "Port";
            public const string Scale = "Scale";
        }

        public static OnlineSettingsBag GetCurrent()
        {
            return (OnlineSettingsBag) Current;
        }
        
        public string IPAddress
        {
            get => this[SettingName.IPAddress].As<string>();
            set => this[SettingName.IPAddress] = Setting.Create(SettingName.IPAddress, value);
        }
        
        public int Port
        {
            get => this[SettingName.Port].As<int>();
            set => this[SettingName.Port] = Setting.Create(SettingName.Port, value);
        }
        
        public float Scale
        {
            get => this[SettingName.Scale].As<float>();
            set => this[SettingName.Scale] = Setting.Create(SettingName.Scale, value);
        }

        public OnlineSettingsBag() : base()
        {
            
        }
        
        public OnlineSettingsBag(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            
        }
        
        protected override bool Equals(SettingsBag other)
        {
            if (other.GetType() != this.GetType()) return false;
            var obj = (OnlineSettingsBag) other;
            if (!obj.ContainsKey(SettingName.Port) || !obj.ContainsKey(SettingName.Port)) return false;
            return IPAddress == obj.IPAddress && Port == obj.Port;
        }
    }
}