using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Common.Extensions
{
    public static class ObjectPoolExtensions
    {
        private static void SetActiveFalse(GameObject o, ObjectPool op)
        {
            o.SetActive(false);
        }

        private static void SetActiveTrue(GameObject o, ObjectPool op)
        {
            o.SetActive(true);
        }

        public static void SetActive(this ObjectPool pool, bool value)
        {
            if (value)
            {
                pool.OnObjectSpawn -= SetActiveFalse;
                pool.OnObjectSpawn += SetActiveTrue;
            }
            else
            {
                pool.OnObjectSpawn += SetActiveFalse;
                pool.OnObjectSpawn -= SetActiveTrue;
            }
            foreach (var obj in pool.ActiveObject)
                obj.SetActive(value);
        }
    }
}
