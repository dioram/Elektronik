#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor.Build.Reporting;

namespace Elektronik
{
    public class VersionAutoIncrementer : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            AddressableAssetSettings.CleanPlayerContent();
            BuildCache.PurgeCache(false);
            AddressableAssetSettings.BuildPlayerContent();
            
            string currentVersion = PlayerSettings.bundleVersion;

            int major = int.Parse(currentVersion.Split('.')[0]);
            int minor = int.Parse(currentVersion.Split('.')[1]);
            int patch = int.Parse(currentVersion.Split('.')[2]);
            int build = int.Parse(currentVersion.Split('.')[3]) + 1;
            
            PlayerSettings.bundleVersion = $"{major}.{minor}.{patch}.{build}";
        }
    }
}
#endif