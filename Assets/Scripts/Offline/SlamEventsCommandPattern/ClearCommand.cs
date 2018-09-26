using Elektronik.Common;
using Elektronik.Offline.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Offline.SlamEventsCommandPattern
{
    class ClearCommand : MacroCommand
    {
        private SlamPoint[] m_undoPoints;
        private SlamObservation[] m_undoObservations;
        private SlamLine[] m_undoLines;
        private SlamObservationsGraph m_graph;
        private FastPointCloud m_pointCloud;
        private FastLinesCloud m_linesCloud;

        public ClearCommand(FastPointCloud pointCloud, FastLinesCloud linesCloud, SlamObservationsGraph graph)
        {
            m_graph = graph;
            m_pointCloud = pointCloud;
            m_linesCloud = linesCloud;

            m_undoPoints = GetUndoPoints(pointCloud);
            m_undoObservations = GetUndoObservations(graph);
            m_undoLines = GetUndoLines();

            m_commands.Add(new RepaintPointsCommand(pointCloud, m_undoPoints));
            m_commands.Add(new RepaintObservationsCommand(graph, m_undoObservations));
            m_commands.Add(new RepaintLinesCommand(linesCloud, pointCloud, m_undoLines));
        }

        private SlamLine[] GetUndoLines(FastLinesCloud linesCloud)
        {
            linesCloud.getli
        }

        private SlamObservation[] GetUndoObservations(SlamObservationsGraph graph)
        {
            return graph.GetAllObservations();
        }

        private SlamPoint[] GetUndoPoints(FastPointCloud pointCloud)
        {
            int[] indices;
            Vector3[] positions;
            Color[] colors;
            pointCloud.GetAllPoints(out indices, out positions, out colors);
            SlamPoint[] slamPoints = new SlamPoint[indices.Length];
            for (int slamPointNum = 0; slamPointNum < slamPoints.Length; ++slamPointNum)
            {
                slamPoints[slamPointNum] = new SlamPoint()
                {
                    id = indices[slamPointNum],
                    position = positions[slamPointNum],
                    color = colors[slamPointNum],
                    justColored = false,
                    isRemoved = false,
                };
            }
            return slamPoints;
        }

        public override void Execute()
        {
            m_graph.Clear();
            m_pointCloud.Clear();
            m_linesCloud.Clear();
        }
    }
}
