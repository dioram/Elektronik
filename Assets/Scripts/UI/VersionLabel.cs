using UnityEngine;

namespace Elektronik.UI
{
    [ExecuteInEditMode]
    public class VersionLabel : MonoBehaviour
    {
        public string VersionNumber = "";
        
        private void Awake()
        {
            VersionNumber = Application.version;
        }
    }
}