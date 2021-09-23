﻿using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Data.Converters;
using Elektronik.Data.PackageObjects;
using Elektronik.DataConsumers;
using Elektronik.DataConsumers.Windows;
using Elektronik.DataSources;
using Elektronik.DataSources.SpecialInterfaces;
using Elektronik.Plugins.Common.DataDiff;
using Elektronik.Protobuf.Data;
using Elektronik.Threading;
using Elektronik.UI.Windows;

namespace Elektronik.Protobuf.Offline.Presenters
{
    public class SlamDataInfoPresenter : ISourceTreeNode, IRendersToWindow
    {
        public SlamDataInfoPresenter(string displayName)
        {
            DisplayName = displayName;
        }

        public void Present(PacketPb data, ICSConverter? converter)
        {
            MainThreadInvoker.Enqueue(() => _info?.Clear());
            IEnumerable<ICloudItem> objects = Pkg2Pts(data, converter).ToArray();
            MainThreadInvoker.Enqueue(() => _info?.Render((data.Message, objects)));
        }

        #region ISourceTreeNode

        public ISourceTreeNode? TakeSnapshot() => null;

        public string DisplayName { get; set; }
        public IEnumerable<ISourceTreeNode> Children { get; } = Array.Empty<ISourceTreeNode>();
        
        public void AddConsumer(IDataConsumer consumer)
        {
            if (consumer is WindowsManager factory)
            {
                factory.CreateWindow<SlamInfoRenderer>(DisplayName, (renderer, window) =>
                {
                    _info = renderer;
                    Window = window;
                });
            }
        }

        public void RemoveConsumer(IDataConsumer consumer)
        {
            if (_info != consumer) return;
            _info = null;
            Window = null;
        }

        public void Clear()
        {
            MainThreadInvoker.Enqueue(() => _info?.Clear());
        }

        #endregion

        #region IRendersToWindow

        public Window? Window { get; private set; }
        public string? Title { get; set; }

        #endregion

        #region Private

        private IDataRenderer<(string info, IEnumerable<ICloudItem> objects)>? _info;

        private IEnumerable<ICloudItem> Pkg2Pts(PacketPb packet, ICSConverter? converter)
        {
            switch (packet.DataCase)
            {
            case PacketPb.DataOneofCase.Points:
                foreach (var pointPb in packet.Points.Data)
                {
                    var point = ((SlamPointDiff)pointPb).Apply();
                    converter?.Convert(ref point.Position);
                    yield return point;
                }
                break;
            case PacketPb.DataOneofCase.Observations:
                foreach (var observationPb in packet.Observations.Data)
                {
                    var observation = ((SlamObservationDiff)observationPb).Apply();
                    converter?.Convert(ref observation.Point.Position, ref observation.Rotation);
                    yield return observation;
                }
                break;
            default:
                yield break;
            }
        }

        #endregion
    }
}