﻿using System.IO;
using System.Linq;
using Elektronik.Data.Converters;
using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Protobuf.Data
{
    public static class PacketPbExtensions
    {
        public static SlamPointDiff[] ExtractPoints(this PacketPb packet, ICSConverter? converter = null)
        {
            var result = new SlamPointDiff[packet.Points.Data.Count];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = packet.Points.Data[i];
                if (result[i].Position.HasValue) result[i].Position = converter?.Convert(result[i].Position.Value);
            }

            return result;
        }

        public static SlamObservationDiff[] ExtractObservations(this PacketPb packet,
                                                              ICSConverter? converter,
                                                              string imageDir)
        {
            var result = new SlamObservationDiff[packet.Observations.Data.Count];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = packet.Observations.Data[i];
                if (result[i].Point.Position.HasValue)
                {
                    result[i].Point.Position = converter?.Convert(result[i].Point.Position.Value);
                }

                if (result[i].Rotation.HasValue)
                {
                    result[i].Rotation = converter?.Convert(result[i].Rotation.Value);
                }

                if (!string.IsNullOrEmpty(result[i].FileName) 
                    && !Path.IsPathRooted(result[i].FileName))
                {
                    result[i].FileName = Path.Combine(imageDir, result[i].FileName);
                }
            }

            return result;
        }

        public static SlamTrackedObjectDiff[] ExtractTrackedObjects(this PacketPb packet,
                                                                  ICSConverter? converter = null)
        {
            var result = new SlamTrackedObjectDiff[packet.TrackedObjs.Data.Count];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = packet.TrackedObjs.Data[i];
                if (result[i].Position.HasValue)
                {
                    result[i].Position = converter?.Convert(result[i].Position.Value);
                }

                if (result[i].Rotation.HasValue)
                {
                    result[i].Rotation = converter?.Convert(result[i].Rotation.Value);
                }
            }

            return result;
        }

        public static SlamLineDiff[] ExtractLines(this PacketPb packet, ICSConverter? converter = null)
        {
            var result = new SlamLineDiff[packet.Lines.Data.Count];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = packet.Lines.Data[i];
                if (result[i].Point1.Position.HasValue)
                {
                    result[i].Point1.Position = converter?.Convert(result[i].Point1.Position.Value);
                }

                if (result[i].Point2.Position.HasValue)
                {
                    result[i].Point2.Position = converter?.Convert(result[i].Point2.Position.Value);
                }
            }

            return result;
        }

        public static SlamInfinitePlaneDiff[] ExtractInfinitePlanes(this PacketPb packet,
                                                                  ICSConverter? converter = null)
        {
            var result = new SlamInfinitePlaneDiff[packet.InfinitePlanes.Data.Count];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = packet.InfinitePlanes.Data[i];
                if (result[i].Offset.HasValue) result[i].Offset = converter?.Convert(result[i].Offset.Value);
                if (result[i].Normal.HasValue) result[i].Normal = converter?.Convert(result[i].Normal.Value);
            }

            return result;
        }
    }

    public partial class Vector3Pb
    {
        public static implicit operator Vector3?(Vector3Pb? v)
        {
            if (v == null) return null;
            return new Vector3((float) v.X, (float) v.Y, (float) v.Z);
        }
    }

    public partial class Vector4Pb
    {
        public static implicit operator Quaternion?(Vector4Pb? v)
        {
            if (v == null) return null;
            return new Quaternion((float) v.X, (float) v.Y, (float) v.Z, (float) v.W);
        }
    }

    public partial class ColorPb
    {
        public static implicit operator Color32?(ColorPb? c)
        {
            if (c == null) return null;
            return new Color32((byte) c.R, (byte) c.G, (byte) c.B, 255);
        }

        public static implicit operator Color?(ColorPb c) => (Color32?) c;

        public static implicit operator ColorPb(Color c) => (Color32) c;

        public static implicit operator ColorPb(Color32 c) => new ColorPb {R = c.r, G = c.g, B = c.b,};
    }

    public partial class PointPb
    {
        public static implicit operator SlamPointDiff(PointPb? p)
            => p != null
                    ? new SlamPointDiff {Id = p.id_, Position = p.position_, Color = p.color_, Message = p.message_}
                    : default;
    }

    public partial class LinePb
    {
        public static implicit operator SlamLineDiff(LinePb? c)
            => c != null ? new SlamLineDiff {Point1 = c.pt1_, Point2 = c.pt2_} : default;
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

        public static implicit operator SlamObservationDiff(ObservationPb? o)
            => o != null
                    ? new SlamObservationDiff
                    {
                        Point = o.point_,
                        Rotation = o.Orientation,
                        FileName = o.filename_,
                        Message = o.message_,
                        ObservedPoints = o.observedPoints_.ToArray(),
                    }
                    : default;
    }

    public partial class TrackedObjPb
    {
        public static implicit operator SlamTrackedObjectDiff(TrackedObjPb? o)
            => o != null
                    ? new SlamTrackedObjectDiff
                    {
                        Id = o.id_,
                        Position = o.position_,
                        Rotation = o.orientation_,
                        Color = o.color_,
                        Message = o.message_
                    }
                    : default;
    }

    public partial class InfinitePlanePb
    {
        public static implicit operator SlamInfinitePlaneDiff(InfinitePlanePb? p)
            => p != null
                    ? new SlamInfinitePlaneDiff
                    {
                        Id = p.Id,
                        Color = p.Color,
                        Message = p.Message,
                        Normal = p.Normal,
                        Offset = p.Offset
                    }
                    : default;
    }

    public static class Conversions
    {
        public static Vector3Pb ToProtobuf(this Vector3 v, ICSConverter? converter)
        {
            converter?.ConvertBack(ref v);
            return new Vector3Pb {X = v.x, Y = v.y, Z = v.z};
        }

        public static Vector4Pb ToProtobuf(this Quaternion q, ICSConverter? converter)
        {
            if (converter != null)
            {
                var v = Vector3.zero;
                converter.ConvertBack(ref v, ref q);
            }

            return new Vector4Pb {X = q.x, Y = q.y, Z = q.z, W = q.w};
        }

        public static PointPb ToProtobuf(this SlamPoint p, ICSConverter? converter)
            => new()
            {
                Id = p.Id, Position = p.Position.ToProtobuf(converter), Color = p.Color, Message = p.Message ?? ""
            };

        public static LinePb ToProtobuf(this SlamLine l, ICSConverter? converter)
            => new() {Pt1 = l.Point1.ToProtobuf(converter), Pt2 = l.Point2.ToProtobuf(converter)};

        public static ObservationPb ToProtobuf(this SlamObservation o, ICSConverter? converter)
        {
            var pb = new ObservationPb
            {
                Point = o.Point.ToProtobuf(converter),
                Orientation = o.Rotation.ToProtobuf(converter),
                Message = o.Message,
                Filename = o.FileName,
                Stats = o.Statistics
            };
            pb.ObservedPoints.AddRange(o.ObservedPoints);
            return pb;
        }

        public static TrackedObjPb ToProtobuf(this SlamTrackedObject o, ICSConverter? converter)
            => new()
            {
                Id = o.Id,
                Position = o.Position.ToProtobuf(converter),
                Orientation = o.Rotation.ToProtobuf(converter),
                Color = o.Color,
                Message = o.Message
            };

        public static InfinitePlanePb ToProtobuf(this SlamInfinitePlane p, ICSConverter? converter)
            => new()
            {
                Color = p.Color,
                Id = p.Id,
                Message = p.Message,
                Normal = p.Normal.ToProtobuf(converter),
                Offset = p.Offset.ToProtobuf(converter)
            };
    }
}