using System;
using Elektronik.UI;
using UnityEngine;

namespace Elektronik
{
    public class CmdArgsChecker : MonoBehaviour
    {
        public MainMenuButtons mainMenu;

        // Start is called before the first frame update
        void Start()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                mainMenu.OnOfflineModeClick();
            }
        }
    }
}