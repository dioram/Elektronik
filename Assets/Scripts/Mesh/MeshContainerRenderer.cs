using System.Collections;
using UnityEngine;

namespace Elektronik.Mesh
{
    [RequireComponent(typeof(MeshFilter))]
    public class MeshContainerRenderer : MonoBehaviour, IMeshRenderer
    {
        #region IMeshRenderer

        public void OnMeshUpdated(object sender, MeshUpdatedEventArgs e)
        {
            lock (_lock)
            {
                _vertices = e.Vertices;
                _normals = e.Normals;
                _triangles = e.Triangles;
                _meshUpdated = true;
            }
        }

        #endregion

        #region Unity events


        private void Start()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshFilter.sharedMesh = new UnityEngine.Mesh();
            StartCoroutine(UpdateMeshCoroutine());
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        #endregion

        #region Private
        
        private MeshFilter _meshFilter;
        private bool _meshUpdated;
        private Vector3[] _vertices;
        private Vector3[] _normals;
        private int[] _triangles;
        private object _lock = new object();

        private IEnumerator UpdateMeshCoroutine()
        {
            while (true)
            {
                if (_meshUpdated)
                {
                    _meshFilter.sharedMesh.Clear();
                    if (_vertices.Length != 0)
                    {
                        lock (_lock)
                        {
                            _meshFilter.sharedMesh.vertices = _vertices;
                            _meshFilter.sharedMesh.normals = _normals;
                            _meshFilter.sharedMesh.triangles = _triangles;
                        }
                        _meshFilter.sharedMesh.RecalculateNormals();
                        _meshFilter.sharedMesh.RecalculateTangents();
                        _meshFilter.sharedMesh.RecalculateBounds();
                        _meshFilter.sharedMesh.Optimize();
                        _meshFilter.sharedMesh.MarkModified(); 
                    }

                    _meshUpdated = false;
                }

                yield return new WaitForSeconds(2);
            }
        }
        
        #endregion
    }
}