using Elektronik.Protobuf.Data;

namespace Elektronik.Protobuf.Online.Presenters
{
    public class RawImagePresenter : ImagePresenter<byte[]>
    {
        public RawImagePresenter(string displayName) : base(displayName)
        {
        }

        public override void Present(byte[] data)
        {
            Renderer.Render(data);
        }
    }
}