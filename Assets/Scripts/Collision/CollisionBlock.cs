using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Elektronik.Collision
{
    public class CollisionBlock
    {
        public CollisionBlock(Vector3Int center, int sideSize = int.MaxValue)
        {
            _center = center;
            _sideSize = sideSize;
        }

        public void AddItem(int id, Vector3 pos)
        {
            if (_sideSize == 1)
            {
                _items.Add((id, pos));
                return;
            }

            int childSide = _sideSize == int.MaxValue ? TopCubeSize : _sideSize / 3;
            var center = GetBlockPos(pos, childSide);
            if (!_children.ContainsKey(center))
            {
                _children.Add(center, new CollisionBlock(center, childSide));
            }

            _children[center].AddItem(id, pos);
        }

        public void UpdateItem(int id, Vector3 before, Vector3 after)
        {
            RemoveItem(id, before);
            AddItem(id, after);
        }

        public void RemoveItem(int id, Vector3 pos)
        {
            if (_sideSize == 1)
            {
                _items.RemoveAll(item => item.id == id);
                return;
            }

            int childSide = _sideSize == int.MaxValue ? TopCubeSize : _sideSize / 3;
            var center = GetBlockPos(pos, childSide);
            if (!_children.ContainsKey(center)) return;

            _children[center].RemoveItem(id, pos);
            if (_children[center]._children.Count == 0 && _children[center]._items.Count == 0)
            {
                _children.Remove(center);
            }
        }

        public void Clear()
        {
            _children.Clear();
        }

        public int? FindItem(Ray ray, float radius)
        {
            var blocks = FindBlocks(ray).ToList();
            blocks.Sort((i, j) => Comparer<float>.Default.Compare(i.distance, j.distance));

            int? minId = null;
            float minDistance = float.MaxValue;
            foreach (var (block, _) in blocks)
            {
                foreach (var (id, pos) in block._items)
                {
                    if (CollisionAlgorithms.RaySphere(ray, pos, radius) 
                        && (pos - ray.origin).magnitude < minDistance)
                    {
                        minId = id;
                    }
                }
            }

            return minId;
        }

        #region Protected

        private const int TopCubeSize = 3 * 3 * 3 * 3 * 3 * 3;
        private const float Oversize = 0.2f;

        private IEnumerable<(CollisionBlock block, float distance)> FindBlocks(Ray ray)
        {
            var leftBottom = _center - Vector3.one * (_sideSize / 2.0f + Oversize);
            var rightTop = _center + Vector3.one * (_sideSize / 2.0f + Oversize);
            var distance = CollisionAlgorithms.RayAABB(ray, leftBottom, rightTop);

            if (distance < 0) return new (CollisionBlock, float)[0];
            if (_children.Count == 0) return new[] {(this, distance)};
            return _children.Values.SelectMany(ch => ch.FindBlocks(ray));
        }

        #endregion

        #region Private

        private readonly Dictionary<Vector3Int, CollisionBlock> _children
                = new Dictionary<Vector3Int, CollisionBlock>();

        private readonly List<(int id, Vector3 pos)> _items = new List<(int, Vector3)>();
        private readonly int _sideSize;
        private readonly Vector3Int _center;

        private Vector3Int GetBlockPos(Vector3 itemPos, int childSide)
        {
            return new Vector3Int(Round(itemPos.x, childSide),
                                  Round(itemPos.y, childSide),
                                  Round(itemPos.z, childSide));
        }

        private int Round(float value, float @base)
        {
            return (int) (Mathf.Round(value / @base) * @base);
        }

        #endregion
    }
}