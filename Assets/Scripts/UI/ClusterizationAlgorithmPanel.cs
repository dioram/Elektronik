using System;
using Elektronik.PluginsSystem;
using Elektronik.UI.SettingsFields;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using SettingsBag = Elektronik.Settings.SettingsBag;

namespace Elektronik.UI
{
    [RequireComponent(typeof(SettingsFieldsUiGenerator))]
    public class ClusterizationAlgorithmPanel : MonoBehaviour
    {
        #region Editor fields

        [SerializeField] private Button ComputeButton;
        [SerializeField] private GameObject PanelLocker;
        [SerializeField] private TMP_Text ErrorLabel;
        [SerializeField] private TMP_Text NameLabel;

        #endregion
        
        public void Setup(IClusteringAlgorithmFactory factory)
        {
            GetComponent<SettingsFieldsUiGenerator>().Generate(factory.Settings);
            _settingsBag = factory.Settings;
            _subject = new Subject<Unit>();
            NameLabel.text = factory.DisplayName;
            OnComputeRequested = _subject.Select(_ => (IClusteringAlgorithm)factory.Start());
        }

        public IObservable<IClusteringAlgorithm> OnComputeRequested;

        #region Unity events

        private void Awake()
        {
            ComputeButton.OnClickAsObservable()
                .Select(_ => _settingsBag?.Validate() ?? SettingsBag.ValidationResult.Failed(""))
                .Where(v => !v.Success)
                .Do(_ => ErrorLabel.gameObject.SetActive(true))
                .Subscribe(v => ErrorLabel.text = v.Message)
                .AddTo(this);
            
            ComputeButton.OnClickAsObservable()
                .Select(_ => _settingsBag?.Validate() ?? SettingsBag.ValidationResult.Failed(""))
                .Where(v => v.Success)
                .Do(_ => ErrorLabel.gameObject.SetActive(false))
                .Do(_ => enabled = false)
                .Select(_ => Unit.Default)
                .Subscribe(_subject)
                .AddTo(this);
        }

        private void OnEnable()
        {
            ComputeButton.interactable = true;
            PanelLocker.SetActive(false);
        }

        private void OnDisable()
        {
            ComputeButton.interactable = false;
            PanelLocker.SetActive(true);
        }

        #endregion

        #region Private

        private Subject<Unit> _subject;
        private SettingsBag _settingsBag;

        #endregion
    }
}