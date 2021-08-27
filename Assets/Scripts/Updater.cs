using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik
{
    public class Updater : MonoBehaviour
    {
        [SerializeField] private GameObject ReleasePrefab;
        [SerializeField] private Transform ReleaseRenderTarget;
        [SerializeField] private Transform PreReleaseRenderTarget;
        [SerializeField] private GameObject ReleasesPanel;
        [SerializeField] private GameObject PrereleasesPanel;
        [SerializeField] private string ApiPath = "https://api.github.com/repos/dioram/Elektronik-Tools-2.0/releases";
        [SerializeField] private TMP_Text VersionLabel;
        [SerializeField] private Color NewReleaseColor;
        [SerializeField] private Color NewPrereleaseColor;
        
        public static bool IsNewer(string version1, string version2)
        {
            const string pattern = @"v?(\d+)(?:\.(\d+))?(?:\.(\d+))?(?:-rc(\d+))?(-WIP)?";
            var match1 = Regex.Match(version1, pattern);
            var match2 = Regex.Match(version2, pattern);

            var major1 = Parse(match1.Groups[1].Value);
            var minor1 = Parse(match1.Groups[2].Value);
            var fix1 = Parse(match1.Groups[3].Value);
            var rc1 = Parse(match1.Groups[4].Value);
            var wip1 = !string.IsNullOrEmpty(match1.Groups[5].Value);

            var major2 = Parse(match2.Groups[1].Value);
            var minor2 = Parse(match2.Groups[2].Value);
            var fix2 = Parse(match2.Groups[3].Value);
            var rc2 = Parse(match2.Groups[4].Value);
            var wip2 = !string.IsNullOrEmpty(match2.Groups[5].Value);

            if (major1 > major2) return true;
            if (major1 < major2) return false;
            
            if (minor1 > minor2) return true;
            if (minor1 < minor2) return false;
            
            if (fix1 > fix2) return true;
            if (fix1 < fix2) return false;

            if (rc1 == 0 && rc2 > 0) return true;
            if (rc2 == 0 && rc1 > 0) return false;
            
            if (rc1 > rc2) return true;
            if (rc1 < rc2) return false;

            if (wip1) return false;
            if (wip2) return true;

            return false;
        }
        
        #region Unity events

        private void Start()
        {
            GetReleases();
            ShowReleases(true);
            ShowReleases(false);
        }

        #endregion

        #region Private

        private struct Release
        {
            public string Version;
            public string ReleaseNotes;
            public bool IsPreRelease;

            public void Update()
            {
                var currentDir = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
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
        
        private static int Parse(string str) => int.TryParse(str, out var res) ? res : 0;

        private void GetReleases()
        {
            try
            {
                var request = WebRequest.CreateHttp(ApiPath);
                request.UserAgent = "request";
                var response = request.GetResponse();
                var serializer = new JsonSerializer();

                using var sr = new StreamReader(response.GetResponseStream());
                using var jsonTextReader = new JsonTextReader(sr);
                var data = serializer.Deserialize<JArray>(jsonTextReader);
                foreach (var field in data)
                {
                    _releases.Add(new Release
                    {
                        Version = (string)field["tag_name"],
                        ReleaseNotes = (string)field["body"],
                        IsPreRelease = (bool)field["prerelease"],
                    });
                }
            }
            catch (Exception e)
            {
                // Catching any network or parsing exceptions
            }
        }

        private void ShowReleases(bool preReleases)
        {
            var releases = _releases
                    .Where(r => IsNewer(r.Version, Application.version))
                    .Where(r => r.IsPreRelease == preReleases).ToList();
            foreach (var release in releases)
            {
                var go = Instantiate(ReleasePrefab, preReleases ? PreReleaseRenderTarget : ReleaseRenderTarget);
                go.transform.Find("Header/Version").GetComponent<TMP_Text>().text = release.Version;
                go.transform.Find("Notes").GetComponent<TMP_Text>().text = release.ReleaseNotes;
                var updateButton =  go.transform.Find("Header/UpdateButton").GetComponent<Button>();
                updateButton.OnClickAsObservable().Subscribe(_ => release.Update());
#if UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX
                updateButton.gameObject.SetActive(false);
#endif
            }

            if (releases.Count > 0)
            {
                VersionLabel.color = preReleases ? NewPrereleaseColor : NewReleaseColor;
                return;
            }
            if (preReleases) PrereleasesPanel.SetActive(false);
            else ReleasesPanel.SetActive(false);
        }

        #endregion
    }
}