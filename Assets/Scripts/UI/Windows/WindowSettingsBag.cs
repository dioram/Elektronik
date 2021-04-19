using Elektronik.Settings;
using Elektronik.Settings.Bags;

namespace Elektronik.UI.Windows
{
    public class WindowSettingsBag : SettingsBag
    {
        [CheckForEquals] public float X;
        [CheckForEquals] public float Y;
        [CheckForEquals] public float Height;
        [CheckForEquals] public float Width;
        [CheckForEquals] public bool IsShowing;
        [CheckForEquals] public bool IsMaximized;
    }
}