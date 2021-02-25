using Elektronik.Common;
using Elektronik.Common.Commands;
using Elektronik.Common.Data.Converters;
using Elektronik.Common.Data.Pb;

namespace Elektronik.ProtobufPlugin.Offline.Parsers
{
    /// <summary>
    /// Base class for parse data in offline mode. Used in pattern "Chain of responsibility".
    /// </summary>
    public abstract class PackageParser : IChainable<PackageParser>
    {
        protected PackageParser Successor;
        protected ICSConverter Converter;
        public IChainable<PackageParser> SetSuccessor(IChainable<PackageParser> parser) => Successor = parser as PackageParser;

        /// <summary> Sets converter for this parser and its successors. </summary>
        /// <param name="converter"> Converter to set. </param>
        public virtual void SetConverter(ICSConverter converter)
        {
            Converter = converter;
            Successor?.SetConverter(converter);
        }
            
        /// <summary> Extracts command form packet. </summary>
        /// <param name="pkg"> Packet with command. </param>
        public virtual ICommand GetCommand(PacketPb pkg)
        {
            return Successor?.GetCommand(pkg);
        }
    }
}
