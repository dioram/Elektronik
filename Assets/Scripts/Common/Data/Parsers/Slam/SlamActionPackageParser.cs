using Elektronik.Common.Data.Packages.SlamActionPackages;
using Elektronik.Common.Data.Converters;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Packages;
using Elektronik.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Common.Data.Parsers.Slam
{
    public abstract class SlamActionPackageParser : DataParser
    {
        public SlamActionPackageParser(ICSConverter converter) : base(converter) { }
        protected static readonly int MAX_MESSAGE_LENGTH_IN_BYTES = 128;
        protected struct Header
        {
            public PackageType PackageType { get; set; }
            public int Timestamp { get; set; }
            public bool IsKey { get; set; }
            public ObjectType ObjectType { get; set; }
            public ActionType ActionType { get; set; }
        }
        protected Header ParseHeader(ref byte[] data, int startIdx, ref int offset)
        {
            offset = startIdx;
            return new Header()
            {
                PackageType = (PackageType)data[offset++],
                Timestamp = BitConverterEx.ToInt32(data, offset, ref offset),
                IsKey = BitConverterEx.ToBoolean(data, offset, ref offset),
                ObjectType = (ObjectType)data[offset++],
                ActionType = (ActionType)data[offset++],
            };
        }
    }
}
