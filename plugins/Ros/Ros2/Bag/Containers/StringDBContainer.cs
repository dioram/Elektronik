﻿using System.Collections.Generic;
using Elektronik.DataConsumers.Windows;
using Elektronik.DataSources;
using Elektronik.RosPlugin.Ros2.Bag.Data;
using RosSharp.RosBridgeClient.MessageTypes.Std;
using SQLite;

namespace Elektronik.RosPlugin.Ros2.Bag.Containers
{
    public class StringDBContainer : DBContainerToWindow<String, StringRenderer, string>
    {
        public StringDBContainer(string displayName, List<SQLiteConnection> dbModels, Topic topic,
                                 List<long> actualTimestamps)
                : base(displayName, dbModels, topic, actualTimestamps)
        {
        }

        public override ISourceTreeNode? TakeSnapshot() => null;

        protected override string ToRenderType(String message)
        {
            return message.data;
        }
    }
}