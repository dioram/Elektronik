using System.Collections.Generic;
using System.Linq;
using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Clusterization.Algorithms
{
    public class Simple3PlaneClusterization : IClusterizationAlgorithm
    {
        public Simple3PlaneClusterization(Vector3 offset)
        {
            _offset = offset;
        }

        public List<List<SlamPoint>> Compute(IEnumerable<SlamPoint> items)
        {
            var l = items.ToList();
            var l1 = l.Where(i => i.Position.x > _offset.x
                                     && i.Position.y > _offset.y
                                     && i.Position.z > _offset.z).ToList();
            var l2 = l.Where(i => i.Position.x > _offset.x
                                     && i.Position.y > _offset.y
                                     && i.Position.z < _offset.z).ToList();
            var l3 = l.Where(i => i.Position.x > _offset.x
                                     && i.Position.y < _offset.y
                                     && i.Position.z > _offset.z).ToList();
            var l4 = l.Where(i => i.Position.x > _offset.x
                                     && i.Position.y < _offset.y
                                     && i.Position.z < _offset.z).ToList();
            var l5 = l.Where(i => i.Position.x < _offset.x
                                     && i.Position.y > _offset.y
                                     && i.Position.z > _offset.z).ToList();
            var l6 = l.Where(i => i.Position.x < _offset.x
                                     && i.Position.y > _offset.y
                                     && i.Position.z < _offset.z).ToList();
            var l7 = l.Where(i => i.Position.x < _offset.x
                                     && i.Position.y < _offset.y
                                     && i.Position.z > _offset.z).ToList();
            var l8 = l.Where(i => i.Position.x < _offset.x
                                     && i.Position.y < _offset.y
                                     && i.Position.z < _offset.z).ToList();

            var res = new List<List<SlamPoint>> {l1, l2, l3, l4, l5, l6, l7, l8};
            for (int i = 0; i < res.Count; i++)
            {
                for (int j = 0; j < res[i].Count; j++)
                {
                    var slamPoint = res[i][j];
                    slamPoint.Color = _colors[i];
                    res[i][j] = slamPoint;
                }
            }

            return res;
        }

        private readonly Vector3 _offset;

        private readonly Color[] _colors =
        {
            Color.blue,
            Color.red,
            Color.green,
            Color.yellow,
            Color.magenta,
            new Color(0.5f, 0.5f, 1f),
            new Color(1f, 0.5f, 0),
            new Color(0.5f, 0, 1)
        };
    }
}