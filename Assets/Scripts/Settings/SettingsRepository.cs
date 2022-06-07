using UnityEngine;

namespace Elektronik.Settings
{
    /// <summary> This class provides solution-wide path to directory where settings are saved. </summary>
    /// <remarks>
    /// We can't just use <see cref="Application.persistentDataPath"/>
    /// because it raises exceptions in plugins test runtime.
    /// </remarks>
    public class SettingsRepository : MonoBehaviour
    {
        /// <summary> Path to directory where settings should be saved. </summary>
        public static string Path { get; private set; } = "./";

        private void Awake()
        {
            Path = Application.persistentDataPath;
        }
    }
}