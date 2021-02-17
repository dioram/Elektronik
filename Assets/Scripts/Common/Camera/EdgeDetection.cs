using UnityEngine;

namespace Elektronik.Common.Cameras
{
    public class EdgeDetection : MonoBehaviour
    {
        [SerializeField]
        private Material postprocessMaterial;

        private Camera _cam;

        private void Start(){
            _cam = GetComponent<Camera>();
            _cam.depthTextureMode = _cam.depthTextureMode | DepthTextureMode.DepthNormals;
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination){
            Graphics.Blit(source, destination, postprocessMaterial);
        }
    }
}