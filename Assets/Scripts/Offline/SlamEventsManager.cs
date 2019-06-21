using Elektronik.Common;
using Elektronik.Common.Data;
using Elektronik.Common.Extensions;
using Elektronik.Common.PackageViewUpdateCommandPattern;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Elektronik.Offline
{
    public class SlamEventsManager : MonoBehaviour
    {
        public bool ReadyToPlay { get; private set; }
        public float scale;
        public RepaintablePackageViewUpdateCommander[] commanders;

        private IPackage[] m_packages;
        private LinkedList<IPackageViewUpdateCommand> m_commands;
        private Dictionary<IPackageViewUpdateCommand, IPackage> m_extendedEvents;
        private PackageViewUpdateCommander m_commander;
        private DataSource m_dataSource;
        private int m_position = 0;
        private LinkedListNode<IPackageViewUpdateCommand> m_currentCommand;

        private void Awake()
        {
            m_extendedEvents = new Dictionary<IPackageViewUpdateCommand, IPackage>();
            m_commands = new LinkedList<IPackageViewUpdateCommand>();
            m_dataSource = new DataSource();
        }

        private void Start()
        {
            m_commander = commanders.BuildChain();
            StartCoroutine(ProcessEvents());
        }

        public void Clear()
        {
            foreach (var commander in commanders)
                commander.Clear();
            m_position = 0;
            m_currentCommand = m_commands.First;
            m_currentCommand.Value.Execute();
            Repaint();
        }

        public void Repaint()
        {
            foreach (var commander in commanders)
                commander.Repaint();
        }

        public int GetLength() => m_commands.Count;

        public int GetCurrentEventPosition() => m_position;

        public IPackage GetCurrentEvent() => m_position == -1 ? null : m_extendedEvents[m_currentCommand.Value];

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

        private bool FindKeyEvent(Func<LinkedListNode<IPackageViewUpdateCommand>, LinkedListNode<IPackageViewUpdateCommand>> switchCommand)
        {
            var command = switchCommand(m_currentCommand);
            bool isKey = false;
            while (!isKey && command != null)
            {
                if (isKey = m_extendedEvents[m_currentCommand.Value].IsKey)
                {
                    break;
                }
                command = switchCommand(command);
            }
            return isKey;
        }

        public bool NextKeyEvent()
        {
            if (FindKeyEvent(c => c.Next))
            {
                while (Next(needRepaint: false) && !m_extendedEvents[m_currentCommand.Value].IsKey) { }
                Repaint();
                return true;
            }
            return false;
        }

        public bool PrevKeyEvent()
        {
            if (FindKeyEvent(c => c.Previous))
            {
                while (Previous(needRepaint: false) && !m_extendedEvents[m_currentCommand.Value].IsKey) { }
                Repaint();
                return true;
            }
            return false;
        }

        private IEnumerator ProcessEvents()
        {
            ElektronikLogger.OpenLog();
            Application.logMessageReceived += ElektronikLogger.Log;
            Debug.Log("ANALYSIS STARTED");
            m_packages = m_dataSource.Parse(FileModeSettings.Current.Path);
            Debug.Log("PROCESSING FINISHED");
            for (int i = 0; i < m_packages.Length; ++i)
            {
                Debug.Log(m_packages[i]);
                var pkgCommands = m_commander.GetCommands(m_packages[i]);
                foreach (var pkgCommand in pkgCommands)
                    m_extendedEvents[pkgCommand] = m_packages[i];
                m_commands.MoveFrom(pkgCommands);
                if (i % 10 == 0) yield return null;
            }
            Debug.Log("PROCESSING FINISHED");
            Clear();
            Repaint();
            ReadyToPlay = true;
            Application.logMessageReceived -= ElektronikLogger.Log;
            ElektronikLogger.CloseLog();
            yield return null;
        }
    }
}
