using Elektronik.Common.Presenters;
using Elektronik.Common.Extensions;
using Elektronik.Common.Commands;
using Elektronik.Common.Loggers;
using Elektronik.Offline.Settings;
using Elektronik.Common.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Elektronik.Common.Data.Pb;
using System.IO;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.Converters;

namespace Elektronik.Offline
{
    public class SlamEventsManager : MonoBehaviour
    {
        public bool ReadyToPlay { get; private set; }
        public Commander[] commanders;
        public RepaintablePackagePresenter[] presenters;
        public CSConverter converter;
        public GameObject Containers;

        private Commander _commander;
        private PackagePresenter _presenter;

        private LinkedListNode<ICommand> _currentCommand;
        private LinkedList<ICommand> _commands;
        private Dictionary<ICommand, PacketPb> _extendedEvents;
        private int _position = 0;

        private void Awake()
        {
            _extendedEvents = new Dictionary<ICommand, PacketPb>();
            _commands = new LinkedList<ICommand>();
        }

        private void Start()
        {
            _commander = commanders.BuildChain();
            converter.SetInitTRS(
                Vector3.zero,
                Quaternion.identity,
                Vector3.one * SettingsBag.Current[SettingName.Scale].As<float>());
            _commander.SetConverter(converter);

            _presenter = presenters.BuildChain();
            StartCoroutine(ProcessEvents());
        }

        public void Clear()
        {
            foreach (var container in Containers.GetComponentsInChildren<IClearable>())
            {
                container.Clear();
            }
            foreach (var presenter in presenters)
                presenter.Clear();
            _position = 0;
            _currentCommand = _commands.First;
            _currentCommand.Value.Execute();
            _presenter.Present(_extendedEvents[_currentCommand.Value]);
            Repaint();
        }

        public void Repaint()
        {
            foreach (var presenter in presenters)
                presenter.Repaint();
        }

        public int GetLength() => _commands.Count;

        public int GetCurrentEventPosition() => _position;

        public PacketPb GetCurrentEvent() => _position == -1 ? null : _extendedEvents[_currentCommand.Value];

        public void SetPosition(int pos, Action whenPositionWasSet)
        {
            if (!ReadyToPlay)
                return;
            int maxLength = GetLength();
            Debug.AssertFormat(pos >= 0 && pos < maxLength, "[SlamEventsManger.SetPosition] out of range pos == {0}, but range is [0,{1})", pos, maxLength);
            StartCoroutine(MoveToPosition(pos, whenPositionWasSet));
        }

        private IEnumerator MoveToPosition(int pos, Action whenPositionWasSet)
        {
            ReadyToPlay = false;
            while (_position != pos)
            {
                if (pos > _position)
                    Next(false);
                if (pos < _position)
                    Previous(false);
                if (_position % 10 == 0)
                    yield return null;
            }
            whenPositionWasSet();
            Repaint();
            ReadyToPlay = true;
            yield return null;
        }

        public bool Next(bool needRepaint = true)
        {
            if (_currentCommand.Next != null)
            {
                ++_position;
                _currentCommand = _currentCommand.Next;
                _currentCommand.Value.Execute();
                _presenter.Present(_extendedEvents[_currentCommand.Value]);
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
            if (_currentCommand.Previous != null)
            {
                --_position;
                _currentCommand.Value.UnExecute();
                _currentCommand = _currentCommand.Previous;
                _presenter.Present(_extendedEvents[_currentCommand.Value]);
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
        private bool KeyEventIsFound(Func<LinkedListNode<ICommand>, LinkedListNode<ICommand>> switchCommand)
        {
            var command = switchCommand(_currentCommand);
            bool isKey = false;
            while (!isKey && command != null)
            {
                // ReSharper disable once AssignmentInConditionalExpression
                if (isKey = _extendedEvents[command.Value].Special)
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
                while (Next(needRepaint: false) && !_extendedEvents[_currentCommand.Value].Special) { }
                Repaint();
                return true;
            }
            return false;
        }

        public bool PrevKeyEvent()
        {
            if (KeyEventIsFound(c => c.Previous))
            {
                while (Previous(needRepaint: false) && !_extendedEvents[_currentCommand.Value].Special) { }
                Repaint();
                return true;
            }
            return false;
        }

        private IEnumerator ProcessEvents()
        {
            // Let other objects initialize
            yield return new WaitForSeconds(0.5f);
            
            ElektronikLogger.OpenLog();
            Application.logMessageReceived += ElektronikLogger.Log;
            Debug.Log("Parsing file...");
            using (var input = File.OpenRead(SettingsBag.Current[SettingName.FilePath].As<string>()))
            {
                var commands = new LinkedList<ICommand>();
                while (input.Position != input.Length)
                {
                    var packet = PacketPb.Parser.ParseDelimitedFrom(input);
                    try
                    {
                        _commander.GetCommands(packet, in commands);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message);
                        break;
                    }
                    foreach (var command in commands)
                        _extendedEvents[command] = packet;
                    _commands.MoveFrom(commands);
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
