using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UniRx;
using VRTK;

namespace Elektronik.Offline
{
    public class Return2OfflineMenu : MonoBehaviour
    {

        void Start()
        {
            Button button = GetComponent<Button>();
            
            button.OnClickAsObservable()
                .Do(_ => VRTK_SDKManager.instance.UnloadSDKSetup())
                .Do(_ => SceneManager.LoadScene(@"Assets/Scenes/Offline/Offline settings.unity"))
                .Subscribe();
        }

    }
}