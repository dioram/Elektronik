using Elektronik.Common.Clouds;
using UnityEngine;

namespace DefaultNamespace
{
    public class PlaneTester : MonoBehaviour
    {
        public int idx;
        public Vector3 offset;
        public Vector3 normal = Vector3.up;
        public Color color = Color.red;
        public FastPlaneCloud cloud;   
        
        public void SetPoint()
        {
            cloud.Set(idx, offset, normal);
        }
        
        public void SetColor()
        {
            cloud.Set(idx, color);
        }
    }
}