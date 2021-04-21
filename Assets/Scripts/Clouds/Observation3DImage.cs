using System;
using System.Collections;
using System.IO;
using System.Linq;
using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Clouds
{
    public class Observation3DImage : MonoBehaviour
    {
        [Range(0, 10)] public float Scale = 0.5f;
        public MeshRenderer Renderer;

        private void Awake()
        {
            Renderer.material = new Material(Shader.Find("Standard"));
            Renderer.material.mainTexture = new Texture2D(100, 100);
        }

        private void OnEnable()
        {
            StartCoroutine(LoadData());
            Renderer.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            Renderer.gameObject.SetActive(false);
        }

        private IEnumerator LoadData()
        {
            while (true)
            {
                var data = GetComponent<DataComponent<SlamObservation>>();
                if (data == null || !File.Exists(data.Data.FileName)) continue;

                var texture = Renderer.sharedMaterial.mainTexture as Texture2D;
                texture!.LoadImage(File.ReadAllBytes(data.Data.FileName));
                var aspect = texture.width / (float)texture.height;
                Renderer.transform.localScale = new Vector3(Scale * aspect, 1, Scale);
                yield return new WaitForSeconds(1);
            }
        }
    }
}