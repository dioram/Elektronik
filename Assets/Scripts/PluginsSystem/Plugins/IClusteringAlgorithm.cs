using System.Collections.Generic;
using System.Threading.Tasks;
using Elektronik.DataObjects;
using Elektronik.DataSources.Containers;
using Elektronik.DataSources.SpecialInterfaces;
using UnityEngine;
using SettingsBag = Elektronik.Settings.SettingsBag;

namespace Elektronik.PluginsSystem
{
    // TODO: Maybe it should not return ClustersContainer but List<List<SlamPoint>
    
    /// <summary> Interface for plugins that implements custom clustering algorithms. </summary>
    /// <remarks>
    /// It is better to inherit from <see cref="ClusteringAlgorithmBase{TSettings}"/>
    /// than implementing this interface.
    /// </remarks>
    public interface IClusteringAlgorithm : IElektronikPlugin
    {
        /// <summary> Computes clusters from given data source. </summary>
        Task<ClustersContainer> ComputeAsync(IClusterableDataSource container);
    }

    /// <summary> Base class for clustering algorithm plugins. </summary>
    /// <typeparam name="TSettings"> Type of settings used by plugin. </typeparam>
    public abstract class ClusteringAlgorithmBase<TSettings> : IClusteringAlgorithm
            where TSettings : SettingsBag
    {
        /// <summary> Constructor. </summary>
        public ClusteringAlgorithmBase(TSettings settings)
        {
            TypedSettings = settings;
        }

        #region IClusteringAlgorithm
        
        /// <inheritdoc />
        public async Task<ClustersContainer> ComputeAsync(IClusterableDataSource container)
        {
            var clusters = await Task.Run(() => Compute(container.GetAllPoints(), TypedSettings));
            return new ClustersContainer($"Clustered {container.DisplayName}", clusters, container as IVisibleDataSource);
        }

        #endregion

        #region IElektronikPlugin

        /// <inheritdoc />
        public virtual void Dispose()
        {
            // Do nothing
        }

        /// <inheritdoc />
        public abstract string DisplayName { get; }

        /// <inheritdoc />
        public SettingsBag Settings => TypedSettings;

        /// <inheritdoc />
        public virtual Texture2D Logo => null;

        /// <inheritdoc />
        public virtual void Update(float delta)
        {
            // Do nothing
        }

        #endregion

        #region Protected

        /// <summary> Actual computation of clusters. </summary>
        /// <param name="points"> Point cloud. </param>
        /// <param name="settings"> Clusterization settings. </param>
        /// <returns></returns>
        protected abstract IList<IList<SlamPoint>> Compute(IList<SlamPoint> points, TSettings settings);
        
        /// <summary> Settings with type specific  for this plugin. </summary>
        protected readonly TSettings TypedSettings;

        #endregion
    }
}