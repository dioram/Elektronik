using System.Collections.Generic;
using System.Threading.Tasks;
using Elektronik.Data.PackageObjects;
using Elektronik.DataSources;
using Elektronik.DataSources.Containers;
using Elektronik.DataSources.SpecialInterfaces;
using UnityEngine;
using SettingsBag = Elektronik.Settings.SettingsBag;

namespace Elektronik.PluginsSystem
{
    public interface IClusteringAlgorithm: IElektronikPlugin
    {
        Task<ClustersContainer> ComputeAsync(IClusterable container);
    }

    public abstract class ClusteringAlgorithmBase<TSettings> : IClusteringAlgorithm
        where TSettings : SettingsBag
    {
        public ClusteringAlgorithmBase(TSettings typedSettings)
        {
            TypedSettings = typedSettings;
        }

        public async Task<ClustersContainer> ComputeAsync(IClusterable container)
        {
            var clusters = await Task.Run(() => Compute(container.GetAllPoints(), TypedSettings));
            return new ClustersContainer($"Clustered {((ISourceTreeNode)container).DisplayName}",
                                         clusters, container as IVisible);
        }

        #region IElektronikPlugin
        
        public virtual void Dispose()
        {
            // Do nothing
        }

        public abstract string DisplayName { get; }
        public SettingsBag Settings => TypedSettings;
        public virtual Texture2D Logo => null;
        
        public virtual void Update(float delta)
        {
            // Do nothing
        }
        
        #endregion

        #region Protected

        protected abstract IList<IList<SlamPoint>> Compute(IList<SlamPoint> points, TSettings settings);
        protected readonly TSettings TypedSettings;

        #endregion
    }
}