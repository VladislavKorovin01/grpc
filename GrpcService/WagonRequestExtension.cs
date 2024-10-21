using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;

namespace EpcDataApp.GrpcService
{
    public static class WagonRequestExtension
    {
        public static void Validate(this WagonRequest request)
        {
            if (request.TimeEnd < request.TimeStart)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Конечная дата не может быть меньше начальной!"));
            }

        }
    }
}
