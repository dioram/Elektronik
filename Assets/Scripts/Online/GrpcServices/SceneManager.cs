using Elektronik.Common.Data.Pb;
using Elektronik.Common.Maps;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elektronik.Online.GrpcServices
{
    public class SceneManager : SceneManagerPb.SceneManagerPbBase
    {
        SlamMap m_map;

        public SceneManager(SlamMap slamMap)
        {
            m_map = slamMap;
        }

        public override Task<ErrorStatusPb> Clear(Empty request, ServerCallContext context)
        {
            var err = new ErrorStatusPb()
            {
                ErrType = ErrorStatusPb.Types.ErrorStatusEnum.Succeeded,
            };
            try
            {
                m_map.Clear();
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
