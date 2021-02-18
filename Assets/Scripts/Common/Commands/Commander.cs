using Elektronik.Common.Data.Converters;
using Elektronik.Common.Data.Pb;
using System.Collections.Generic;
using UnityEngine;

namespace Elektronik.Common.Commands
{
    /// <summary>
    /// Base class for handle data in offline mode. Used in pattern "Chain of responsibility".
    /// </summary>
    public abstract class Commander : MonoBehaviour, IChainable<Commander>
    {
        protected Commander Successor;
        protected ICSConverter Converter;
        public IChainable<Commander> SetSuccessor(IChainable<Commander> commander) => Successor = commander as Commander;

        /// <summary> Sets converter for this commander and its successors. </summary>
        /// <param name="converter"> Converter to set. </param>
        public virtual void SetConverter(ICSConverter converter)
        {
            Converter = converter;
            if (Successor != null) Successor.SetConverter(converter);
        }
            
        /// <summary> Extracts commands form packet. </summary>
        /// <param name="pkg"> Packet with commands. </param>
        /// <param name="commands"> List where new commands will be added to. </param>
        public virtual void GetCommands(PacketPb pkg, in LinkedList<ICommand> commands)
        {
            if (Successor != null) Successor.GetCommands(pkg, in commands);
        }
    }
}
