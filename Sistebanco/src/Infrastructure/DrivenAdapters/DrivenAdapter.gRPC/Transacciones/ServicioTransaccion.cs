using Domain.Model.Interfaces.UseCases.Transacciones;
using Domain.UseCase.Transacciones;
using Grpc.Core;
using GrpcServicioCliente;
using GrpcServicioTransaccion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GrpcServicioTransaccion.ServicioTransaccion;

namespace DrivenAdapter.gRPC.Transacciones
{
    public class ServicioTransaccion : ServicioTransaccionBase
    {
        private readonly IConsignarUseCase _consignarUseCase;
        private readonly IRetirarUseCase _retirarUseCase;
        private readonly ITransferirUseCase _transferirUseCase;

        public ServicioTransaccion(
            IConsignarUseCase consignarUseCase,
            IRetirarUseCase retirarUseCase,
            ITransferirUseCase transferirUseCase)
        {
            _consignarUseCase = consignarUseCase;
            _retirarUseCase = retirarUseCase;
            _transferirUseCase = transferirUseCase;
        }

        public override async Task<Transaccion> Consignar(DatosConsignarYRetirar request, ServerCallContext context) =>
            await Handler.HandleRequestAsync(
            async () =>
            {
                Decimal.TryParse(request.Monto, out decimal monto);
                Domain.Model.Entities.Transaccion transaccion = await _consignarUseCase.Consignar(request.IdCliente, request.IdCuenta, monto, request.Mensaje);
                return Converter.FromDomainTransaccionToGrpcTransaccion(transaccion);
            });

        public override async Task<Transaccion> Retirar(DatosConsignarYRetirar request, ServerCallContext context) =>
            await Handler.HandleRequestAsync(
            async () =>
            {
                Decimal.TryParse(request.Monto, out decimal monto);
                Domain.Model.Entities.Transaccion transaccion = await _retirarUseCase.Retirar(request.IdCliente, request.IdCuenta, monto, request.Mensaje);
                return Converter.FromDomainTransaccionToGrpcTransaccion(transaccion);
            });

        public override async Task<Transaccion> Transferir(DatosTransferir request, ServerCallContext context) =>
            await Handler.HandleRequestAsync(
            async () =>
            {
                Decimal.TryParse(request.Monto, out decimal monto);
                Domain.Model.Entities.Transaccion transaccion = await _transferirUseCase.Transferir(request.IdCliente, request.IdCuentaOrigen, request.IdCuentaDestino, monto, request.Mensaje);
                return Converter.FromDomainTransaccionToGrpcTransaccion(transaccion);
            });
    }
}