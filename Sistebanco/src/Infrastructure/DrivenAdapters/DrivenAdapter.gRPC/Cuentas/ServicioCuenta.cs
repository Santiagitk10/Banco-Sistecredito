using Domain.Model.Interfaces.UseCases.Cuentas;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcServicioCliente;
using GrpcServicioCuenta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GrpcServicioCuenta.ServicioCuenta;
using static Microsoft.Azure.Amqp.Serialization.SerializableType;
using Cuenta = GrpcServicioCliente.Cuenta;

namespace DrivenAdapter.gRPC.Cuentas
{
    public class ServicioCuenta : ServicioCuentaBase
    {
        private readonly ICancelarCuentaUseCase _cancelarCuentaUseCase;
        private readonly ICrearCuentaUseCase _crearCuentaUseCase;
        private readonly IMarcarExentaDeGravamenUseCase _marcarExentaGravamenUseCase;
        private readonly IObtenerEstadoDeCuentaUseCase _obtenerEstadoDeCuentaUseCase;

        public ServicioCuenta(ICancelarCuentaUseCase cancelarCuentaUseCase, ICrearCuentaUseCase crearCuentaUseCase, IMarcarExentaDeGravamenUseCase marcarExentaGravamenUseCase, IObtenerEstadoDeCuentaUseCase obtenerEstadoDeCuentaUseCase)
        {
            _cancelarCuentaUseCase = cancelarCuentaUseCase;
            _crearCuentaUseCase = crearCuentaUseCase;
            _marcarExentaGravamenUseCase = marcarExentaGravamenUseCase;
            _obtenerEstadoDeCuentaUseCase = obtenerEstadoDeCuentaUseCase;
        }


        public override async Task<GrpcServicioCliente.Cuenta> CrearCuenta(IdClienteYCuenta request, ServerCallContext context)
        => await Handler.HandleRequestAsync(
            async () =>
            {
                Domain.Model.Entities.Cuenta cuenta = await _crearCuentaUseCase.CrearCuenta(request.IdCliente, Converter.FromGrpcCuentaRequestToDomainCuenta(request.Cuenta));
                return Converter.FromDomainCuentaToGrpcCuenta(cuenta);
            }
            );

        public override async Task<GrpcServicioCliente.Empty> CancelarCuenta(IdCuenta request, ServerCallContext context)
            => await Handler.HandleRequestAsync(
                async () =>
                {
                    await _cancelarCuentaUseCase.CancelarCuenta(request.Id);
                    return new GrpcServicioCliente.Empty();
                });

        public override async Task<Cuenta> ConsultarEstadoDeCuenta(IdCuenta request, ServerCallContext context)
            => await Handler.HandleRequestAsync(
                async () =>
                {
                    Domain.Model.Entities.Cuenta cuenta = await _obtenerEstadoDeCuentaUseCase.ObtenerEstadoDeCuenta(request.Id);
                    return Converter.FromDomainCuentaToGrpcCuenta(cuenta);
                }
                );


        public override async Task<Cuenta> MarcarCuentaExentaDeGravamen(IdCuentaYCliente request, ServerCallContext context)
        => await Handler.HandleRequestAsync(
            async () =>
            {
                Domain.Model.Entities.Cuenta cuenta = await _marcarExentaGravamenUseCase.MarcarExentaDeGravamen(request.IdCliente, request.IdCuenta);
                return Converter.FromDomainCuentaToGrpcCuenta(cuenta);
            }
            );
    }
}