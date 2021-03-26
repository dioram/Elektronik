using Elektronik.Containers;
using Elektronik.Data.Converters;
using Elektronik.Settings;

namespace Elektronik.PluginsSystem
{
    public abstract class DataSourcePluginBase<TSettings> : IDataSourcePlugin
            where TSettings : SettingsBag, new()
    {
        // ReSharper disable once NotNullMemberIsNotInitialized
        public DataSourcePluginBase()
        {
            var sh = new SettingsHistory<TSettings>($"{GetType().FullName}.json");
            SettingsHistory = sh;
            if (sh.Recent.Count > 0) TypedSettings = (TSettings) sh.Recent[0].Clone();
            else TypedSettings = new TSettings();
        }
        
        #region IDataSource implementation

        public abstract string DisplayName { get; }
        public abstract string Description { get; }

        public SettingsBag Settings
        {
            get => TypedSettings;
            set => TypedSettings = (TSettings) value;
        }
        
        public ISettingsHistory SettingsHistory { get; }

        public abstract void Start();

        public abstract void Stop();

        public abstract void Update(float delta);

        public ICSConverter Converter { get; set; }
        public ISourceTree Data { get; protected set; }

        #endregion

        #region Protected definitions

        protected TSettings TypedSettings;

        #endregion

    }
}