#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Elektronik.Common
{
    public class VersionAutoIncrementer : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            string currentVersion = PlayerSettings.bundleVersion;

            int major = int.Parse(currentVersion.Split('.')[0]);
            int minor = int.Parse(currentVersion.Split('.')[1]);
            int build = int.Parse(currentVersion.Split('.')[2]) + 1;
            
            PlayerSettings.bundleVersion = major + "." + minor + "." + build;
        }
    }
}
#endif