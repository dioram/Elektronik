using Elektronik.Common;
using Elektronik.Offline.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Offline.SlamEventsCommandPattern
{
    public class PostProcessingCommand : MacroCommand
    {
        public PostProcessingCommand(FastPointCloud pointCloud, FastLinesCloud linesCloud, ISlamEvent slamEvent)
        {
            if (slamEvent.Points != null)
            {
                SlamPoint[] points = GetPoints(slamEvent.Points.Where(p => p.id != -1).ToArray(), pointCloud);
                m_commands.Add(new RepaintPointsCommand(pointCloud, points));
            }
            if (slamEvent.Lines != null)
            {
                SlamLine[] lines = GetLines(slamEvent.Lines);
                m_commands.Add(new RepaintLinesCommand(linesCloud, pointCloud, lines));
            }
        }

        private SlamLine[] GetLines(SlamLine[] lines)
        {
            SlamLine[] dstLines = new SlamLine[lines.Length];
            for (int i = 0; i < dstLines.Length; ++i)
            {
                dstLines[i].isRemoved = true;
            }
            return dstLines;
        }

        private SlamPoint[] GetPoints(SlamPoint[] points, FastPointCloud pointCloud)
        {
            SlamPoint[] dstPoints = new SlamPoint[points.Length];
            for (int i = 0; i < dstPoints.Length; ++i)
            {
                dstPoints[i] = points[i];

                //dstPoints[i].position = Vector3.zero; // передвигать не надо, только раскрасить

                if (dstPoints[i].isRemoved)
                {
                    dstPoints[i].color = new Color(0, 0, 0, 0);
                }
                else
                {
                    dstPoints[i].color = Color.black;
                }
            }
            return dstPoints;
        }
    }
}
