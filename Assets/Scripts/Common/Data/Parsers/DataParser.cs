using UnityEngine;
using Elektronik.Common.Data.Converters;
using Elektronik.Common.Data.Packages;

namespace Elektronik.Common.Data.Parsers
{
    public abstract class DataParser : IChainable<IParser>, IParser
    {
        protected ICSConverter m_converter;
        protected IParser m_successor;
        public DataParser(ICSConverter converter)
        {
            m_converter = converter;
        }
        public IChainable<IParser> SetSuccessor(IChainable<IParser> parser)
        {
            Debug.Assert(parser != this, "[DataParser.SetSuccessor] Cyclic reference!");
            m_successor = parser as IParser;
            return parser;
        }
        public abstract IPackage Parse(byte[] data, int startIdx, ref int offset);
    }
}
