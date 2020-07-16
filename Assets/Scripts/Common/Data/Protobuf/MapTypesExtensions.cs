using Elektronik.Common.Data.Converters;
using Elektronik.Common.Data.PackageObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx.Operators;
using UnityEngine;

namespace Elektronik.Common.Data.Pb
{
    public static class PacketPbExtensions
    {
        public static IEnumerable<SlamPoint> ExtractPoints(this PacketPb packet, ICSConverter converter = null)
        {
            Debug.Assert(packet.DataCase == PacketPb.DataOneofCase.Points);
            var stub = Quaternion.identity;
            foreach (var p in packet.Points.Data)
            {
                SlamPoint point = p;
                converter?.Convert(ref point.position, ref stub);
                yield return point;
            }
        }

        public static IEnumerable<SlamObservation> ExtractObservations(this PacketPb packet, ICSConverter converter = null)
        {
            Debug.Assert(packet.DataCase == PacketPb.DataOneofCase.Observations);
            foreach (var o in packet.Observations.Data)
            {
                SlamObservation observation = o;
                converter?.Convert(ref observation.point.position, ref observation.rotation);
                yield return observation;
            }
        }

        public static IEnumerable<SlamTrackedObject> ExtractTrackedObjects(this PacketPb packet, ICSConverter converter = null)
        {
            Debug.Assert(packet.DataCase == PacketPb.DataOneofCase.TrackedObjs);
            foreach (var o in packet.TrackedObjs.Data)
            {
                SlamTrackedObject trackedObject = o;
                converter?.Convert(ref trackedObject.position, ref trackedObject.rotation);
                yield return trackedObject;
            }
        }

        public static IEnumerable<SlamLine> ExtractLines(this PacketPb packet, ICSConverter converter = null)
        {
            Debug.Assert(packet.DataCase == PacketPb.DataOneofCase.Lines);
            var stub = Quaternion.identity;
            foreach (var l in packet.Lines.Data)
            {
                SlamLine line = l;
                converter?.Convert(ref line.pt1.position, ref stub);
                converter?.Convert(ref line.pt2.position, ref stub);
                yield return line;
            }
        }
    }

    public partial class Vector3Pb
    {
        public static implicit operator Vector3(Vector3Pb v) 
            => v != null ? new Vector3((float)v.X, (float)v.Y, (float)v.Z) : Vector3.zero;
    }
    public partial class Vector4Pb
    {
        public static implicit operator Quaternion(Vector4Pb v)
            => v != null ? new Quaternion((float)v.X, (float)v.Y, (float)v.Z, (float)v.W) : Quaternion.identity;
    }
    public partial class ColorPb
    {
        public static implicit operator Color32(ColorPb c)
            => c != null ? new Color32((byte)c.R, (byte)c.G, (byte)c.B, 255) : new Color32(0, 0, 0, 255);
        public static implicit operator Color(ColorPb c) 
            => (Color32)c;
    }
    public partial class PointPb
    {
        public static implicit operator SlamPoint(PointPb p)
            => p != null ? new SlamPoint() { id = p.id_, position = p.position_, color = p.color_, message = p.message_ } : default;
    }
    public partial class LinePb
    {
        public static implicit operator SlamLine(LinePb c)
            => c != null ? new SlamLine(c.pt1_, c.pt2_) : default;
    }

    public partial class ObservationPb
    {
        public partial class Types
        {
            public partial class Stats
            {
                public static implicit operator SlamObservation.Stats(Stats s)
                    => default; // TODO: make statistics
            }
        }

        public static implicit operator SlamObservation(ObservationPb o)
            => o != null ? new SlamObservation(o.point_, o.orientation_, o.stats_) : default;
    }

    public partial class TrackedObjPb
    {
        public static implicit operator SlamTrackedObject(TrackedObjPb o)
            => o != null ? new SlamTrackedObject(o.id_, o.trackColor_, o.translation_, o.rotation_) : default;
    }
}
