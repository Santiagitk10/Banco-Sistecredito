using credinet.exception.middleware.models;
using Domain.Model.Enums;
using Domain.Model.Interfaces.UseCases.Cuentas;
using Grpc.Core;
using GrpcServicioCliente;
using GrpcServicioCuenta;
using Helpers.Commons.Exceptions;
using Helpers.Domain.EntityBuilders;
using Helpers.ObjectsUtils.Extensions;
using Moq;

namespace DrivenAdapter.gRPC.Tests
{
    public class GrpcCuentaTest
    {
        private readonly Mock<ServerCallContext> _serverCallContext;
        private readonly Mock<ICancelarCuentaUseCase> _cancelarUseCase;
        private readonly Mock<ICrearCuentaUseCase> _crearUseCase;
        private readonly Mock<IMarcarExentaDeGravamenUseCase> _marcarUseCase;
        private readonly Mock<IObtenerEstadoDeCuentaUseCase> _obtenerUseCase;

        public GrpcCuentaTest()
        {
            _serverCallContext = new();
            _cancelarUseCase = new();
            _crearUseCase = new();
            _marcarUseCase = new();
            _obtenerUseCase = new();
        }

        [Fact]
        public async Task CancelarCuentaTest_Excepcion_CuentaNoExiste()
        {
            _cancelarUseCase.Setup(c => c.CancelarCuenta(It.IsAny<string>()))
                .ThrowsAsync(new BusinessException(TipoExcepcionNegocio.CuentaNoExisteEnCliente.GetDescription(), (int)TipoExcepcionNegocio.CuentaNoExisteEnCliente));

            var grpc = new DrivenAdapter.gRPC.Cuentas.ServicioCuenta(
                _cancelarUseCase.Object,
                _crearUseCase.Object,
                _marcarUseCase.Object,
                _obtenerUseCase.Object
                );
            IdCuenta id = new()
            {
                Id = "1"
            };
            RpcException result = await Assert.ThrowsAsync<RpcException>(async () => await grpc.CancelarCuenta(id, _serverCallContext.Object));

            Assert.Equal(TipoExcepcionNegocio.CuentaNoExisteEnCliente.GetDescription(), result.Status.Detail);
        }

        [Fact]
        public async Task CancelarCuentaTest_Exitoso()
        {
            _cancelarUseCase.Setup(c => c.CancelarCuenta(It.IsAny<string>()));

            var grpc = new DrivenAdapter.gRPC.Cuentas.ServicioCuenta(
                _cancelarUseCase.Object,
                _crearUseCase.Object,
                _marcarUseCase.Object,
                _obtenerUseCase.Object
                );

            IdCuenta id = new()
            {
                Id = "1"
            };
            var result = await grpc.CancelarCuenta(id, _serverCallContext.Object);

            Assert.IsType<Empty>(result);
        }

        [Fact]
        public async Task CrearCuenta_Exitoso()
        {
            _crearUseCase.Setup(c => c.CrearCuenta(It.IsAny<string>(), It.IsAny<Domain.Model.Entities.Cuenta>()))
                .ReturnsAsync(GetCuentaAhorrosActivaConId());

            var grpc = new DrivenAdapter.gRPC.Cuentas.ServicioCuenta(
                _cancelarUseCase.Object,
                _crearUseCase.Object,
                _marcarUseCase.Object,
                _obtenerUseCase.Object
                );

            IdClienteYCuenta idycuenta = new()
            {
                IdCliente = "1",
                Cuenta = new()
                {
                    TipoDeCuenta = "Ahorros"
                }
            };
            var result = await grpc.CrearCuenta(idycuenta, _serverCallContext.Object);

            Assert.IsType<Cuenta>(result);
            Assert.Equal(TiposDeCuenta.AHORROS.ToString(), result.TipoDeCuenta);
        }

        [Fact]
        public async Task ConsultarEstadoDeCuenta_Exitoso()
        {
            _obtenerUseCase.Setup(c => c.ObtenerEstadoDeCuenta(It.IsAny<string>()))
                .ReturnsAsync(GetCuentaAhorrosActivaConId());

            var grpc = new DrivenAdapter.gRPC.Cuentas.ServicioCuenta(
                _cancelarUseCase.Object,
                _crearUseCase.Object,
                _marcarUseCase.Object,
                _obtenerUseCase.Object
                );

            IdCuenta idCuenta = new()
            {
                Id = "1"
            };
            var result = await grpc.ConsultarEstadoDeCuenta(idCuenta, _serverCallContext.Object);

            Assert.IsType<Cuenta>(result);
            Assert.Equal(TiposDeCuenta.AHORROS.ToString(), result.TipoDeCuenta);
        }

        [Fact]
        public async Task MarcarCuentaExentaDeGravamen_Exitoso()
        {
            _marcarUseCase.Setup(c => c.MarcarExentaDeGravamen(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(GetCuentaAhorrosActivaConIdSinGravamen());

            var grpc = new DrivenAdapter.gRPC.Cuentas.ServicioCuenta(
                _cancelarUseCase.Object,
                _crearUseCase.Object,
                _marcarUseCase.Object,
                _obtenerUseCase.Object
                );

            IdCuentaYCliente idCuentaYCliente = new()
            {
                IdCliente = "1",
                IdCuenta = "1"
            };
            var result = await grpc.MarcarCuentaExentaDeGravamen(idCuentaYCliente, _serverCallContext.Object);

            Assert.IsType<Cuenta>(result);
            Assert.Equal(TiposDeCuenta.AHORROS.ToString(), result.TipoDeCuenta);
            Assert.True(result.EsNoGravable);
        }

        private Domain.Model.Entities.Cuenta GetCuentaAhorrosActivaConId()
        {
            return new CuentaTestBuilder()
                .ConEstadoDeCuenta(Domain.Model.Enums.EstadosDeCuenta.ACTIVA)
                .ConId("46")
                .ConSaldo(0)
                .ConTransacciones(new List<Domain.Model.Entities.Transaccion>())
                .ConTipoDeCuenta(Domain.Model.Enums.TiposDeCuenta.AHORROS)
                .Build();
        }

        private Domain.Model.Entities.Cuenta GetCuentaAhorrosActivaConIdSinGravamen()
        {
            return new CuentaTestBuilder()
                .ConEstadoDeCuenta(Domain.Model.Enums.EstadosDeCuenta.ACTIVA)
                .ConId("46")
                .ConSaldo(0)
                .EsNoGravable(true)
                .ConTransacciones(new List<Domain.Model.Entities.Transaccion>())
                .ConTipoDeCuenta(Domain.Model.Enums.TiposDeCuenta.AHORROS)
                .Build();
        }
    }
}