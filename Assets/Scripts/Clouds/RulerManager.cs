using System.Collections;
using UnityEngine;

namespace Elektronik.Clouds
{
    public class RulerManager : MonoBehaviour
    {
        public GameObject LineRendererPrefab;
        public float MaxDistance = 100f;
        [SerializeField] private PointCloudRenderer PointCloudRenderer;
        public Transform Cursor;

        private Camera _camera;
        private Ruler _currentRuler = null;
        
        #region Unity events
        
        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            if (Cursor.gameObject.activeSelf && Input.GetMouseButtonUp(0))
            {
                if (_currentRuler is null)
                {
                    _currentRuler = Instantiate(LineRendererPrefab, transform).GetComponent<Ruler>();
                    _currentRuler.FirstPoint = Cursor.position;
                }
                else
                {
                    _currentRuler.SecondPoint = Cursor.position;
                    _currentRuler = null;
                }
            }
        }

        private void OnEnable()
        {
            StartCoroutine(FindHoveredPoint());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            Cursor.gameObject.SetActive(false);
        }

        #endregion

        #region Private

        
        
        private bool RaySphereIntersection(Ray ray, Vector3 sphereOrigin, float radius)
        {
            var k = ray.origin - sphereOrigin;
            var b = Vector3.Dot(k, ray.direction);
            var c = Vector3.Dot(k, k) - radius * radius;
            var d = b * b - c;

            if (d < 0) return false;
            var sqrtD = Mathf.Sqrt(d);

            var t1 = -b + sqrtD;
            var t2 = -b - sqrtD;
            var minT = Mathf.Min(t1, t2);
            var maxT = Mathf.Max(t1, t2);
            var t = (minT >= 0) ? minT : maxT;

            return t > 0;
        }

        private IEnumerator FindHoveredPoint()
        {
            while (true)
            {
                var ray = _camera.ScreenPointToRay(Input.mousePosition);
                var radius = PointCloudRenderer.ItemSize;
                var point = Vector3.positiveInfinity;
                var cameraPos = _camera.transform.position;

                foreach (var item in PointCloudRenderer.GetPoints())
                {
                    if (!RaySphereIntersection(ray, item, radius)) continue;
                    if ((cameraPos - item).magnitude < (cameraPos - point).magnitude)
                    {
                        point = item;
                    }
                }

                var found = (cameraPos - point).magnitude < MaxDistance;
                Cursor.gameObject.SetActive(found);

                if (found)
                {
                    Cursor.position = point;
                    Cursor.localScale = Vector3.one * (radius * 2.2f);
                }
                
                yield return new WaitForSeconds(0.1f);
            }
            // ReSharper disable once IteratorNeverReturns
        }

        #endregion
    }
}