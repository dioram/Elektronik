using UnityEditor;
using UnityEditor.AddressableAssets.Settings;

namespace Elektronik.Editor
{
    public static class PlayerBuildScript
    {
        public static void BuildAddressables()
        {
            AddressableAssetSettings.BuildPlayerContent();
            BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, @".\Build\Elektronik.exe",
                                      EditorUserBuildSettings.activeBuildTarget, BuildOptions.None);
        }
    }
}