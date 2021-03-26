using System;
using System.Linq;
using UnityEngine;

namespace Elektronik.UI.Windows
{
    public class WindowsFactory : MonoBehaviour
    {
        public RectTransform Canvas;
        public GameObject[] RendererWindowsPrefabs;

        public void GetNewDataRenderer<T>(string title, Action<T, Window> callback)
        {
            MainThreadInvoker.Instance.Enqueue(() =>
            {
                var prefab = RendererWindowsPrefabs.First(g => g.GetComponent<T>() != null);
                var go = Instantiate(prefab, Canvas);
                var window = go.GetComponent<Window>();
                window.Title = title;
                go.SetActive(false);
                callback(go.GetComponent<T>(), window);
            });
        }
    }
}