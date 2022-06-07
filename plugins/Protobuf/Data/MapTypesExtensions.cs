using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Elektronik.DataObjects;
using Elektronik.Plugins.Common;
using Elektronik.Plugins.Common.DataDiff;
using UnityEngine;

namespace Elektronik.Protobuf.Data
{
    public static class PacketPbExtensions
    {
        public static SlamPointDiff[] ExtractPoints(this PacketPb packet, ICSConverter? converter)
        {
            return packet.Points.Data.Select(p => p.ToUnity(converter)).ToArray();
        }

        public static SlamObservationDiff[] ExtractObservations(this PacketPb packet, ICSConverter? converter,
                                                                string imageDir)
        {
            var result = new SlamObservationDiff[packet.Observations.Data.Count];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = packet.Observations.Data[i].ToUnity(converter);

                if (!string.IsNullOrEmpty(result[i].FileName) && !Path.IsPathRooted(result[i].FileName))
                {
                    result[i].FileName = Path.Combine(imageDir, result[i].FileName!);
                }
            }

            return result;
        }

        public static SlamTrackedObjectDiff[] ExtractTrackedObjects(this PacketPb packet, ICSConverter? converter)
        {
            return packet.TrackedObjs.Data.Select(p => p.ToUnity(converter)).ToArray();
        }

        public static SlamLineDiff[] ExtractLines(this PacketPb packet, ICSConverter? converter)
        {
            return packet.Lines.Data.Select(p => p.ToUnity(converter)).ToArray();
        }

        public static SlamPlaneDiff[] ExtractPlanes(this PacketPb packet, ICSConverter? converter)
        {
            return packet.Planes.Data.Select(p => p.ToUnity(converter)).ToArray();
        }

        public static SlamMarkerDiff[] ExtractMarkers(this PacketPb packet, ICSConverter? converter)
        {
            return packet.Markers.Data.Select(p => p.ToUnity(converter)).ToArray();
        }

        public static byte[] ExtractImage(this PacketPb packetPb, string imageDir)
        {
            switch (packetPb.Image.ImageCase)
            {
            case ImagePb.ImageOneofCase.Bytes:
                return packetPb.Image.Bytes.ToByteArray();
            case ImagePb.ImageOneofCase.Path:
                string fullPath;
                var filename = packetPb.Image.Path;
                if (Path.IsPathRooted(filename) || string.IsNullOrEmpty(imageDir)) fullPath = filename;
                else fullPath = Path.Combine(imageDir, filename);
                return File.Exists(fullPath) ? File.ReadAllBytes(fullPath) : Array.Empty<byte>();
            default:
                return Array.Empty<byte>();
            }
        }
    }

    public partial class Vector3Pb
    {
        public Vector3 ToUnity(ICSConverter? converter)
        {
            var res = new Vector3((float)X, (float)Y, (float)Z);
            converter?.Convert(ref res);
            return res;
        }
    }

    public partial class Vector4Pb
    {
        public Quaternion ToUnity(ICSConverter? converter)
        {
            var res = new Quaternion((float)X, (float)Y, (float)Z, (float)W);
            converter?.Convert(ref res);
            return res;
        }
    }

    public partial class ColorPb
    {
        public Color32 ToUnity32()
        {
            return new Color32((byte)R, (byte)G, (byte)B, 255);
        }

        public Color ToUnity() => ToUnity32();

        public static ColorPb FromUnity32(Color32 color) => new ColorPb { R = color.r, G = color.g, B = color.b, };

        public static ColorPb FromUnity(Color color) => FromUnity32(color);
    }

    public partial class PointPb
    {
        public SlamPointDiff ToUnity(ICSConverter? converter) =>
                new(Id, Position?.ToUnity(converter), Color?.ToUnity(), Message);
    }

    public partial class LinePb
    {
        public SlamLineDiff ToUnity(ICSConverter? converter) =>
                new SlamLineDiff { Point1 = Pt1.ToUnity(converter), Point2 = Pt2.ToUnity(converter) };
    }

    public partial class ObservationPb
    {
        public SlamObservationDiff ToUnity(ICSConverter? converter) =>
                new(Point.Id, Point.Position?.ToUnity(converter), Point.Color?.ToUnity(),
                    Orientation?.ToUnity(converter), ObservedPoints, Message, Filename);
    }

    public partial class TrackedObjPb
    {
        public SlamTrackedObjectDiff ToUnity(ICSConverter? converter) =>
                new(Id, Position?.ToUnity(converter), Orientation?.ToUnity(converter), Color?.ToUnity(), Message);
    }

    public partial class PlanePb
    {
        public SlamPlaneDiff ToUnity(ICSConverter? converter) =>
                new(Id, Offset?.ToUnity(converter), Normal?.ToUnity(converter), Color?.ToUnity(), Message);
    }

    public partial class MarkerPb
    {
        public SlamMarkerDiff ToUnity(ICSConverter? converter) =>
                new(Id, Position?.ToUnity(converter), Orientation?.ToUnity(converter), Scale?.ToUnity(null),
                    Color?.ToUnity(), Message, GetPrimitive());

        private SlamMarker.MarkerType? GetPrimitive()
        {
            if (!HasPrimitive) return null;
            return Primitive switch
            {
                Types.Type.Sphere => SlamMarker.MarkerType.Sphere,
                Types.Type.Cube => SlamMarker.MarkerType.Cube,
                Types.Type.Crystal => SlamMarker.MarkerType.Crystal,
                Types.Type.SemitransparentCube => SlamMarker.MarkerType.SemitransparentCube,
                Types.Type.SemitransparentSphere => SlamMarker.MarkerType.SemitransparentSphere,
                _ => null,
            };
        }
    }

    public static class Conversions
    {
        public static Vector3Pb ToProtobuf(this Vector3 v, ICSConverter? converter = null)
        {
            if (converter is not null) v = converter.ConvertedBack(v);
            return new Vector3Pb { X = v.x, Y = v.y, Z = v.z };
        }

        public static Vector4Pb ToProtobuf(this Quaternion q, ICSConverter? converter = null)
        {
            if (converter is not null) q = converter.ConvertedBack(q);
            return new Vector4Pb { X = q.x, Y = q.y, Z = q.z, W = q.w };
        }

        public static PointPb ToProtobuf(this SlamPoint p, ICSConverter? converter = null)
            => new()
            {
                Id = p.Id,
                Position = p.Position.ToProtobuf(converter),
                Color = ColorPb.FromUnity(p.Color),
                Message = p.Message ?? ""
            };

        public static LinePb ToProtobuf(this SlamLine l, ICSConverter? converter = null)
            => new() { Pt1 = l.Point1.ToProtobuf(converter), Pt2 = l.Point2.ToProtobuf(converter) };

        public static ObservationPb ToProtobuf(this SlamObservation o, ICSConverter? converter = null)
        {
            var pb = new ObservationPb
            {
                Point = new PointPb
                {
                    Id = o.Id, Position = o.Position.ToProtobuf(converter), Color = ColorPb.FromUnity(o.Color)
                },
                Orientation = o.Rotation.ToProtobuf(converter),
                Message = o.Message ?? "",
                Filename = o.FileName ?? "",
            };
            pb.ObservedPoints.AddRange(o.ObservedPoints ?? new HashSet<int>());
            return pb;
        }

        public static TrackedObjPb ToProtobuf(this SlamTrackedObject o, ICSConverter? converter = null)
            => new()
            {
                Id = o.Id,
                Position = o.Position.ToProtobuf(converter),
                Orientation = o.Rotation.ToProtobuf(converter),
                Color = ColorPb.FromUnity(o.Color),
                Message = o.Message
            };

        public static PlanePb ToProtobuf(this SlamPlane p, ICSConverter? converter = null)
            => new()
            {
                Id = p.Id,
                Color = ColorPb.FromUnity(p.Color),
                Message = p.Message,
                Normal = p.Normal.ToProtobuf(converter),
                Offset = p.Offset.ToProtobuf(converter)
            };
    }
}