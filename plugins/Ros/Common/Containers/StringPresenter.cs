using Elektronik.Renderers;
using RosSharp.RosBridgeClient.MessageTypes.Std;

namespace Elektronik.RosPlugin.Common.Containers
{
    public class StringPresenter : PresenterBase<String, StringRenderer, string>
    {
        public StringPresenter(string displayName) : base(displayName)
        {
        }

        protected override string ToRenderType(String message)
        {
            return message.data;
        }
    }
}