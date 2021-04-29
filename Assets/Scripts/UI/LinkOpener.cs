using UnityEngine;

namespace Elektronik.UI
{
    public class LinkOpener : MonoBehaviour
    {
        [SerializeField] private string GithubLink = "https://github.com/dioram/Elektronik-Tools-2.0";
        
        public void GoToGithub()
        {
            Application.OpenURL(GithubLink);
        }
    }
}