using Elektronik.Common;
using Elektronik.Common.Clouds;
using Elektronik.Common.Containers;
using Elektronik.Common.Data;
using Elektronik.Common.PackageViewUpdateCommandPattern;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elektronik.Offline
{
    public class SlamEventsManager : MonoBehaviour
    {
        public bool ReadyToPlay { get; private set; }
        public float scale;

        IList<IPackage> m_packages;
        LinkedList<IPackageViewUpdateCommand> m_commands;
        IList<IPackage> m_extendedEvents;
        

        public Helmet helmet;
        public RepaintablePackageViewUpdateCommander[] commanders;
        public PackagePresenter[] presenters;
        private PackageViewUpdateCommander m_commander;

        private DataSource m_dataSource;

        private int m_position = -1;
        private LinkedListNode<IPackageViewUpdateCommand> m_currentCommand;

        void Awake()
        {
            m_extendedEvents = new List<IPackage>();
            m_commands = new LinkedList<IPackageViewUpdateCommand>();
            m_dataSource = new DataSource();
            m_dataSource.DataReady += (IList<IPackage> packages) => 
            {
                m_packages = packages;
                StartCoroutine(ProcessEvents());
            };
        }

        void Start()
        {
            PackageViewUpdateCommander commander = m_commander = null;
            for (int i = 0; i < presenters.Length; ++i)
            {
                if (commander == null)
                    m_commander = commander = commanders[i];
                else
                    commander = commander.SetSuccessor(commanders[i]);
            }
            ElektronikLogger.OpenLog();
            Application.logMessageReceived += ElektronikLogger.Log;
            m_dataSource.ParseData(FileModeSettings.Current.Path);
            Application.logMessageReceived -= ElektronikLogger.Log;
            ElektronikLogger.CloseLog();
        }

        public void Clear()
        {
            m_position = -1;
            foreach (var commander in commanders)
                commander.Clear();
            helmet.ResetHelmet();
        }

        public void Repaint()
        {
            foreach (var commander in commanders)
                commander.Clear();
        }

        public int GetLength() => m_commands.Count;

        public int GetCurrentEventPosition() => m_position;

        public IPackage GetCurrentEvent()
        {
            if (m_position == -1) // до свершения какого либо события
                return null;
            return m_extendedEvents[m_position];
        }

        public void SetPosition(int pos, Action whenPositionWasSet)
        {
            if (!ReadyToPlay)
                return;
            int maxLength = GetLength();
            Debug.AssertFormat(pos >= 0 && pos < maxLength, "[SlamEventsManger.SetPosition] out of range pos == {0}, but range is [0,{1})", pos, maxLength);
            StartCoroutine(MoveToPostion(pos, whenPositionWasSet));
        }

        IEnumerator MoveToPostion(int pos, Action whenPositionWasSet)
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
            if (m_currentCommand != m_commands.Last)
            {
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
            if (m_currentCommand != m_commands.First)
            {
                m_currentCommand = m_currentCommand.Previous;
                m_currentCommand.Value.UnExecute();
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

        private int FindNextKeyEventIdx(int srcIdx)
        {
            for (int i = srcIdx; i < m_extendedEvents.Count; ++i)
            {
                if (m_extendedEvents[i].IsKey)
                {
                    return i;
                }
            }
            return -1;
        }

        public bool NextKeyEvent()
        {
            int idxOfKeyEvent = FindNextKeyEventIdx(m_position + 1);
            if (idxOfKeyEvent == -1)
                return false;
            while (m_position != idxOfKeyEvent)
            {
                Next(needRepaint: false);
            }
            Repaint();
            return true;
        }

        private int FindPrevKeyEventIdx(int srcIdx)
        {
            for (int i = srcIdx; i >= 0; --i)
            {
                if (m_extendedEvents[i].IsKey)
                {
                    return i;
                }
            }
            return -1;
        }

        public bool PrevKeyEvent()
        {
            if (m_position == GetLength())
                Previous(needRepaint: false);
            int idxOfKeyEvent = FindPrevKeyEventIdx(m_position - 1);
            if (idxOfKeyEvent == -1)
                return false;
            while (m_position != idxOfKeyEvent)
            {
                Previous(needRepaint: false);
            }
            Repaint();
            return true;
        }

        IEnumerator ProcessEvents()
        {
            Debug.Log("PROCESSING STARTED");
            for (int i = 0; i < m_packages.Count; ++i)
            {
                Debug.Log(m_packages[i]);
                LinkedList<IPackageViewUpdateCommand> pkgCommands = m_commander.GetCommands(m_packages[i]);
                m_commands.AddLast(pkgCommands.First);
                foreach (var pkgCommand in pkgCommands)
                    m_extendedEvents.Add(m_packages[i]);
                if (i % 10 == 0)
                {
                    yield return null;
                }
            }
            Debug.Log("PROCESSING FINISHED");
            Clear();
            Repaint();
            ReadyToPlay = true;
            yield return null;
        }
    }
}
