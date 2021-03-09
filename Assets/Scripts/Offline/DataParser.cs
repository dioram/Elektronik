using Elektronik.Commands;
using Elektronik.Data.Converters;

namespace Elektronik.Offline
{
    /// <summary>
    /// Base class for parse data in offline mode. Used in pattern "Chain of responsibility".
    /// </summary>
    public abstract class DataParser<T> : IChainable<DataParser<T>>
    {
        protected DataParser<T> Successor;
        protected ICSConverter Converter;

        public IChainable<DataParser<T>> SetSuccessor(IChainable<DataParser<T>> parser)
        {
            return Successor = parser as DataParser<T>;
        }

        /// <summary> Sets converter for this parser and its successors. </summary>
        /// <param name="converter"> Converter to set. </param>
        public virtual void SetConverter(ICSConverter converter)
        {
            Converter = converter;
            Successor?.SetConverter(converter);
        }

        /// <summary> Extracts command form packet. </summary>
        /// <param name="pkg"> Packet with command. </param>
        public virtual ICommand GetCommand(T pkg)
        {
            return Successor?.GetCommand(pkg);
        }
    }
}