using System.Collections.Generic;
using System.IO;
using Elektronik.Data.Converters;
using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Protobuf.Data
{
    public static class PacketPbExtensions
    {
        public static IEnumerable<SlamPoint> ExtractPoints(this PacketPb packet, ICSConverter converter = null)
        {
            foreach (var p in packet.Points.Data)
            {
                SlamPoint point = p;
                converter?.Convert(ref point.Position);
                yield return point;
            }
        }

        public static IEnumerable<SlamObservation> ExtractObservations(this PacketPb packet,
                                                                       ICSConverter converter,
                                                                       string imageDir)
        {
            foreach (var o in packet.Observations.Data)
            {
                SlamObservation observation = o;
                converter?.Convert(ref observation.Point.Position, ref observation.Rotation);
                if (!Path.IsPathRooted(observation.FileName))
                {
                    observation.FileName = Path.Combine(imageDir, observation.FileName);
                }

                yield return observation;
            }
        }

        public static IEnumerable<SlamTrackedObject> ExtractTrackedObjects(this PacketPb packet,
                                                                           ICSConverter converter = null)
        {
            foreach (var o in packet.TrackedObjs.Data)
            {
                SlamTrackedObject trackedObject = o;
                converter?.Convert(ref trackedObject.Position, ref trackedObject.Rotation);
                yield return trackedObject;
            }
        }

        public static IEnumerable<SlamLine> ExtractLines(this PacketPb packet, ICSConverter converter = null)
        {
            foreach (var l in packet.Lines.Data)
            {
                SlamLine line = l;
                converter?.Convert(ref line.Point1.Position);
                converter?.Convert(ref line.Point2.Position);
                yield return line;
            }
        }

        public static IEnumerable<SlamInfinitePlane> ExtractInfinitePlanes(this PacketPb packet,
                                                                           ICSConverter converter = null)
        {
            foreach (var p in packet.InfinitePlanes.Data)
            {
                SlamInfinitePlane plane = p;
                converter?.Convert(ref plane.Offset);
                converter?.Convert(ref plane.Normal);
                yield return plane;
            }
        }
    }

    public partial class Vector3Pb
    {
        public static implicit operator Vector3(Vector3Pb v)
            => v != null ? new Vector3((float) v.X, (float) v.Y, (float) v.Z) : Vector3.zero;

        public static implicit operator Vector3Pb(Vector3 v)
            => new Vector3Pb {X = v.x, Y = v.y, Z = v.z};
    }

    public partial class Vector4Pb
    {
        public static implicit operator Quaternion(Vector4Pb v)
            => v != null ? new Quaternion((float) v.X, (float) v.Y, (float) v.Z, (float) v.W) : Quaternion.identity;

        public static implicit operator Vector4Pb(Quaternion v) => new Vector4Pb {X = v.x, Y = v.y, Z = v.z, W = v.w};
    }

    public partial class ColorPb
    {
        public static implicit operator Color32(ColorPb c)
            => c != null ? new Color32((byte) c.R, (byte) c.G, (byte) c.B, 255) : new Color32(0, 0, 0, 255);

        public static implicit operator Color(ColorPb c)
            => (Color32) c;

        public static implicit operator ColorPb(Color c) => (Color32) c;

        public static implicit operator ColorPb(Color32 c) => new ColorPb {R = c.r, G = c.g, B = c.b,};
    }

    public partial class PointPb
    {
        public static implicit operator SlamPoint(PointPb p)
            => p != null
                    ? new SlamPoint() {Id = p.id_, Position = p.position_, Color = p.color_, Message = p.message_}
                    : default;

        public static implicit operator PointPb(SlamPoint p)
            => new PointPb {Id = p.Id, Position = p.Position, Color = p.Color, Message = p.Message ?? ""};
    }

    public partial class LinePb
    {
        public static implicit operator SlamLine(LinePb c)
            => c != null ? new SlamLine(c.pt1_, c.pt2_) : default;
        
        public static implicit operator LinePb(SlamLine l)
            => new LinePb {Pt1 = l.Point1, Pt2 = l.Point2};
    }

    public partial class ObservationPb
    {
        public partial class Types
        {
            public partial class Stats
            {
                public static implicit operator SlamObservation.Stats(Stats s)
                    => default; // TODO: make statistics

                public static implicit operator Stats(SlamObservation.Stats s)
                    => default; // TODO: make statistics
            }
        }

        public static implicit operator SlamObservation(ObservationPb o)
            => o != null ? new SlamObservation(o.point_, o.orientation_, o.message_, o.filename_, o.stats_) : default;

        public static implicit operator ObservationPb(SlamObservation o)
            => new ObservationPb
            {
                Point = o.Point, Orientation = o.Rotation, Message = o.Message, Filename = o.FileName,
                Stats = o.Statistics
            };
    }

    public partial class TrackedObjPb
    {
        public static implicit operator SlamTrackedObject(TrackedObjPb o)
            => o != null
                    ? new SlamTrackedObject(o.id_, o.translation_, o.rotation_, o.trackColor_, o.message_)
                    : default;

        public static implicit operator TrackedObjPb(SlamTrackedObject o)
            => new TrackedObjPb
            {
                Id = o.Id, Translation = o.Position, Rotation = o.Rotation, TrackColor = o.Color, Message = o.Message
            };
    }

    public partial class InfinitePlanePb
    {
        public static implicit operator SlamInfinitePlane(InfinitePlanePb p)
            => p != null
                    ? new SlamInfinitePlane
                            {Color = p.Color, Id = p.Id, Message = p.Message, Normal = p.Normal, Offset = p.Offset}
                    : default;

        public static implicit operator InfinitePlanePb(SlamInfinitePlane p)
            => new InfinitePlanePb {Color = p.Color, Id = p.Id, Message = p.Message, Normal = p.Normal, Offset = p.Offset};
    }
}