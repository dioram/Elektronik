using Elektronik.Common.Containers;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Elektronik.Common.Data.Packages;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Loggers;

namespace Elektronik.Common.Data
{
    public static class PackageTestExtensions
    {
        public static void Test(
            SlamPackage package,
            IEnumerable<SlamPoint> objects,
            string messageTemplate)
        {
            var objArray = objects.ToArray();
            if (objArray.Length != 0)
            {
                ElektronikLogger.Log(package.ToString(), "", LogType.Error);
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
            this SlamPackage package,
            Func<SlamPoint, bool> filter,
            ICloudObjectsContainer<SlamPoint> pointsContainer,
            ICloudObjectsContainer<SlamObservation> graph)
        {
            if (package.Points != null)
            {
                var existentPoints = SlamObjectQuery(package.Points.Where(p => filter(p)), id => pointsContainer.Exists(id));
                Test(package, existentPoints, "Point id {0} already exists. New: '{1}'. Removed: '{2}'");
            }
            if (package.Observations != null)
            {
                var existentObs = SlamObjectQuery(package.Observations.Select(o => (SlamPoint)o).Where(o => filter(o)), id => graph.Exists(id));
                Test(package, existentObs, "Observation id {0} already exists. New: '{1}'. Removed: '{2}'");
            }
        }

        public static void TestNonExistent(
            this SlamPackage package,
            Func<SlamPoint, bool> filter,
            ICloudObjectsContainer<SlamPoint> pointsContainer,
            ICloudObjectsContainer<SlamObservation> graph)
        {
#if DEBUG
            if (package.Points != null)
            {
                var existentPoints = SlamObjectQuery(package.Points.Where(p => filter(p)), id => !pointsContainer.Exists(id));
                ElektronikLogger.WrapDebug(() => Test(package, existentPoints, "Point id {0} doesn't exists. New: '{1}'. Removed: '{2}'"));
            }
            if (package.Observations != null)
            {
                var existentObs = SlamObjectQuery(package.Observations.Select(o => (SlamPoint)o).Where(o => filter(o)), id => !graph.Exists(id));
                ElektronikLogger.WrapDebug(() => Test(package, existentObs, "Observation id {0} doesn't exists. New: '{1}'. Removed: '{2}'"));
            }
#endif
        }
    }
}
