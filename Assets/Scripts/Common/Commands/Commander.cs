using Elektronik.Common.Data.Converters;
using Elektronik.Common.Data.Pb;
using System.Collections.Generic;
using UnityEngine;

namespace Elektronik.Common.Commands
{
    public abstract class Commander : MonoBehaviour, IChainable<Commander>
    {
        protected Commander Successor;
        protected ICSConverter Converter;
        public IChainable<Commander> SetSuccessor(IChainable<Commander> commander) => Successor = commander as Commander;

        public virtual void SetConverter(ICSConverter converter)
        {
            Converter = converter;
            if (Successor != null) Successor.SetConverter(converter);
        }
            

        public virtual void GetCommands(PacketPb pkg, in LinkedList<ICommand> commands)
        {
            if (Successor != null) Successor.GetCommands(pkg, in commands);
        }
    }
}
