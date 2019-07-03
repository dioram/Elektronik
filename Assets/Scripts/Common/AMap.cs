using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Common
{
    public abstract class AMap : MonoBehaviour
    {
        public abstract void Repaint();
        public abstract void Clear();
    }
}
