using System;
using Elektronik.PluginsSystem;
using Elektronik.Settings.Bags;
using Elektronik.UI.SettingsFields;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI
{
    [RequireComponent(typeof(SettingsGenerator))]
    public class ClusterizationAlgorithmPanel : MonoBehaviour
    {
        #region Editor fields

        [SerializeField] private Button ComputeButton;
        [SerializeField] private GameObject PanelLocker;
        [SerializeField] private TMP_Text ErrorLabel;
        [SerializeField] private TMP_Text NameLabel;

        #endregion
        
        public void Setup(IClusterizationAlgorithmFactory factory)
        {
            _generator = GetComponent<SettingsGenerator>();
            _subject = new Subject<Unit>();
            _generator.Generate(factory.Settings);
            NameLabel.text = factory.DisplayName;
            OnComputeRequested = _subject.Select(_ => (IClusterizationAlgorithm)factory.Start(null));
        }

        public IObservable<IClusterizationAlgorithm> OnComputeRequested;

        #region Unity events

        private void Awake()
        {
            ComputeButton.OnClickAsObservable()
                .Select(_ => _generator.Settings?.Validate() ?? SettingsBag.ValidationResult.Failed(""))
                .Where(v => !v.Success)
                .Do(_ => ErrorLabel.gameObject.SetActive(true))
                .Subscribe(v => ErrorLabel.text = v.Message)
                .AddTo(this);
            
            ComputeButton.OnClickAsObservable()
                .Select(_ => _generator.Settings?.Validate() ?? SettingsBag.ValidationResult.Failed(""))
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
        private SettingsGenerator _generator;

        #endregion
    }
}