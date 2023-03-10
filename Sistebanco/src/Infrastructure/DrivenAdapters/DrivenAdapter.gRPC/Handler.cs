using credinet.exception.middleware.models;
using Grpc.Core;
using Grpc.Core.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrivenAdapter.gRPC
{
    public class Handler
    {
        public static async Task<TResult> HandleRequestAsync<TResult>(Func<Task<TResult>> requestHandler)
        {
            try
            {
                TResult result = await requestHandler();
                return result;
            }
            catch (Exception ex)
            {
                if (ex is BusinessException businessEx)
                {

                    throw new RpcException(new Status(StatusCode.Internal, businessEx.Message));
                }

                throw new RpcException(new Status(StatusCode.Unavailable, ex.Message));
            }
        }
    }
}
