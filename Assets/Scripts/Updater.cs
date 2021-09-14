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

        public struct Version : IComparable<Version>
        {
            public readonly int Major;
            public readonly int Minor;
            public readonly int Fix;
            public readonly int Rc;
            public readonly bool Wip;

            public Version(string version)
            {
                const string pattern = @"v?(\d+)(?:\.(\d+))?(?:\.(\d+))?(?:-rc(\d+))?(-WIP)?";
                var match = Regex.Match(version, pattern);

                Major = Parse(match.Groups[1].Value);
                Minor = Parse(match.Groups[2].Value);
                Fix = Parse(match.Groups[3].Value);
                Rc = Parse(match.Groups[4].Value);
                Wip = !string.IsNullOrEmpty(match.Groups[5].Value);
            }

            public int CompareTo(Version other)
            {
                if (Major > other.Major) return 1;
                if (Major < other.Major) return -1;
            
                if (Minor > other.Minor) return 1;
                if (Minor < other.Minor) return -1;
            
                if (Fix > other.Fix) return 1;
                if (Fix < other.Fix) return -1;

                if (Rc == 0 && other.Rc > 0) return 1;
                if (other.Rc == 0 && Rc > 0) return -1;
            
                if (Rc > other.Rc) return 1;
                if (Rc < other.Rc) return -1;

                if (Wip) return -1;
                if (other.Wip) return 1;

                return 0;
            }

            public static bool operator >(Version first, Version second)
            {
                return first.CompareTo(second) > 0;
            }

            public static bool operator <(Version first, Version second)
            {
                return first.CompareTo(second) < 0;
            }

            public static bool operator ==(Version first, Version second)
            {
                return first.CompareTo(second) == 0;
            }

            public static bool operator !=(Version first, Version second)
            {
                return !(first == second);
            }

            public override string ToString()
            {
                var rc = Rc > 0 ? $"-rc{Rc}" : "";
                var wip = Wip ? "-WIP" : "";
                return $"v{Major}.{Minor}.{Fix}{rc}{wip}";
            }
            
            private static int Parse(string str) => int.TryParse(str, out var res) ? res : 0;
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
            public Version Version;
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
                        Version = new Version((string) field["tag_name"]),
                        ReleaseNotes = (string) field["body"],
                        IsPreRelease = (bool) field["prerelease"],
                    });
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                }
            }
        }

        private void ShowReleases(bool preReleases)
        {
            var releases = _releases
                    .Where(r => r.Version > new Version(Application.version))
                    .Where(r => r.IsPreRelease == preReleases).ToList();
            foreach (var release in releases)
            {
                var go = Instantiate(ReleasePrefab, preReleases ? PreReleaseRenderTarget : ReleaseRenderTarget);
                go.transform.Find("Header/Version").GetComponent<TMP_Text>().text = release.Version.ToString();
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