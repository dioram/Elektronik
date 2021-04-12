using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI
{
    [RequireComponent(typeof(Text))]
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