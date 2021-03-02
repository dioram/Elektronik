using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UI
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Button))]
    public class ButtonChangingIcons : MonoBehaviour
    {
        public Sprite[] Icons;
        public Image TargetImage;

        public int State
        {
            get => _state;
            set
            {
                _state = value % Icons.Length;
                TargetImage.sprite = Icons[_state];
            }
        }

        public IObservable<Unit> OnClickAsObservable() => _button.OnClickAsObservable();

        private Button _button;
        private int _state;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.OnClickAsObservable().Subscribe(_ => State++);
            TargetImage.sprite = Icons[0];
        }

    }
}