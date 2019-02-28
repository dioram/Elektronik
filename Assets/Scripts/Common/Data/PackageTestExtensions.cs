using Elektronik.Common;
using Elektronik.Common.Containers;
using Elektronik.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Elektronik.Common.Data
{
    public static class PackageTestExtensions
    {
        public static void Test(
            Package package,
            IEnumerable<SlamPoint> objects,
            string messageTemplate)
        {
            var objArray = objects.ToArray();
            if (objArray.Length != 0)
            {
                ElektronikLogger.Log(package.Summary(), "", LogType.Error);
                foreach (var obj in objArray)
                {
                    string msg = String.Format(messageTemplate, obj.id, obj.isNew, obj.isRemoved);
                    ElektronikLogger.Log(msg, "", LogType.Error);
                }
            }
        }

        private static IEnumerable<SlamPoint> SlamObjectQuery(IEnumerable<SlamPoint> src, Func<int, bool> cond) 
        {
            IEnumerable<SlamPoint> query = src
                .Where(p => cond(p.id));
            return query;
        }

        public static void TestExistent(
            this Package package,
            Func<SlamPoint, bool> filter,
            ISlamContainer<SlamPoint> pointsContainer,
            SlamObservationsGraph graph)
        {
            if (package.Points != null)
            {
                var existentPoints = SlamObjectQuery(package.Points.Where(p => filter(p)), id => pointsContainer.Exists(id));
                Test(package, existentPoints, "Point id {0} already exists. New: '{1}'. Removed: '{2}'");
            }
            if (package.Observations != null)
            {
                var existentObs = SlamObjectQuery(package.Observations.Select(o => (SlamPoint)o).Where(o => filter(o)), id => graph.ObservationExists(id));
                Test(package, existentObs, "Observation id {0} already exists. New: '{1}'. Removed: '{2}'");
            }
        }

        public static void TestNonExistent(
            this Package package,
            Func<SlamPoint, bool> filter,
            ISlamContainer<SlamPoint> pointsContainer,
            SlamObservationsGraph graph)
        {
#if DEBUG
            if (package.Points != null)
            {
                var existentPoints = SlamObjectQuery(package.Points.Where(p => filter(p)), id => !pointsContainer.Exists(id));
                ElektronikLogger.WrapDebug(() => Test(package, existentPoints, "Point id {0} doesn't exists. New: '{1}'. Removed: '{2}'"));
            }
            if (package.Observations != null)
            {
                var existentObs = SlamObjectQuery(package.Observations.Select(o => (SlamPoint)o).Where(o => filter(o)), id => !graph.ObservationExists(id));
                ElektronikLogger.WrapDebug(() => Test(package, existentObs, "Observation id {0} doesn't exists. New: '{1}'. Removed: '{2}'"));
            }
#endif
        }
    }
}
