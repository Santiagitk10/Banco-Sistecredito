using credinet.exception.middleware.models;
using Domain.Model.Enums;
using Domain.Model.Interfaces.UseCases.Transacciones;
using DrivenAdapter.gRPC.Transacciones;
using Grpc.Core;
using GrpcServicioCliente;
using GrpcServicioTransaccion;
using Helpers.Commons.Exceptions;
using Helpers.Domain.EntityBuilders;
using Helpers.ObjectsUtils.Extensions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServicioTransaccion = DrivenAdapter.gRPC.Transacciones.ServicioTransaccion;

namespace DrivenAdapter.gRPC.Tests
{
    public class GrpcTransaccionTest
    {
        private readonly Mock<ServerCallContext> _serverCallContext;
        private readonly Mock<IConsignarUseCase> _consignarUseCase;
        private readonly Mock<IRetirarUseCase> _retirarUseCase;
        private readonly Mock<ITransferirUseCase> _transferirUseCase;

        public GrpcTransaccionTest()
        {
            _serverCallContext = new();
            _consignarUseCase = new();
            _retirarUseCase = new();
            _transferirUseCase = new();
        }

        [Fact]
        public async Task ConsignarTest_Excepcion_TransaccionFallidaMontoInvalido()
        {
            _consignarUseCase.Setup(c => c.Consignar(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>()))
                .ThrowsAsync(new BusinessException(
                    TipoExcepcionNegocio.TransaccionFallidaMontoInvalido.GetDescription(),
                    (int)TipoExcepcionNegocio.TransaccionFallidaMontoInvalido));

            var grpc = new ServicioTransaccion(
                     _consignarUseCase.Object,
                     _retirarUseCase.Object,
                     _transferirUseCase.Object
                );

            DatosConsignarYRetirar datosConsignarYRetirar = new()
            {
                IdCliente = "1",
                IdCuenta = "2300000000",
                Monto = "0",
            };

            RpcException result = await Assert.ThrowsAsync<RpcException>(async () => await grpc.Consignar(datosConsignarYRetirar, _serverCallContext.Object));

            Assert.Equal(TipoExcepcionNegocio.TransaccionFallidaMontoInvalido.GetDescription(), result.Status.Detail);
        }

        [Fact]
        public async Task ConsignarTest_Exitoso()
        {
            _consignarUseCase.Setup(c => c.Consignar(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>()))
                .ReturnsAsync(GetTransaccion());

            var grpc = new ServicioTransaccion(
                     _consignarUseCase.Object,
                     _retirarUseCase.Object,
                     _transferirUseCase.Object
                );

            DatosConsignarYRetirar datosConsignarYRetirar = new()
            {
                IdCliente = "1",
                IdCuenta = "2300000000",
                Monto = "1000"
            };

            var result = await grpc.Consignar(datosConsignarYRetirar, _serverCallContext.Object);

            Assert.IsType<Transaccion>(result);
        }

        [Fact]
        public async Task RetirarTest_Excepcion_TransaccionFallidaMontoInvalido()
        {
            _retirarUseCase.Setup(c => c.Retirar(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>()))
                .ThrowsAsync(new BusinessException(
                    TipoExcepcionNegocio.TransaccionFallidaMontoInvalido.GetDescription(),
                    (int)TipoExcepcionNegocio.TransaccionFallidaMontoInvalido));

            var grpc = new ServicioTransaccion(
                     _consignarUseCase.Object,
                     _retirarUseCase.Object,
                     _transferirUseCase.Object
                );

            DatosConsignarYRetirar datosConsignarYRetirar = new()
            {
                IdCliente = "1",
                IdCuenta = "2300000000",
                Monto = "0"
            };

            RpcException result = await Assert.ThrowsAsync<RpcException>(async () => await grpc.Retirar(datosConsignarYRetirar, _serverCallContext.Object));

            Assert.Equal(TipoExcepcionNegocio.TransaccionFallidaMontoInvalido.GetDescription(), result.Status.Detail);
        }

        [Fact]
        public async Task TransferirTest_Excepcion_TransaccionFallidaMontoInvalido()
        {
            _transferirUseCase.Setup(c => c.Transferir(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>()))
                .ThrowsAsync(new BusinessException(
                    TipoExcepcionNegocio.TransaccionFallidaMontoInvalido.GetDescription(),
                    (int)TipoExcepcionNegocio.TransaccionFallidaMontoInvalido));

            var grpc = new ServicioTransaccion(
                     _consignarUseCase.Object,
                     _retirarUseCase.Object,
                     _transferirUseCase.Object
                );

            DatosTransferir datosTransferir = new()
            {
                IdCliente = "1",
                IdCuentaOrigen = "2300000000",
                IdCuentaDestino = "2300000001",
                Monto = "0",
                Mensaje = "transferencia"
            };

            RpcException result = await Assert.ThrowsAsync<RpcException>(async () => await grpc.Transferir(datosTransferir, _serverCallContext.Object));

            Assert.Equal(TipoExcepcionNegocio.TransaccionFallidaMontoInvalido.GetDescription(), result.Status.Detail);
        }

        private Domain.Model.Entities.Transaccion GetTransaccion()
        {
            return new TransaccionTestBuilder()
                .ConId("11111")
                .ConMonto(1000)
                .ConTipoDeTransaccion(TipoDeTransacciones.CONSIGNACION)
                .ConFechaDelMovimiento(DateTime.UtcNow)
                .ConGravamenDelMovimiento(0)
                .Build();
        }
    }
}