using UnityEditor;
using UnityEditor.AddressableAssets.Settings;

namespace Elektronik
{
    public static class BuildScript
    {
        public static void BuildAddressables()
        {
            AddressableAssetSettings.BuildPlayerContent();
            BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, @".\Build\Elektronik.exe",
                                      EditorUserBuildSettings.activeBuildTarget, BuildOptions.None);
        }
    }
}