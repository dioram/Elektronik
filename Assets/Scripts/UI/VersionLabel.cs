using UnityEngine;

namespace Elektronik.UI
{
    /// <summary> Component for rendering version in UI. </summary>
    [ExecuteInEditMode]
    public class VersionLabel : MonoBehaviour
    {
        // TODO: rewrite this class to become more useful.
        public string VersionNumber = "";
        
        private void Awake()
        {
            VersionNumber = Application.version;
        }
    }
}