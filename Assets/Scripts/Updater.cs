using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace Elektronik
{
    /// <summary> Class for Elektronik self-update. </summary>
    public partial class Updater : MonoBehaviour
    {
        #region Editor fields

        [SerializeField] private GameObject ReleasePrefab;
        [SerializeField] private Transform ReleaseTarget;
        [SerializeField] private string ApiPath = "https://api.github.com/repos/dioram/Elektronik-Tools-2.0/releases";
        [SerializeField] private Button UpdateButton;
        [SerializeField] private Image UpdateButtonImage;
        [SerializeField] private Sprite NewReleaseSprite;
        [SerializeField] private Sprite NewPrereleaseSprite;

        #endregion

        #region Unity events

        private void Start()
        {
            GetReleases();
            ShowReleases();
        }

        #endregion

        #region Private

        private struct Release
        {
            public Version Version;
            public string ReleaseNotes;
            public bool IsPreRelease;

            public void Update()
            {
                var currentDir = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
                if (currentDir == null) throw new ArgumentNullException(nameof(currentDir));
                
                var updaterOldDir = Path.Combine(currentDir, "Plugins/Updater");
                var updaterNewDir = Path.Combine(currentDir, "../ElektronikUpdater");
                
                if (Directory.Exists(updaterNewDir)) Directory.Delete(updaterNewDir, true);
                Directory.Move(updaterOldDir, updaterNewDir);

                var process = new Process
                {
                    StartInfo =
                    {
                        WorkingDirectory = updaterNewDir,
                        FileName = Path.Combine(updaterNewDir, "Updater.exe"),
                        Arguments = $"{Version} {currentDir}",
                    }
                };
                process.Start();
                Application.Quit();
            }
        }

        private readonly List<Release> _releases = new List<Release>();
        
        private void GetReleases()
        {
            try
            {
                var request = WebRequest.CreateHttp(ApiPath);
                request.UserAgent = "request";
                var response = request.GetResponse();
                var serializer = new JsonSerializer();

                using var sr = new StreamReader(response.GetResponseStream()!);
                using var jsonTextReader = new JsonTextReader(sr);
                var data = serializer.Deserialize<JArray>(jsonTextReader);
                foreach (var field in data!)
                {
                    _releases.Add(new Release
                    {
                        Version = new Version((string) field["tag_name"]),
                        ReleaseNotes = (string) field["body"],
                        IsPreRelease = (bool) field["prerelease"],
                    });
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private void ShowReleases()
        {
            var releases = _releases.Where(r => r.Version > new Version(Application.version)).ToList();
            var hasReleases = releases.Any(r => !r.IsPreRelease);
            
            foreach (var release in releases)
            {
                var go = Instantiate(ReleasePrefab, ReleaseTarget);
                go.transform.Find("Header/Version").GetComponent<TMP_Text>().text = release.Version.ToString();
                go.transform.Find("Notes").GetComponent<TMP_Text>().text = release.ReleaseNotes;
                var button = go.transform.Find("Header/UpdateButton").GetComponent<Button>();
                button.OnClickAsObservable().Subscribe(_ => release.Update());
#if UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX
                button.gameObject.SetActive(false);
#endif
            }

            if (releases.Count > 0)
            {
                UpdateButton.gameObject.SetActive(true);
                UpdateButtonImage.sprite = hasReleases ? NewReleaseSprite : NewPrereleaseSprite;
            }
            else
            {
                UpdateButton.gameObject.SetActive(false);
            }
        }

        #endregion
    }
}