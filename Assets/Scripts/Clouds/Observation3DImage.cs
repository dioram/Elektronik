using System;
using System.IO;
using System.Linq;
using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Clouds
{
    public class Observation3DImage : MonoBehaviour
    {
        [Range(0, 1)] public float Alpha = 0.5f;
        public MeshRenderer Renderer;

        private DataComponent<SlamObservation> _data;

        private void Start()
        {
            _data = GetComponent<DataComponent<SlamObservation>>();
        }

        private void OnEnable()
        {
            var texture = new Texture2D(100, 100);
            if (_data == null && !File.Exists(_data.Data.FileName)) return;
            
            texture.LoadImage(File.ReadAllBytes(_data.Data.FileName));
            texture.SetPixels(texture.GetPixels().Select(c =>
            {
                c.a = Alpha;
                return c;
            }).ToArray());

            Renderer.material.mainTexture = texture;
            Renderer.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            Renderer.gameObject.SetActive(false);
        }
    }
}