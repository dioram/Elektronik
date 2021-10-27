using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.DataConsumers;
using Elektronik.DataConsumers.Windows;
using Elektronik.DataObjects;
using Elektronik.DataSources;
using Elektronik.DataSources.SpecialInterfaces;
using Elektronik.Plugins.Common;
using Elektronik.Protobuf.Data;
using Elektronik.UI.Windows;

namespace Elektronik.Protobuf.Offline.Presenters
{
    public class SlamDataInfoPresenter : IRendersToWindow
    {
        public SlamDataInfoPresenter(string displayName)
        {
            DisplayName = displayName;
        }

        public void Present(PacketPb data, ICSConverter? converter)
        {
            _info?.Clear();
            var objects = Pkg2Pts(data, converter).ToArray();
            _info?.Render((data.Message, objects));
        }

        #region IRendersToWindow

        public IDataSource? TakeSnapshot() => null;

        public string DisplayName { get; set; }
        public IEnumerable<IDataSource> Children { get; } = Array.Empty<IDataSource>();

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
            _info?.Clear();
        }

        public Window? Window { get; private set; }

        #endregion

        #region Private

        private IDataRenderer<(string info, IEnumerable<ICloudItem> objects)>? _info;

        private IEnumerable<ICloudItem> Pkg2Pts(PacketPb packet, ICSConverter? converter)
        {
            return packet.DataCase switch
            {
                PacketPb.DataOneofCase.Points => packet.ExtractPoints(converter)
                        .Select(d => d.Apply())
                        .OfType<ICloudItem>(),
                PacketPb.DataOneofCase.Observations => packet.ExtractObservations(converter, "")
                        .Select(d => d.Apply())
                        .OfType<ICloudItem>(),
                _ => Array.Empty<ICloudItem>()
            };
        }

        #endregion
    }
}