using UnityEngine;

namespace Elektronik.Common.Cameras
{
    public class EdgeDetection : MonoBehaviour
    {
        [SerializeField]
        private Material postprocessMaterial;

        private Camera cam;

        private void Start(){
            cam = GetComponent<Camera>();
            cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.DepthNormals;
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination){
            Graphics.Blit(source, destination, postprocessMaterial);
        }
    }
}