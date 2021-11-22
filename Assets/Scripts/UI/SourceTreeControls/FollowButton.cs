using Elektronik.Cameras;
using Elektronik.DataObjects;
using Elektronik.DataSources.SpecialInterfaces;
using Elektronik.UI.Buttons;
using UnityEngine;

namespace Elektronik.UI.SourceTreeControls
{
    /// <summary> This buttons turns on/off follow mode. </summary>
    [RequireComponent(typeof(ButtonChangingIcons))]
    internal class FollowButton : SourceTreeButton<IFollowableDataSource<SlamTrackedObject>, ButtonChangingIcons>
    {
        /// <inheritdoc />
        protected override void Initialize(IFollowableDataSource<SlamTrackedObject> dataSource, ButtonChangingIcons uiButton)
        {
            
            if (Camera.main is null)
            {
                Destroy(gameObject);
                return;
            }

            var cam = Camera.main.GetComponent<LookableCamera>();
            var lookable = dataSource as ILookableDataSource;
            if (cam is null || lookable is null)
            {
                Destroy(gameObject);
                return;
            }
            
            uiButton.OnStateChanged += state =>
            {
                if (state == 0)
                {
                    dataSource.Unfollow();
                }
                else
                {
                    cam.Look(lookable.Look(cam.transform));
                    dataSource.Follow();
                }
            };
        }

        private void Update()
        {
            UiButton.SilentSetState(DataSource.IsFollowed ? 1 : 0);
        }
    }
}