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
            IEnumerable<ISlamObject> objects,
            string messageTemplate)
        {
            ElektronikLogger.Log(package.Summary(), "", LogType.Error);
            foreach (var obj in objects)
            {
                string msg = String.Format(messageTemplate, obj.id, obj.isNew, obj.isRemoved);
                ElektronikLogger.Log(msg, "", LogType.Error);
            }
        }

        private static IEnumerable<ISlamObject> SlamObjectQuery<T>(IEnumerable<T> src, Func<int, bool> cond) 
            where T : ISlamObject
        {
            IEnumerable<ISlamObject> query = src
                .Where(p => cond(p.id))
                .Cast<ISlamObject>();
            return query;
        }

        public static void TestExistent(
            this Package package,
            Func<ISlamObject, bool> filter,
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
                var existentObs = SlamObjectQuery(package.Observations.Where(o => filter(o)), id => graph.ObservationExists(id));
                Test(package, existentObs, "Observation id {0} already exists. New: '{1}'. Removed: '{2}'");
            }
        }

        public static void TestNonExistent(
            this Package package,
            Func<ISlamObject, bool> filter,
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
                var existentObs = SlamObjectQuery(package.Observations.Where(o => filter(o)), id => !graph.ObservationExists(id));
                ElektronikLogger.WrapDebug(() => Test(package, existentObs, "Observation id {0} doesn't exists. New: '{1}'. Removed: '{2}'"));
            }
#endif
        }
    }
}
