using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Common.Data
{
    public abstract class DataParser
    {
        protected ICSConverter m_converter;
        protected DataParser m_successor;

        public DataParser(ICSConverter converter)
        {
            m_converter = converter;
        }

        public void SetSuccessor(DataParser parser)
        {
            Debug.AssertFormat(parser != this, "[DataParser.SetSuccessor] Cyclic reference!");
            m_successor = parser;
        }
        public abstract int Parse(byte[] data, int startIdx, out IPackage result);
    }
}
