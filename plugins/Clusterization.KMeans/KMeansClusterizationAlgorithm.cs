using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Elektronik.Data.PackageObjects;
using Elektronik.PluginsSystem;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace Elektronik.Clusterization.KMeans
{
    public class KMeansClusterizationAlgorithm: ClusterizationAlgorithmBase<KMeansSettings>
    {
        public KMeansClusterizationAlgorithm(KMeansSettings settings, string displayName) : base(settings)
        {
            DisplayName = displayName;
        }

        protected override IList<IList<SlamPoint>> Compute(IList<SlamPoint> points, KMeansSettings settings)
        {
            var res = Enumerable.Range(0, settings.NumberOfClusters)
                .Select(_ => (IList<SlamPoint>)new List<SlamPoint>())
                .ToList();
            var clusters = ComputeClusters(points, settings.NumberOfClusters);
            for (int i = 0; i < clusters.Count; i++)
            {
                res[clusters[i]].Add(points[i]);
            }

            return res;
        }

        public override string DisplayName { get; }

        private List<int> ComputeClusters(IList<SlamPoint> points, int numberOfClusters)
        {
            // TODO: rewrite without file
            File.WriteAllLines("tmp.csv", points
                                   .Select(p => $"{p.Position.x.ToString(CultureInfo.InvariantCulture)};" +
                                                $"{p.Position.y.ToString(CultureInfo.InvariantCulture)};" +
                                                $"{p.Position.z.ToString(CultureInfo.InvariantCulture)}"));
            var mlContext = new MLContext(seed: 0);
            var reader = mlContext.Data.CreateTextLoader(
                                                         new[]
                                                         {
                                                             new TextLoader.Column("X", DataKind.Single, 0),
                                                             new TextLoader.Column("Y", DataKind.Single, 1),
                                                             new TextLoader.Column("Z", DataKind.Single, 2),
                                                         },
                                                         hasHeader: false,
                                                         separatorChar: ';'
                                                        );

            var dataView = reader.Load("tmp.csv");
            var pipeline = mlContext.Transforms
                .Concatenate("Features", "X", "Y", "Z")
                .Append(mlContext.Clustering.Trainers.KMeans(numberOfClusters: numberOfClusters));

            var transformed = pipeline.Fit(dataView).Transform(dataView);
            var clusterColumn = transformed.Schema.GetColumnOrNull("PredictedLabel")!.Value;
            var cursor = transformed.GetRowCursor(new[] { clusterColumn });
            var clusterizatorResult = cursor.GetGetter<uint>(clusterColumn);

            var res = new List<int>();
            while (cursor.MoveNext())
            {
                uint cluster = 0;
                clusterizatorResult(ref cluster);
                res.Add((int)cluster - 1);
            }

            File.Delete("tmp.csv");

            return res;
        }
    }
}