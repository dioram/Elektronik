using Elektronik.Common.Containers;
using Elektronik.Common.Data;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Packages;
using Elektronik.Common.Data.Packages.SlamActionPackages;
using Elektronik.Common.Maps;
using Elektronik.Common.Presenters;

namespace Elektronik.Online.Presenters.Slam
{
    public class LinesPresenter : SlamObjectsPresenter<SlamLine2>
    {
        public SlamMap map;
        public override IRepaintableContainer<SlamLine2> Map => map.LinesContainer;

        public override void Present(IPackage package)
        {
            if (package.PackageType == PackageType.SLAMPackage)
            {
                var slamPackage = package as ISlamActionPackage;
                if (slamPackage.ObjectType == ObjectType.Line)
                {
                    UpdateObjects(slamPackage as ActionDataPackage<SlamLine2>);
                }
            }
            m_presenter?.Present(package);
        }

        protected override SlamLine2 Move(SlamLine2 p)
        {
            var line = map.LinesContainer[p];
            var pt1 = line.pt1; pt1.position = p.pt1.position;
            var pt2 = line.pt2; pt2.position = p.pt2.position;
            return new SlamLine2(pt1, pt2);
        }

        protected override SlamLine2 Tint(SlamLine2 p)
        {
            var line = map.LinesContainer[p];
            var pt1 = line.pt1; pt1.color = p.pt1.color;
            var pt2 = line.pt2; pt2.color = p.pt2.color;
            return new SlamLine2(pt1, pt2);
        }
    }
}
