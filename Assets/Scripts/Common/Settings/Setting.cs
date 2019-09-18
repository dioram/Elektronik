using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Elektronik.Common.Settings
{
    [Serializable]
    public class Setting : ISerializable
    {
        public object UnknownValue
        {
            get;
            private set;
        }
        public string Name { get; }
        public Type Type { get; }

        /// <summary>
        /// This method may cause boxing/unboxing operations, don't use it frequently.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="throwException"></param>
        /// <returns></returns>
        public T As<T>(bool throwException = true)
        {
            if (throwException && !(Type.IsEquivalentTo(typeof(T))))
            {
                throw new InvalidCastException($"Cannot cast {Type.Name} to {nameof(T)}");
            }
            return (T)UnknownValue;
        }

        [JsonConstructor]
        private Setting(string name, object unknownValue, Type type)
        {
            UnknownValue = unknownValue;
            Name = name;
            Type = type;
        }

        protected Setting(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString("Name");
            Type = (Type)info.GetValue("Type", typeof(Type));
            UnknownValue = info.GetValue("Value", Type);
        }

        virtual public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("Type", Type);
            info.AddValue("Value", UnknownValue, Type);
        }

        public override string ToString() => UnknownValue.ToString();
        public static Setting Create<T>(string name, T value)
        {
            return new Setting(name, value, typeof(T));
        }
    }
}