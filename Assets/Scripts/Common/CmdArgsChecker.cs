using Elektronik.Common.UI;
using System;
using UnityEngine;

namespace Elektronik.Common
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