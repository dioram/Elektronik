using System.Collections.Generic;
using System.Linq;
using Elektronik.Data.PackageObjects;
using GK;
using UnityEngine;

namespace Elektronik.Clouds
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class ConvexMesh : MonoBehaviour
    {
        public Material MeshMaterial;
        [Range(0, 1)] public float Alpha = 0.2f;

        private MeshRenderer _renderer;
        private MeshFilter _meshFilter;
        
        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _renderer = GetComponent<MeshRenderer>();
        }

        public void CreateHull(IEnumerable<SlamPoint> slamPoints)
        {
            var list = slamPoints.ToList();
            var positions = new List<Vector3>(list.Count);
            float r = 0, g = 0, b = 0;
            foreach (var point in list)
            {
                positions.Add(point.Position);
                r += point.Color.r;
                g += point.Color.g;
                b += point.Color.b;
            }

            r /= list.Count;
            g /= list.Count;
            b /= list.Count;
            var mat = new Material(MeshMaterial) {color = new Color(r, g, b, Alpha)};
            _renderer.material = mat;

            var chc = new ConvexHullCalculator();
            var vertices = new List<Vector3>();
            var tris = new List<int>();
            var normals = new List<Vector3>();
            chc.GenerateHull(positions, false, ref vertices, ref tris, ref normals);
            var mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetTriangles(tris, 0);
            mesh.SetNormals(normals);
            mesh.Optimize();
            _meshFilter.mesh = mesh;
        }
    }
}