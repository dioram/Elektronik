using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

namespace Elektronik.Editor
{
    public static class PlayerBuildScript
    {
        public static void BuildAddressables()
        {
            var s = AddressableAssetSettingsDefaultObject.Settings;
            s.buildSettings.bundleBuildPath = "./AddressableAssetsData";
            AddressableAssetSettings.BuildPlayerContent();
        }
    }
}