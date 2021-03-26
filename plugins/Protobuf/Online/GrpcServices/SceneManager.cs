using System;
using System.Threading.Tasks;
using Elektronik.Containers;
using Elektronik.Protobuf.Data;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Elektronik.Protobuf.Online.GrpcServices
{
    public class SceneManager : SceneManagerPb.SceneManagerPbBase
    {
        private readonly ISourceTree _container;

        public SceneManager(ISourceTree container)
        {
            _container = container;
        }

        public override Task<ErrorStatusPb> Clear(Empty request, ServerCallContext context)
        {
            var err = new ErrorStatusPb()
            {
                ErrType = ErrorStatusPb.Types.ErrorStatusEnum.Succeeded,
            };
            try
            {
                foreach (var child in _container.Children)
                {
                    child.Clear();
                }
            }
            catch(Exception e)
            {
                err.ErrType = ErrorStatusPb.Types.ErrorStatusEnum.Failed;
                err.Message = e.Message;
            }
            return Task.FromResult(err);
        }
    }
}
