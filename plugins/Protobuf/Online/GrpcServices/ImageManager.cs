﻿using System;
using System.Threading.Tasks;
using Elektronik.Common.Data.Pb;
using Elektronik.Protobuf.Online.Presenters;
using Grpc.Core;

namespace Elektronik.Protobuf.Online.GrpcServices
{
    public class ImageManager : ImageManagerPb.ImageManagerPbBase
    {
        private readonly RawImagePresenter _presenter;

        public ImageManager(RawImagePresenter presenter)
        {
            _presenter = presenter;
        }

        public override Task<ErrorStatusPb> Handle(ImagePacketPb request, ServerCallContext context)
        {
            var err = new ErrorStatusPb()
            {
                ErrType = ErrorStatusPb.Types.ErrorStatusEnum.Succeeded,
            };
            try
            {
                _presenter.Present(request.ImageData.ToByteArray());
            }
            catch (Exception e)
            {
                err.ErrType = ErrorStatusPb.Types.ErrorStatusEnum.Failed;
                err.Message = e.Message;
            }

            return Task.FromResult(err);
        }
    }
}