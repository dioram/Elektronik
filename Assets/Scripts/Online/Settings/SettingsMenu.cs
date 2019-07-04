using Elektronik.Common.Settings;
using Elektronik.Online.UI;
using System;
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

        const string SETTINGS_FILE = @"online\settings.dat";

        #region Settings

        public SettingsBagStore store;
        public UIListBox recentConnectionsListBox;

        #endregion

        #region UIs

        InputField uiMapInfoAddress;
        InputField uiMapInfoPort;
        InputField uiMapInfoScalingFactor;
        Button uiCancel;
        Button uiLoad;

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
            store.Deserialize(SETTINGS_FILE);
            AttachBehavior2RecentConnections();
        }

        void FindUIs()
        {
            uiMapInfoAddress = GameObject.Find("Map info address").GetComponent<InputField>();
            uiMapInfoPort = GameObject.Find("Map info port").GetComponent<InputField>();
            uiMapInfoScalingFactor = GameObject.Find("Map info scaling").GetComponent<InputField>();
            uiCancel = GameObject.Find("Cancel").GetComponent<Button>();
            uiLoad = GameObject.Find("Load").GetComponent<Button>();
        }

        SettingsBag SettingsSelector()
        {
            if (SettingsBag.Current.TryGetValue(SettingName.IPAddress, out Setting ipSetting) &&
                SettingsBag.Current.TryGetValue(SettingName.Port, out Setting portSetting))
            {
                bool mapInfoAddressesAreEqual = uiMapInfoAddress.text == ipSetting.As<IPAddress>().ToString();
                bool mapInfoPortsAreEqual = int.Parse(uiMapInfoPort.text) == portSetting.As<int>();
                return mapInfoAddressesAreEqual && mapInfoPortsAreEqual ? SettingsBag.Current : new SettingsBag();
            }
            return new SettingsBag();
        }

        void SubscribeUIs()
        {
            uiMapInfoAddress.ObserveEveryValueChanged(addr => addr.text)
                .Do(content => uiLoad.enabled = IPAddress.TryParse(content, out IPAddress stub) && !String.IsNullOrEmpty(uiMapInfoPort.text))
                .Subscribe();
            uiCancel.OnClickAsObservable()
                .Do(_ => SettingsBag.Current = null)
                .Subscribe(_ => SceneManager.LoadScene("Main menu", LoadSceneMode.Single));
            uiLoad.OnClickAsObservable()
                .Select(_ => SettingsSelector())
                .Do(oms => SettingsBag.Current = oms)
                .Do(_ => SettingsBag.Current.Change(SettingName.Scale,
                    uiMapInfoScalingFactor.text.Length == 0 ? 1f : float.Parse(uiMapInfoScalingFactor.text)))
                .Do(_ => SettingsBag.Current.Change(SettingName.IPAddress, IPAddress.Parse(uiMapInfoAddress.text)))
                .Do(_ => SettingsBag.Current.Change(SettingName.Port, Int32.Parse(uiMapInfoPort.text)))
                .Do(_ => store.Add(SettingsBag.Current))
                .Do(_ => store.Serialize(SETTINGS_FILE))
                .Do(_ => SceneManager.LoadScene("Empty", LoadSceneMode.Single))
                .Subscribe();
        }

        void AttachBehavior2RecentConnections()
        {
            var IPs = store.Recent;
            foreach (var recent in store.Recent)
            {
                var recentConnectionItem = recentConnectionsListBox.Add() as RecentIPListBoxItem;
                recentConnectionItem.FullAddress = $"{recent[SettingName.IPAddress].As<IPAddress>()}:{recent[SettingName.Port].As<int>()}";
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
            SettingsBag.Current = store.Recent[args.index];
            uiMapInfoAddress.text = SettingsBag.Current[SettingName.IPAddress].As<IPAddress>().ToString();
            uiMapInfoPort.text = SettingsBag.Current[SettingName.Port].As<int>().ToString();
            uiMapInfoScalingFactor.text = SettingsBag.Current[SettingName.Scale].As<float>().ToString();
        }
    }
}

