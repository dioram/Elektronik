using Elektronik.Common.Data.Pb;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;
using System.Threading.Tasks;
using Elektronik.Common.Containers;
using UnityEngine;

namespace Elektronik.Online.GrpcServices
{
    public class SceneManager : SceneManagerPb.SceneManagerPbBase
    {
        private GameObject _containers;

        public SceneManager(GameObject containers)
        {
            _containers = containers;
        }

        public override Task<ErrorStatusPb> Clear(Empty request, ServerCallContext context)
        {
            var err = new ErrorStatusPb()
            {
                ErrType = ErrorStatusPb.Types.ErrorStatusEnum.Succeeded,
            };
            try
            {
                foreach (var container in _containers.GetComponentsInChildren<IClearable>())
                {
                    container.Clear();
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
