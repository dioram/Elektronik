using UnityEngine;

namespace Elektronik.Data.PackageObjects
{
    public struct SimpleLine : ICloudItem
    {
        public int Id { get; set; }
        public string Message { get; set; }

        public Vector3 BeginPos;
        public Vector3 EndPos;
        public Color BeginColor;
        public Color EndColor;

        public SimpleLine(int id, Vector3 beginPos, Vector3 endPos, Color beginColor, Color endColor) : this()
        {
            Id = id;
            BeginPos = beginPos;
            EndPos = endPos;
            BeginColor = beginColor;
            EndColor = endColor;
        }

        public SimpleLine(int id, Vector3 beginPos, Vector3 endPos, Color color = default) : this()
        {
            Id = id;
            BeginPos = beginPos;
            EndPos = endPos;
            BeginColor = color;
            EndColor = color;
        }

        public SlamPoint AsPoint() => new SlamPoint(Id, BeginPos, BeginColor, Message);
    }
}