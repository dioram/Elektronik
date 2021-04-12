using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build.Pipeline.Utilities;

public static class AddressableBuildScript
{
    static void BuildPlayer()
    {
        AddressableAssetSettings.CleanPlayerContent();
        BuildCache.PurgeCache(false);
        AddressableAssetSettings.BuildPlayerContent();
        BuildPipeline.BuildPlayer(new BuildPlayerOptions());
    }
}