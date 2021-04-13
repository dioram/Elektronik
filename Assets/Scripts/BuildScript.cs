using UnityEditor;
using UnityEditor.AddressableAssets.Settings;

namespace Elektronik
{
    public class BuildScript
    {
        public void BuildAddressables()
        {
            AddressableAssetSettings.BuildPlayerContent();
            BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, @".\Build\Elektronik.exe",
                                      EditorUserBuildSettings.activeBuildTarget, BuildOptions.None);
        }
    }
}