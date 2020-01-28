using Elektronik.Common.Data;
using Elektronik.Common.Data.Packages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elektronik.Common.Data.Packages.SlamActionPackages
{
    public class ActionDataPackage<T> : ISlamActionPackage
    {
        public ActionType ActionType { get; }
        public PackageType PackageType { get; }
        public int Timestamp { get; }
        public bool IsKey { get; }
        public ObjectType ObjectType { get; }
        public T[] Objects { get; }

        public ActionDataPackage(
            ObjectType objectType, 
            ActionType actionType, 
            PackageType packageType, 
            int timestamp, 
            bool isKey, 
            T[] objects)
        {
            ObjectType = objectType;
            ActionType = actionType;
            PackageType = packageType;
            Timestamp = timestamp;
            IsKey = isKey;
            Objects = objects;
        }

        public override string ToString()
        {
            return 
                $"Package type: {PackageType}{Environment.NewLine}" +
                $"Action type: {ActionType}{Environment.NewLine}" +
                $"Object type: {ObjectType}{Environment.NewLine}" +
                $"Count of {ObjectType}s ({nameof(T)}): {Objects.Length}{Environment.NewLine}";
        }
    }
}
