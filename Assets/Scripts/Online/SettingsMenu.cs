using Elektronik.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UniRx;
using System.Net;
using Elektronik.Common.UI;

namespace Elektronik.Online
{
    public class SettingsMenu : MonoBehaviour
    {

        const string SETTINGS_FILE = @"online\settings.dat";

        #region Settings

        public OnlineSettingStore store;
        public UIListBox recentConnectionsListBox;
        public RecentIPListBoxItem recentConnectionPrefab;

        #endregion

        #region Buttons

        public GameObject settingSceneControls;

        #endregion

        #region UIs

        InputField uiMapInfoAddress;
        InputField uiMapInfoPort;
        InputField uiMapInfoScalingFactor;
        InputField uiVRAddress;
        InputField uiVRPort;
        InputField uiVRScalingFactor;
        Dropdown uiVRConnectionType;
        Button uiCancel;
        Button uiLoad;

        #endregion

        private void Awake()
        {
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
            uiVRAddress = GameObject.Find("VR address").GetComponent<InputField>();
            uiVRPort = GameObject.Find("VR port").GetComponent<InputField>();
            uiVRScalingFactor = GameObject.Find("VR scaling").GetComponent<InputField>();
            uiVRConnectionType = GameObject.Find("VR connection type").GetComponent<Dropdown>();
            uiCancel = GameObject.Find("Cancel").GetComponent<Button>();
            uiLoad = GameObject.Find("Load").GetComponent<Button>();
        }

        void SubscribeUIs()
        {
            uiMapInfoAddress.ObserveEveryValueChanged(addr => addr.text)
                .Do(content =>
                {
                    IPAddress stub = null;
                    uiLoad.enabled = IPAddress.TryParse(content, out stub) && !String.IsNullOrEmpty(uiMapInfoPort.text);
                })
                .Subscribe();
            uiCancel.OnClickAsObservable()
                .Do(_ => OnlineModeSettings.Current = null)
                .Subscribe(_ => SceneManager.LoadScene("Main menu", LoadSceneMode.Single));
            uiLoad.OnClickAsObservable()
                .Select(_ =>
                {
                    bool mapInfoAddressesAreEqual = String.Compare(uiMapInfoAddress.text, OnlineModeSettings.Current.MapInfoAddress.ToString()) == 0;
                    bool mapInfoPortsAreEqual = int.Parse(uiMapInfoPort.text) == OnlineModeSettings.Current.MapInfoPort;
                    return mapInfoAddressesAreEqual && mapInfoPortsAreEqual ? OnlineModeSettings.Current : new OnlineModeSettings();
                })
                .Do(oms => OnlineModeSettings.Current = oms)
                .Do(_ => OnlineModeSettings.Current.MapInfoScaling = uiMapInfoScalingFactor.text.Length == 0 ? 1.0f : float.Parse(uiMapInfoScalingFactor.text))
                .Do(_ => OnlineModeSettings.Current.MapInfoAddress = IPAddress.Parse(uiMapInfoAddress.text))
                .Do(_ => OnlineModeSettings.Current.MapInfoPort = Int32.Parse(uiMapInfoPort.text))
                .Do(_ => OnlineModeSettings.Current.VRAddress = IPAddress.Parse(uiVRAddress.text))
                .Do(_ => OnlineModeSettings.Current.VRPort = Int32.Parse(uiVRPort.text))
                .Do(_ => OnlineModeSettings.Current.VRScaling = uiVRScalingFactor.text.Length == 0 ? 1.0f : float.Parse(uiVRScalingFactor.text))
                .Do(_ => OnlineModeSettings.Current.VRConnectionType = 
                    uiVRConnectionType.value == 0 ? OnlineModeSettings.ConnectionTypes.UDP : OnlineModeSettings.ConnectionTypes.TCP)
                .Do(_ => OnlineModeSettings.Current.Time = DateTime.Now)
                .Do(_ => store.Add(OnlineModeSettings.Current))
                .Do(_ => store.Serialize(SETTINGS_FILE))
                .Do(_ => SceneManager.LoadScene("Empty", LoadSceneMode.Single))
                .Subscribe();
        }

        void AttachBehavior2RecentConnections()
        {
            var IPs = store.Recent;
            foreach (var recent in store.Recent)
            {
                recentConnectionPrefab.FullAddress = String.Format("{0}:{1}", recent.MapInfoAddress.ToString(), recent.MapInfoPort.ToString());
                recentConnectionPrefab.Time = recent.Time;
                recentConnectionsListBox.Add(recentConnectionPrefab);
            }
            if (store.Recent.Count > 0)
            {
                OnlineModeSettings.Current = store.Recent.First();
            }
            else
            {
                OnlineModeSettings.Current = new OnlineModeSettings();
            }
            recentConnectionsListBox.OnSelectionChanged += RecentIPChanged;
        }

        void RecentIPChanged(object sender, UIListBox.SelectionChangedEventArgs args)
        {
            OnlineModeSettings.Current = store.Recent[args.index];
            uiMapInfoAddress.text = OnlineModeSettings.Current.MapInfoAddress.ToString();
            uiMapInfoPort.text = OnlineModeSettings.Current.MapInfoPort.ToString();
            uiMapInfoScalingFactor.text = OnlineModeSettings.Current.MapInfoScaling.ToString();
            uiVRAddress.text = OnlineModeSettings.Current.VRAddress.ToString();
            uiVRPort.text = OnlineModeSettings.Current.VRPort.ToString();
            uiVRScalingFactor.text = OnlineModeSettings.Current.ToString();
            uiVRConnectionType.value =
                OnlineModeSettings.Current.VRConnectionType == OnlineModeSettings.ConnectionTypes.UDP ? 0 : 1;
        }
    }
}

