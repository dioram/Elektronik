using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Elektronik.DataConsumers.Collision
{
    /// <summary> Hierarchical AABB-block of collision objects. </summary>
    public class CollisionBlock
    {
        /// <summary> Constructs new block with given center and edge size. </summary>
        /// <param name="center"> Center of new block. </param>
        /// <param name="edgeSize"> Size of new block's edge. </param>
        public CollisionBlock(Vector3Int center, int edgeSize = int.MaxValue)
        {
            _center = center;
            _edgeSize = edgeSize;
        }

        /// <summary> Add new item in this block. </summary>
        /// <param name="id"> Item's id. </param>
        /// <param name="pos"> Item's position. </param>
        public void AddItem(int id, Vector3 pos)
        {
            if (_edgeSize == 1)
            {
                _items.Add((id, pos));
                return;
            }

            var childSide = _edgeSize == int.MaxValue ? TopCubeSize : _edgeSize / 3;
            var center = GetBlockPos(pos, childSide);
            if (!_children.ContainsKey(center))
            {
                _children.Add(center, new CollisionBlock(center, childSide));
            }

            _children[center].AddItem(id, pos);
        }

        /// <summary> Update item. </summary>
        /// <param name="id"> Item's id. </param>
        /// <param name="before"> Position of item before updating. </param>
        /// <param name="after"> Position of item after updating. </param>
        public void UpdateItem(int id, Vector3 before, Vector3 after)
        {
            RemoveItem(id, before);
            AddItem(id, after);
        }

        /// <summary> Removes item from this block. </summary>
        /// <param name="id"> Item's id. </param>
        /// <param name="pos"> Item's position. </param>
        public void RemoveItem(int id, Vector3 pos)
        {
            if (_edgeSize == 1)
            {
                _items.RemoveAll(item => item.id == id);
                return;
            }

            int childSide = _edgeSize == int.MaxValue ? TopCubeSize : _edgeSize / 3;
            var center = GetBlockPos(pos, childSide);
            if (!_children.ContainsKey(center)) return;

            _children[center].RemoveItem(id, pos);
            if (_children[center]._children.Count == 0 && _children[center]._items.Count == 0)
            {
                _children.Remove(center);
            }
        }

        /// <summary> Clear this block. </summary>
        public void Clear()
        {
            _children.Clear();
        }

        /// <summary> Tries to fine object collided with given ray in this block. </summary>
        /// <param name="ray"> Collision ray. </param>
        /// <param name="radius"> Radius of object. </param>
        /// <returns> Object's id or null if no collided objects was found. </returns>
        public int? FindItem(Ray ray, float radius)
        {
            var blocks = FindBlocks(ray).ToList();
            blocks.Sort((i, j) => Comparer<float>.Default.Compare(i.distance, j.distance));

            int? minId = null;
            var minDistance = float.MaxValue;
            foreach (var (block, _) in blocks)
            {
                foreach (var (id, pos) in block._items)
                {
                    if (CollisionAlgorithms.RaySphere(ray, pos, radius) 
                        && (pos - ray.origin).magnitude < minDistance)
                    {
                        minId = id;
                        minDistance = (pos - ray.origin).magnitude;
                    }
                }
            }

            return minId;
        }

        #region Private

        private const int TopCubeSize = 3 * 3 * 3 * 3 * 3 * 3;
        private const float Oversize = 0.2f;

        private readonly Dictionary<Vector3Int, CollisionBlock> _children
                = new Dictionary<Vector3Int, CollisionBlock>();

        private readonly List<(int id, Vector3 pos)> _items = new List<(int, Vector3)>();
        private readonly int _edgeSize;
        private readonly Vector3Int _center;

        private IEnumerable<(CollisionBlock block, float distance)> FindBlocks(Ray ray)
        {
            var leftBottom = _center - Vector3.one * (_edgeSize / 2.0f + Oversize);
            var rightTop = _center + Vector3.one * (_edgeSize / 2.0f + Oversize);
            var distance = CollisionAlgorithms.RayAABB(ray, leftBottom, rightTop);

            if (distance < 0) return Array.Empty<(CollisionBlock, float)>();
            if (_children.Count == 0) return new[] {(this, distance)};
            return _children.Values.SelectMany(ch => ch.FindBlocks(ray));
        }

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