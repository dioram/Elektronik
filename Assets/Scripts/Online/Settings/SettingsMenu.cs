using Elektronik.Common.Settings;
using Elektronik.Online.UI;
using System;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UniRx;
using System.Net;
using Elektronik.Common.UI;

namespace Elektronik.Online.Settings
{
    public class SettingsMenu : MonoBehaviour
    {
        #region Settings

        public UIListBox recentConnectionsListBox;
        public int MaxCountOfRecentFiles;

        #endregion

        #region UIs

        private InputField _uiMapInfoAddress;
        private InputField _uiMapInfoPort;
        private InputField _uiMapInfoScalingFactor;
        private Button _cancelButton;
        private Button _loadButton;
        private SettingsHistory<OnlineSettingsBag> _settingsHistory;

        #endregion

        private void Awake()
        {
            ModeSelector.Mode = Mode.Online;
            FindUIs();
            SubscribeUIs();
        }

        // Start is called before the first frame update
        void Start()
        {
            _settingsHistory = new SettingsHistory<OnlineSettingsBag>(MaxCountOfRecentFiles);
            AttachBehavior2RecentConnections();
        }

        void FindUIs()
        {
            _uiMapInfoAddress = GameObject.Find("Map info address").GetComponent<InputField>();
            _uiMapInfoPort = GameObject.Find("Map info port").GetComponent<InputField>();
            _uiMapInfoScalingFactor = GameObject.Find("Map info scaling").GetComponent<InputField>();
            _cancelButton = GameObject.Find("Cancel").GetComponent<Button>();
            _loadButton = GameObject.Find("Load").GetComponent<Button>();
        }

        void SubscribeUIs()
        {
            _uiMapInfoAddress.ObserveEveryValueChanged(addr => addr.text)
                .Do(content => _loadButton.enabled = IPAddress.TryParse(content, out IPAddress _) && !String.IsNullOrEmpty(_uiMapInfoPort.text))
                .Subscribe();
            _cancelButton.OnClickAsObservable()
                .Do(_ => SettingsBag.RemoveCurrent(typeof(OnlineSettingsBag)))
                .Subscribe(_ => SceneManager.LoadScene("Main menu", LoadSceneMode.Single));
            _loadButton.OnClickAsObservable()
                .Select(_ => new OnlineSettingsBag())
                .Do(SettingsBag.SetCurrent)
                .Do(osb => osb.Scale = _uiMapInfoScalingFactor.text.Length == 0 ? 1f : float.Parse(_uiMapInfoScalingFactor.text))
                .Do(osb => osb.IPAddress = _uiMapInfoAddress.text)
                .Do(osb => osb.Port = Int32.Parse(_uiMapInfoPort.text))
                .Do(osb => _settingsHistory.Add(osb))
                .Do(_ => _settingsHistory.Save())
                .Do(_ => SceneManager.LoadScene("Empty", LoadSceneMode.Single))
                .Subscribe();
        }

        void AttachBehavior2RecentConnections()
        {
            foreach (var recent in _settingsHistory.Recent)
            {
                var recentConnectionItem = recentConnectionsListBox.Add() as RecentIPListBoxItem;
                recentConnectionItem.FullAddress = $"{recent.IPAddress}:{recent.Port}";
                recentConnectionItem.Time = recent.ModificationTime;
            }
            SettingsBag.SetCurrent(_settingsHistory.Recent.Count > 0 ? _settingsHistory.Recent.First() : new OnlineSettingsBag());
            recentConnectionsListBox.OnSelectionChanged += RecentIPChanged;
        }

        void RecentIPChanged(object sender, UIListBox.SelectionChangedEventArgs args)
        {
            var current = _settingsHistory.Recent[args.Index];
            SettingsBag.SetCurrent(current);
            _uiMapInfoAddress.text = current.IPAddress;
            _uiMapInfoPort.text = current.Port.ToString();
            _uiMapInfoScalingFactor.text = current.Scale.ToString(CultureInfo.CurrentCulture);
        }
    }
}

