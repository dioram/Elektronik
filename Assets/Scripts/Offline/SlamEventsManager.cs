using Elektronik.Common.Maps;
using Elektronik.Offline.Commanders;
using Elektronik.Common.Presenters;
using Elektronik.Common.Data.Packages;
using Elektronik.Common.Extensions;
using Elektronik.Common.PackageViewUpdateCommandPattern;
using Elektronik.Common.Loggers;
using Elektronik.Offline.Settings;
using Elektronik.Common.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Elektronik.Common.Data.Pb;
using System.IO;
using System.Linq;

namespace Elektronik.Offline
{
    public class SlamEventsManager : MonoBehaviour
    {
        public bool ReadyToPlay { get; private set; }
        public PackageViewUpdateCommander[] commanders;
        public RepaintablePackagePresenter[] presenters;
        public SlamMap map;

        private PackageViewUpdateCommander m_commander;
        private PackagePresenter m_presenter;

        private LinkedListNode<IPackageViewUpdateCommand> m_currentCommand;
        private LinkedList<IPackageViewUpdateCommand> m_commands;
        private Dictionary<IPackageViewUpdateCommand, PacketPb> m_extendedEvents;
        private int m_position = 0;
        

        private void Awake()
        {
            m_extendedEvents = new Dictionary<IPackageViewUpdateCommand, PacketPb>();
            m_commands = new LinkedList<IPackageViewUpdateCommand>();
        }

        private void Start()
        {
            m_commander = commanders.BuildChain();
            m_presenter = presenters.BuildChain();
            StartCoroutine(ProcessEvents());
        }

        public void Clear()
        {
            map.Clear();
            foreach (var presenter in presenters)
                presenter.Clear();
            m_position = 0;
            m_currentCommand = m_commands.First;
            m_currentCommand.Value.Execute();
            m_presenter.Present(m_extendedEvents[m_currentCommand.Value]);
            Repaint();
        }

        public void Repaint()
        {
            foreach (var presenter in presenters)
                presenter.Repaint();
        }

        public int GetLength() => m_commands.Count;

        public int GetCurrentEventPosition() => m_position;

        public PacketPb GetCurrentEvent() => m_position == -1 ? null : m_extendedEvents[m_currentCommand.Value];

        public void SetPosition(int pos, Action whenPositionWasSet)
        {
            if (!ReadyToPlay)
                return;
            int maxLength = GetLength();
            Debug.AssertFormat(pos >= 0 && pos < maxLength, "[SlamEventsManger.SetPosition] out of range pos == {0}, but range is [0,{1})", pos, maxLength);
            StartCoroutine(MoveToPostion(pos, whenPositionWasSet));
        }

        private IEnumerator MoveToPostion(int pos, Action whenPositionWasSet)
        {
            ReadyToPlay = false;
            while (m_position != pos)
            {
                if (pos > m_position)
                    Next(false);
                if (pos < m_position)
                    Previous(false);
                if (m_position % 10 == 0)
                    yield return null;
            }
            whenPositionWasSet();
            Repaint();
            ReadyToPlay = true;
            yield return null;
        }

        public bool Next(bool needRepaint = true)
        {
            if (m_currentCommand.Next != null)
            {
                ++m_position;
                m_currentCommand = m_currentCommand.Next;
                m_currentCommand.Value.Execute();
                m_presenter.Present(m_extendedEvents[m_currentCommand.Value]);
                if (needRepaint)
                {
                    Repaint();
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Previous(bool needRepaint = true)
        {
            if (m_currentCommand.Previous != null)
            {
                --m_position;
                m_currentCommand.Value.UnExecute();
                m_currentCommand = m_currentCommand.Previous;
                m_presenter.Present(m_extendedEvents[m_currentCommand.Value]);
                if (needRepaint)
                {
                    Repaint();
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// We need this check because we do not want to switch iterations and come back if there is no any key event
        /// </summary>
        /// <param name="switchCommand">function that define Next or Previous event we need</param>
        /// <returns>true - if key event is found; false - otherwise</returns>
        private bool KeyEventIsFound(Func<LinkedListNode<IPackageViewUpdateCommand>, LinkedListNode<IPackageViewUpdateCommand>> switchCommand)
        {
            var command = switchCommand(m_currentCommand);
            bool isKey = false;
            while (!isKey && command != null)
            {
                if (isKey = m_extendedEvents[command.Value].Special)
                {
                    break;
                }
                command = switchCommand(command);
            }
            return isKey;
        }

        public bool NextKeyEvent()
        {
            if (KeyEventIsFound(c => c.Next))
            {
                while (Next(needRepaint: false) && !m_extendedEvents[m_currentCommand.Value].Special) { }
                Repaint();
                return true;
            }
            return false;
        }

        public bool PrevKeyEvent()
        {
            if (KeyEventIsFound(c => c.Previous))
            {
                while (Previous(needRepaint: false) && !m_extendedEvents[m_currentCommand.Value].Special) { }
                Repaint();
                return true;
            }
            return false;
        }

        private IEnumerator ProcessEvents()
        {
            ElektronikLogger.OpenLog();
            Application.logMessageReceived += ElektronikLogger.Log;
            Debug.Log("Parsing file...");
            using (var input = File.OpenRead(SettingsBag.Current[SettingName.Path].As<string>()))
            {
                int i = 0;
                var commands = new LinkedList<IPackageViewUpdateCommand>();
                while (input.Position != input.Length)
                {
                    var packet = PacketPb.Parser.ParseDelimitedFrom(input);
                    try
                    {
                        m_commander.GetCommands(packet, in commands);
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning(e.Message);
                        break;
                    }
                    foreach (var command in commands)
                        m_extendedEvents[command] = packet;
                    m_commands.MoveFrom(commands);
                    ++i;
                    yield return null;
                }
            }
            Debug.Log("Parsing file... OK!");
            Clear();
            Repaint();
            ReadyToPlay = true;
            Application.logMessageReceived -= ElektronikLogger.Log;
            ElektronikLogger.CloseLog();
            yield return null;
        }
    }
}
