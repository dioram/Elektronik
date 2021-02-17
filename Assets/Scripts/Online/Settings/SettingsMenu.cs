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

        const string SettingsFile = @"online\settings.dat";

        #region Settings

        public SettingsBagStore store;
        public UIListBox recentConnectionsListBox;

        #endregion

        #region UIs

        private InputField _uiMapInfoAddress;
        private InputField _uiMapInfoPort;
        private InputField _uiMapInfoScalingFactor;
        private Button _cancelButton;
        private Button _loadButton;

        #endregion

        private void Awake()
        {
            SettingsBag.Mode = Mode.Online;
            FindUIs();
            SubscribeUIs();
        }

        // Start is called before the first frame update
        void Start()
        {
            store.Deserialize(SettingsFile);
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

        SettingsBag SettingsSelector()
        {
            if (SettingsBag.Current.TryGetValue(SettingName.IPAddress, out Setting ipSetting) &&
                SettingsBag.Current.TryGetValue(SettingName.Port, out Setting portSetting))
            {
                bool mapInfoAddressesAreEqual = _uiMapInfoAddress.text == ipSetting.As<string>();
                bool mapInfoPortsAreEqual = int.Parse(_uiMapInfoPort.text) == portSetting.As<int>();
                return mapInfoAddressesAreEqual && mapInfoPortsAreEqual ? SettingsBag.Current : new SettingsBag();
            }
            return new SettingsBag();
        }

        void SubscribeUIs()
        {
            _uiMapInfoAddress.ObserveEveryValueChanged(addr => addr.text)
                .Do(content => _loadButton.enabled = IPAddress.TryParse(content, out IPAddress _) && !String.IsNullOrEmpty(_uiMapInfoPort.text))
                .Subscribe();
            _cancelButton.OnClickAsObservable()
                .Do(_ => SettingsBag.Current = null)
                .Subscribe(_ => SceneManager.LoadScene("Main menu", LoadSceneMode.Single));
            _loadButton.OnClickAsObservable()
                .Select(_ => SettingsSelector())
                .Do(oms => SettingsBag.Current = oms)
                .Do(_ => SettingsBag.Current.Change(SettingName.Scale,
                    _uiMapInfoScalingFactor.text.Length == 0 ? 1f : float.Parse(_uiMapInfoScalingFactor.text)))
                .Do(_ => SettingsBag.Current.Change(SettingName.IPAddress, _uiMapInfoAddress.text))
                .Do(_ => SettingsBag.Current.Change(SettingName.Port, Int32.Parse(_uiMapInfoPort.text)))
                .Do(_ => store.Add(SettingsBag.Current))
                .Do(_ => store.Serialize(SettingsFile))
                .Do(_ => SceneManager.LoadScene("Empty", LoadSceneMode.Single))
                .Subscribe();
        }

        void AttachBehavior2RecentConnections()
        {
            foreach (var recent in store.Recent)
            {
                var recentConnectionItem = recentConnectionsListBox.Add() as RecentIPListBoxItem;
                recentConnectionItem.FullAddress = $"{recent[SettingName.IPAddress].As<string>()}:{recent[SettingName.Port].As<int>()}";
                recentConnectionItem.Time = recent.ModificationTime;
            }
            if (store.Recent.Count > 0)
            {
                SettingsBag.Current = store.Recent.First();
            }
            else
            {
                SettingsBag.Current = new SettingsBag();
            }
            recentConnectionsListBox.OnSelectionChanged += RecentIPChanged;
        }

        void RecentIPChanged(object sender, UIListBox.SelectionChangedEventArgs args)
        {
            SettingsBag.Current = store.Recent[args.Index];
            _uiMapInfoAddress.text = SettingsBag.Current[SettingName.IPAddress].As<string>();
            _uiMapInfoPort.text = SettingsBag.Current[SettingName.Port].As<int>().ToString();
            _uiMapInfoScalingFactor.text = SettingsBag.Current[SettingName.Scale].As<float>().ToString(CultureInfo.CurrentCulture);
        }
    }
}

