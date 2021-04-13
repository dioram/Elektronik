using UnityEditor.AddressableAssets.Settings;

namespace Elektronik.Editor
{
    public static class PlayerBuildScript
    {
        public static void BuildAddressables()
        {
            AddressableAssetSettings.BuildPlayerContent();
        }
    }
}