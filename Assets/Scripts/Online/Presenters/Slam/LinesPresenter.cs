using Elektronik.Common.Containers;
using Elektronik.Common.Data;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Packages;
using Elektronik.Common.Data.Packages.SlamActionPackages;
using Elektronik.Common.Maps;
using Elektronik.Common.Presenters;

namespace Elektronik.Online.Presenters.Slam
{
    public class LinesPresenter : SlamObjectsPresenter<SlamLine>
    {
        public SlamMap map;
        public override IRepaintableContainer<SlamLine> Map => map.LinesContainer;

        public override void Present(IPackage package)
        {
            if (package.PackageType == PackageType.SLAMPackage)
            {
                var slamPackage = package as ISlamActionPackage;
                if (slamPackage.ObjectType == ObjectType.Line)
                {
                    UpdateObjects(slamPackage as ActionDataPackage<SlamLine>);
                }
            }
            m_presenter?.Present(package);
        }

        protected override SlamLine Move(SlamLine p)
        {
            var line = map.LinesContainer[p];
            var pt1 = line.pt1; pt1.position = p.pt1.position;
            var pt2 = line.pt2; pt2.position = p.pt2.position;
            return new SlamLine(pt1, pt2);
        }

        protected override SlamLine Tint(SlamLine p)
        {
            var line = map.LinesContainer[p];
            var pt1 = line.pt1; pt1.color = p.pt1.color;
            var pt2 = line.pt2; pt2.color = p.pt2.color;
            return new SlamLine(pt1, pt2);
        }
    }
}
