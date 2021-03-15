using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Data.PackageObjects;
using Elektronik.Rosbag2.Parsers;
using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.MessageTypes.Geometry;
using RosSharp.RosBridgeClient.MessageTypes.Std;

namespace Elektronik.Rosbag2.RosMessages
{
    public class Marker : Message
    {
        public const string RosMessageName = "visualization_msgs/Marker";

        public enum MarkerForm
        {
            Arrow = 0,
            Cube = 1,
            Sphere = 2,
            Cylinder = 3,
            LineStrip = 4,
            LineList = 5,
            CubeList = 6,
            SphereList = 7,
            Points = 8,
            TextViewFacing = 9,
            MeshResource = 10,
            TriangleList = 11,
        }

        public enum MarkerAction
        {
            Add = 0,
            Modify = 0,
            Delete = 2,
            DeleteAll = 3,
        }

        public Header Header { get; set; }
        public string Ns { get; set; }
        public int Id { get; set; }
        public MarkerForm Form { get; set; }
        public MarkerAction Action { get; set; }
        public Pose Pose { get; set; }
        public Vector3 Scale { get; set; }
        public ColorRGBA Color { get; set; }
        public Time Lifetime { get; set; }
        public bool FrameLocked { get; set; }
        public Point[] Points { get; set; }
        public ColorRGBA[] Colors { get; set; }
        public string Text { get; set; }
        public string MeshResource { get; set; }
        public bool MeshUseEmbeddedMaterials { get; set; }

        public bool IsSimple => Form == MarkerForm.Sphere || Form == MarkerForm.Cube || Form == MarkerForm.Cylinder;
        public bool IsLines => Form == MarkerForm.LineList || Form == MarkerForm.LineStrip;
        public bool IsList => Form == MarkerForm.Points || Form == MarkerForm.CubeList || Form == MarkerForm.SphereList;

        public IEnumerable<SlamPoint> GetPoints()
        {
            if (IsSimple) return new[] {new SlamPoint(Id, Pose.position.ToUnity(), Color.ToUnity())};

            var pointColor = Colors.Length != 0;
            return Points
                    .Select((p, i) => new SlamPoint(i, p.ToUnity(),
                                                    (pointColor ? Colors[i] : Color).ToUnity()));
        }

        public SlamLine[] GetLines() => Form switch
        {
            MarkerForm.LineList => GetLinesList(),
            MarkerForm.LineStrip => GetStrip(),
            _ => throw new InvalidCastException("Can't get lines from this type of marker")
        };


        private SlamLine[] GetStrip()
        {
            var points = GetPoints().ToArray();
            var res = new SlamLine[points.Length - 1];
            for (int i = 0; i < points.Length - 1; i++)
            {
                res[i] = new SlamLine(points[i], points[i + 1], i) {Message = $"{Ns} {Id}"};
            }

            return res;
        }

        private SlamLine[] GetLinesList()
        {
            var points = GetPoints().ToArray();
            var res = new SlamLine[points.Length / 2];
            for (int i = 0; i < points.Length / 2; i += 2)
            {
                res[i] = new SlamLine(points[i], points[i + 1], i / 2) {Message = $"{Ns} {Id}"};
            }

            return res;
        }

        public long RemoveAt()
        {
            if (Lifetime.secs == 0 && Lifetime.nsecs == 0) return -1;
            return ((long) (Lifetime.secs + Header.stamp.secs) << 32) + Lifetime.nsecs + Header.stamp.nsecs;
        }
    }
}