using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using Microsoft.Win32;

namespace ContextMenuSetter
{
    static class Program
    {
        [SupportedOSPlatform("windows")]
        static void Main(string[] args)
        {
            if (args.Length < 2 || !File.Exists(args[0])) return;
            var elektronikExe = args[0];
            var extensions = args.Skip(1);
            var classes = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Classes", true)!;
            foreach (var extension in extensions)
            {
                using var e = classes.CreateSubKey(extension);
                using var shell = e.CreateSubKey("shell");
                CreateRegistryEntry(shell, elektronikExe);
            }
        }

        [SupportedOSPlatform("windows")]
        static void CreateRegistryEntry(RegistryKey shell, string elektronikExe)
        {
            using var elektronikKey = shell.CreateSubKey("Open in Elektronik");
            elektronikKey.SetValue("icon", elektronikExe);
            using var command = elektronikKey.CreateSubKey("command");
            command.SetValue("", $"{elektronikExe} %1");
        }
    }
}