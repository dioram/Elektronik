using UnityEngine;
using SettingsBag = Elektronik.Settings.SettingsBag;

namespace Elektronik.PluginsSystem
{
    /// <summary> Base class for recorders that are writing to file. </summary>
    public abstract class FileRecorderPluginBase : DataRecorderPluginBase
    {
        /// <summary> Constructor. </summary>
        /// <param name="filename"> Path to file. </param>
        public FileRecorderPluginBase(string filename)
        {
            Filename = filename;
        }

        #region DataRecorder

        /// <inheritdoc />
        public override string DisplayName => "";

        /// <inheritdoc />
        public override SettingsBag Settings => null;

        /// <inheritdoc />
        public override Texture2D Logo => null;

        #endregion

        #region Protected

        /// <summary> Path to file. </summary>
        protected readonly string Filename;

        #endregion
    }
}