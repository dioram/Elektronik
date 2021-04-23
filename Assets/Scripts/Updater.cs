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

        private bool IsNewer(string version1, string version2, bool ignorePreRelease)
        {
            const string pattern = @"v?(\d*)(?:\.(\d*))?(?:\.(\d*))?(?:-rc(\d*))?";
            var match1 = Regex.Match(version1, pattern);
            var match2 = Regex.Match(version2, pattern);
            for (int i = 1; i < match1.Groups.Count; i++)
            {
                int v1 = string.IsNullOrEmpty(match1.Groups[i].Value) ? 0 : int.Parse(match1.Groups[i].Value);
                int v2 = string.IsNullOrEmpty(match2.Groups[i].Value) ? 0 : int.Parse(match2.Groups[i].Value);
                if (i == 4 && ignorePreRelease) return false;
                if (v1 > v2) return true;
                if (v1 < v2) return false;
            }

            return false;
        }

        private void GetReleases()
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
                try
                {
                    _releases.Add(new Release
                    {
                        Version = (string) field["tag_name"],
                        ReleaseNotes = (string) field["body"],
                        IsPreRelease = (bool) field["prerelease"],
                    });
                }
                catch
                {
                }
            }
        }

        private void ShowReleases(bool preReleases)
        {
            var releases = _releases
                    .Where(r => IsNewer(r.Version, Application.version, !preReleases))
                    .Where(r => r.IsPreRelease == preReleases).ToList();
            foreach (var release in releases)
            {
                var go = Instantiate(ReleasePrefab, preReleases ? PreReleaseRenderTarget : ReleaseRenderTarget);
                go.transform.Find("Header/Version").GetComponent<TMP_Text>().text = release.Version;
                go.transform.Find("Notes").GetComponent<TMP_Text>().text = release.ReleaseNotes;
                go.transform.Find("Header/UpdateButton").GetComponent<Button>().OnClickAsObservable()
                        .Subscribe(_ => release.Update());
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