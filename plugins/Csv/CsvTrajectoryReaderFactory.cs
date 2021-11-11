using Elektronik.PluginsSystem;

namespace Csv
{
    public class CsvTrajectoryReaderFactory : ElektronikPluginsFactoryBase<CsvFileSettingsBag>,
                                              ISnapshotReaderPluginsFactory
    {
        protected override IElektronikPlugin StartPlugin(CsvFileSettingsBag settings)
        {
            
            if (!string.IsNullOrEmpty(_filePath))
            {
                settings.PathsToFiles = _filePath!;
            }

            return new CsvTrajectoryReader(settings);
        }

        public override string DisplayName => "CSV";
        public override string Description => "Plugin for reading trajectories from CSV files.";

        public void SetFileName(string path)
        {
            _filePath = path;
        }

        public string[] SupportedExtensions => new[] { ".csv" };

        private string? _filePath;
    }
}