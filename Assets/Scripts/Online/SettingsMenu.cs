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

        InputField uiAddress;
        InputField uiPort;
        InputField uiScalingFactor;
        Dropdown uiConnectionType;
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
            uiAddress = GameObject.Find("Input address").GetComponent<InputField>();
            uiPort = GameObject.Find("Input port").GetComponent<InputField>();
            uiScalingFactor = GameObject.Find("Input scaling").GetComponent<InputField>();
            uiConnectionType = GameObject.Find("Dropdown connection type").GetComponent<Dropdown>();
            uiCancel = GameObject.Find("Cancel").GetComponent<Button>();
            uiLoad = GameObject.Find("Load").GetComponent<Button>();
        }

        void SubscribeUIs()
        {
            uiAddress.ObserveEveryValueChanged(addr => addr.text)
                .Do(content =>
                {
                    IPAddress stub = null;
                    uiLoad.enabled = IPAddress.TryParse(content, out stub) && !String.IsNullOrEmpty(uiPort.text);
                })
                .Subscribe();
            uiCancel.OnClickAsObservable()
                .Subscribe(_ => SceneManager.LoadScene("Main menu", LoadSceneMode.Single));
            uiLoad.OnClickAsObservable()
                .Select(_ =>
                {
                    bool addressesAreEqual = String.Compare(uiAddress.text, OnlineModeSettings.Current.Address.ToString()) == 0;
                    bool portsAreEqual = int.Parse(uiPort.text) == OnlineModeSettings.Current.Port;
                    return addressesAreEqual && portsAreEqual ? OnlineModeSettings.Current : new OnlineModeSettings();
                })
                .Do(oms => OnlineModeSettings.Current = oms)
                .Do(_ => OnlineModeSettings.Current.Scaling = uiScalingFactor.text.Length == 0 ? 1.0f : float.Parse(uiScalingFactor.text))
                .Do(_ => OnlineModeSettings.Current.Address = IPAddress.Parse(uiAddress.text))
                .Do(_ => OnlineModeSettings.Current.Port = Int32.Parse(uiPort.text))
                .Do(_ => OnlineModeSettings.Current.ConnectionType = 
                    uiConnectionType.value == 0 ? OnlineModeSettings.ConnectionTypes.UDP : OnlineModeSettings.ConnectionTypes.TCP)
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
                recentConnectionPrefab.FullAddress = String.Format("{0}:{1}", recent.Address.ToString(), recent.Port.ToString());
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
            uiAddress.text = OnlineModeSettings.Current.Address.ToString();
            uiPort.text = OnlineModeSettings.Current.Port.ToString();
            uiScalingFactor.text = OnlineModeSettings.Current.Scaling.ToString();
            uiConnectionType.value =
                OnlineModeSettings.Current.ConnectionType == OnlineModeSettings.ConnectionTypes.UDP ? 0 : 1;
        }
    }
}

