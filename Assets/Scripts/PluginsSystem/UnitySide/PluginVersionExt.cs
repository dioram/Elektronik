namespace Elektronik.PluginsSystem.UnitySide
{
    public static class PluginVersionExt
    {
        public static string GetVersion(this IElektronikPlugin plugin)
        {
            var version = plugin.GetType().Assembly.GetName().Version; 
            return $"{version.Major}.{version.Minor}";
        }
    }
}