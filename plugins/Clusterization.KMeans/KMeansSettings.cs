using Elektronik.Settings.Bags;
using Elektronik.UI.Localization;

namespace Elektronik.Clusterization.KMeans
{
    public class KMeansSettings : SettingsBag
    {
        public int NumberOfClusters = 5;

        public override ValidationResult Validate()
        {
            return NumberOfClusters > 1 
                       ? ValidationResult.Succeeded 
                       : ValidationResult.Failed("Can't make less than 2 clusters".tr());
        }
    }
}