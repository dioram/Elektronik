﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elektronik.Common.Clouds
{
    public struct CloudLine : ICloudItem
    {
        public static CloudLine Empty(int id) => new CloudLine(id, CloudPoint.Empty(0), CloudPoint.Empty(0));

        public int Id { get; set; }
        public CloudPoint pt1;
        public CloudPoint pt2;

        public CloudLine(int id, CloudPoint pt1, CloudPoint pt2)
        {
            this.Id = id;
            this.pt1 = pt1;
            this.pt2 = pt2;
        }

    }
}