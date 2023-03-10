using credinet.exception.middleware.models;
using Domain.Model.Entities;
using Domain.Model.Enums;
using Domain.Model.Interfaces.Gateways;
using Domain.Model.Interfaces.UseCases.Transacciones;
using Domain.UseCase.Transacciones;
using Helpers.Commons.Exceptions;
using Helpers.Domain.EntityBuilders;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Domain.UseCase.Tests.Transacciones
{
    public class TransferirUseCaseTest
    {
        private readonly MockRepository _mockRepository;
        private readonly Mock<ITransaccionRepository> _mockTransaccionRepository;
        private readonly Mock<ICuentaRepository> _mockCuentaRepository;
        private readonly Mock<IClienteRepository> _mockClienteRepository;
        private readonly TransferirUseCase _transferirUseCase;

        public TransferirUseCaseTest()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _mockTransaccionRepository = _mockRepository.Create<ITransaccionRepository>();
            _mockCuentaRepository = _mockRepository.Create<ICuentaRepository>();
            _mockClienteRepository = _mockRepository.Create<IClienteRepository>();
            _transferirUseCase = new(_mockTransaccionRepository.Object, _mockCuentaRepository.Object, _mockClienteRepository.Object);
        }

        [Fact]
        public async Task TransferirExitoso()
        {
            string idCliente = "1035222333";
            string idCuentaOrigen = "2311111111";
            string idCuentaDestino = "2322222222";
            decimal monto = 500;
            string mensaje = "Pago de Honorarios abogado";

            _mockClienteRepository
                .Setup(repository => repository.ClienteExisteConDocumentoDeIdentidad(It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockClienteRepository
                .Setup(repository => repository.ClienteConDocumentoDeIdentidadTieneCuentaConId(
                    It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockCuentaRepository
                .Setup(repository => repository.ExisteCuentaConId(It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockCuentaRepository
                .Setup(repository => repository.ObtenerCuentaPorId(idCuentaOrigen))
                .ReturnsAsync(GetCuentaOrigenOk());

            _mockCuentaRepository
                .Setup(repository => repository.ObtenerCuentaPorId(idCuentaDestino))
                .ReturnsAsync(GetCuentaDestinoOk());

            _mockClienteRepository
                .Setup(repository => repository.ClienteConDocumentoDeIdentidadTieneCuentaConId(
                    "1035222333", idCuentaDestino))
                .ReturnsAsync(true);

            _mockCuentaRepository
                .Setup(repository => repository.ActualizarCuenta(It.IsAny<string>(), It.IsAny<Cuenta>()))
                .ReturnsAsync(GetCuentaOrigenAhorrosActualizada());

            _mockCuentaRepository
                .Setup(repository => repository.ActualizarCuenta(It.IsAny<string>(), It.IsAny<Cuenta>()))
                .ReturnsAsync(GetCuentaDestinoAhorrosActualizada());

            _mockTransaccionRepository
                .Setup(repository => repository.GuardarTransaccion(It.IsAny<string>(), It.IsAny<Transaccion>()))
            .ReturnsAsync(GetTransaccionAhorros());

            Transaccion transaccionCreada = await _transferirUseCase.Transferir(idCliente, idCuentaOrigen,
                idCuentaDestino, monto, mensaje);

            _mockRepository.VerifyAll();

            Assert.NotNull(transaccionCreada);
            Assert.NotNull(transaccionCreada.Id);
            Assert.Equal(monto, transaccionCreada.Monto);
            Assert.Equal(TipoDeTransacciones.TRANSACCION_ENTRE_CUENTAS, transaccionCreada.TipoDeTransaccion);
            Assert.Equal(idCuentaDestino, transaccionCreada.IdCuentaDeDestino);
            Assert.Equal(mensaje, transaccionCreada.Mensaje);
            Assert.Equal(2m, transaccionCreada.GravamenDelMovimiento);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task TransferirConMontoIgualOInferiorACeroRetornaExcepcion(decimal monto)
        {
            string idCliente = "1035222333";
            string idCuentaOrigen = "2311111111";
            string idCuentaDestino = "2322222222";
            string mensaje = "Pago de Honorarios abogado";

            BusinessException exception =
               await Assert.ThrowsAsync<BusinessException>(async () => await _transferirUseCase.Transferir(
                   idCliente, idCuentaOrigen, idCuentaDestino, monto, mensaje));

            _mockRepository.VerifyAll();

            _mockClienteRepository.Verify(repository => repository.ClienteExisteConDocumentoDeIdentidad(
                It.IsAny<string>()), Times.Never);

            _mockClienteRepository.Verify(repository => repository.ClienteConDocumentoDeIdentidadTieneCuentaConId(
                    It.IsAny<string>(), It.IsAny<string>()), Times.Never);

            _mockCuentaRepository.Verify(repository => repository.ExisteCuentaConId(
                    It.IsAny<string>()), Times.Never);

            _mockCuentaRepository.Verify(repository => repository.ObtenerCuentaPorId(idCuentaOrigen), Times.Never);
            _mockCuentaRepository.Verify(repository => repository.ObtenerCuentaPorId(idCuentaDestino), Times.Never);

            _mockClienteRepository.Verify(repository => repository.ClienteConDocumentoDeIdentidadTieneCuentaConId(
                    idCliente, idCuentaDestino), Times.Never);

            _mockCuentaRepository.Verify(repository => repository.ActualizarCuenta(
                It.IsAny<string>(), It.IsAny<Cuenta>()), Times.Never);

            _mockTransaccionRepository.Verify(repository => repository.GuardarTransaccion(
                It.IsAny<string>(), It.IsAny<Transaccion>()), Times.Never);

            Assert.Equal((int)TipoExcepcionNegocio.TransaccionFallidaMontoInvalido, exception.code);
        }

        [Fact]
        public async Task TransferirConClienteConDocumentoDeIdentidadInexistenteRetornaExcepcion()
        {
            string idCliente = "1035222444";
            string idCuentaOrigen = "2311111111";
            string idCuentaDestino = "2322222222";
            decimal monto = 500;
            string mensaje = "Pago de Honorarios abogado";

            _mockClienteRepository
                .Setup(repository => repository.ClienteExisteConDocumentoDeIdentidad(It.IsAny<string>()))
                .ReturnsAsync(false);

            BusinessException exception =
               await Assert.ThrowsAsync<BusinessException>(async () => await _transferirUseCase.Transferir(
                   idCliente, idCuentaOrigen, idCuentaDestino, monto, mensaje));

            _mockRepository.VerifyAll();

            _mockClienteRepository.Verify(repository => repository.ClienteConDocumentoDeIdentidadTieneCuentaConId(
                    It.IsAny<string>(), It.IsAny<string>()), Times.Never);

            _mockCuentaRepository.Verify(repository => repository.ExisteCuentaConId(
                    It.IsAny<string>()), Times.Never);

            _mockCuentaRepository.Verify(repository => repository.ObtenerCuentaPorId(idCuentaOrigen), Times.Never);
            _mockCuentaRepository.Verify(repository => repository.ObtenerCuentaPorId(idCuentaDestino), Times.Never);

            _mockClienteRepository.Verify(repository => repository.ClienteConDocumentoDeIdentidadTieneCuentaConId(
                    idCliente, idCuentaDestino), Times.Never);

            _mockCuentaRepository.Verify(repository => repository.ActualizarCuenta(
                It.IsAny<string>(), It.IsAny<Cuenta>()), Times.Never);

            _mockTransaccionRepository.Verify(repository => repository.GuardarTransaccion(
                It.IsAny<string>(), It.IsAny<Transaccion>()), Times.Never);

            Assert.Equal((int)TipoExcepcionNegocio.ClienteConDocumentoDeIdentidadNoExiste, exception.code);
        }

        [Fact]
        public async Task TransferirConClienteQueNoTieneLaCuentaRetornaExcepcion()
        {
            string idCliente = "1035222333";
            string idCuentaOrigen = "2311111112";
            string idCuentaDestino = "2322222222";
            decimal monto = 500;
            string mensaje = "Pago de Honorarios abogado";

            _mockClienteRepository
                .Setup(repository => repository.ClienteExisteConDocumentoDeIdentidad(It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockClienteRepository
                .Setup(repository => repository.ClienteConDocumentoDeIdentidadTieneCuentaConId(
                    It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            BusinessException exception =
               await Assert.ThrowsAsync<BusinessException>(async () => await _transferirUseCase.Transferir(
                   idCliente, idCuentaOrigen, idCuentaDestino, monto, mensaje));

            _mockRepository.VerifyAll();

            _mockCuentaRepository.Verify(repository => repository.ExisteCuentaConId(
                    It.IsAny<string>()), Times.Never);

            _mockCuentaRepository.Verify(repository => repository.ObtenerCuentaPorId(idCuentaOrigen), Times.Never);
            _mockCuentaRepository.Verify(repository => repository.ObtenerCuentaPorId(idCuentaDestino), Times.Never);

            _mockClienteRepository.Verify(repository => repository.ClienteConDocumentoDeIdentidadTieneCuentaConId(
                    idCliente, idCuentaDestino), Times.Never);

            _mockCuentaRepository.Verify(repository => repository.ActualizarCuenta(
                It.IsAny<string>(), It.IsAny<Cuenta>()), Times.Never);

            _mockTransaccionRepository.Verify(repository => repository.GuardarTransaccion(
                It.IsAny<string>(), It.IsAny<Transaccion>()), Times.Never);

            Assert.Equal((int)TipoExcepcionNegocio.CuentaNoExisteEnCliente, exception.code);
        }

        [Fact]
        public async Task TransferirConCuentaDestinoInexistenteRetornaExcepcion()
        {
            string idCliente = "1035222333";
            string idCuentaOrigen = "2311111111";
            string idCuentaDestino = "2322222223";
            decimal monto = 500;
            string mensaje = "Pago de Honorarios abogado";

            _mockClienteRepository
                .Setup(repository => repository.ClienteExisteConDocumentoDeIdentidad(It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockClienteRepository
                .Setup(repository => repository.ClienteConDocumentoDeIdentidadTieneCuentaConId(
                    It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockCuentaRepository
                .Setup(repository => repository.ExisteCuentaConId(It.IsAny<string>()))
                .ReturnsAsync(false);

            BusinessException exception =
               await Assert.ThrowsAsync<BusinessException>(async () => await _transferirUseCase.Transferir(
                   idCliente, idCuentaOrigen, idCuentaDestino, monto, mensaje));

            _mockRepository.VerifyAll();

            _mockCuentaRepository.Verify(repository => repository.ObtenerCuentaPorId(idCuentaOrigen), Times.Never);
            _mockCuentaRepository.Verify(repository => repository.ObtenerCuentaPorId(idCuentaDestino), Times.Never);

            _mockClienteRepository.Verify(repository => repository.ClienteConDocumentoDeIdentidadTieneCuentaConId(
                    idCliente, idCuentaDestino), Times.Never);

            _mockCuentaRepository.Verify(repository => repository.ActualizarCuenta(
                It.IsAny<string>(), It.IsAny<Cuenta>()), Times.Never);

            _mockTransaccionRepository.Verify(repository => repository.GuardarTransaccion(
                It.IsAny<string>(), It.IsAny<Transaccion>()), Times.Never);

            Assert.Equal((int)TipoExcepcionNegocio.TransaccionFallidaCuentaDestinoInexistente, exception.code);
        }

        [Fact]
        public async Task TransferirConCuentaOrigenODestinoEnEstadoCanceladoRetornaExcepcion()
        {
            string idCliente = "1035222333";
            string idCuentaOrigen = "2311111111";
            string idCuentaDestino = "2322222222";
            decimal monto = 500;
            string mensaje = "Pago de Honorarios abogado";

            _mockClienteRepository
                .Setup(repository => repository.ClienteExisteConDocumentoDeIdentidad(It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockClienteRepository
                .Setup(repository => repository.ClienteConDocumentoDeIdentidadTieneCuentaConId(
                    It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockCuentaRepository
                .Setup(repository => repository.ExisteCuentaConId(It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockCuentaRepository
                .Setup(repository => repository.ObtenerCuentaPorId(idCuentaOrigen))
                .ReturnsAsync(GetCuentaOrigenCancelada());

            _mockCuentaRepository
                .Setup(repository => repository.ObtenerCuentaPorId(idCuentaDestino))
                .ReturnsAsync(GetCuentaDestinoOk());

            BusinessException exception =
               await Assert.ThrowsAsync<BusinessException>(async () => await _transferirUseCase.Transferir(
                   idCliente, idCuentaOrigen, idCuentaDestino, monto, mensaje));

            _mockRepository.VerifyAll();

            _mockClienteRepository.Verify(repository => repository.ClienteConDocumentoDeIdentidadTieneCuentaConId(
                   idCliente, idCuentaDestino), Times.Never);

            _mockCuentaRepository.Verify(repository => repository.ActualizarCuenta(
                It.IsAny<string>(), It.IsAny<Cuenta>()), Times.Never);

            _mockTransaccionRepository.Verify(repository => repository.GuardarTransaccion(
                It.IsAny<string>(), It.IsAny<Transaccion>()), Times.Never);

            Assert.Equal((int)TipoExcepcionNegocio.TransaccionFallidaCuentaCancelada, exception.code);
        }

        [Fact]
        public async Task TransferirConCuentaOrigenEnEstadoInactivoRetornaExcepcion()
        {
            string idCliente = "1035222333";
            string idCuentaOrigen = "2311111111";
            string idCuentaDestino = "2322222222";
            decimal monto = 500;
            string mensaje = "Pago de Honorarios abogado";

            _mockClienteRepository
                .Setup(repository => repository.ClienteExisteConDocumentoDeIdentidad(It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockClienteRepository
                .Setup(repository => repository.ClienteConDocumentoDeIdentidadTieneCuentaConId(
                    It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockCuentaRepository
                .Setup(repository => repository.ExisteCuentaConId(It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockCuentaRepository
                .Setup(repository => repository.ObtenerCuentaPorId(idCuentaOrigen))
                .ReturnsAsync(GetCuentaOrigenInactiva());

            _mockCuentaRepository
                .Setup(repository => repository.ObtenerCuentaPorId(idCuentaDestino))
                .ReturnsAsync(GetCuentaDestinoOk());

            BusinessException exception =
               await Assert.ThrowsAsync<BusinessException>(async () => await _transferirUseCase.Transferir(
                   idCliente, idCuentaOrigen, idCuentaDestino, monto, mensaje));

            _mockRepository.VerifyAll();

            _mockClienteRepository.Verify(repository => repository.ClienteConDocumentoDeIdentidadTieneCuentaConId(
                   idCliente, idCuentaDestino), Times.Never);

            _mockCuentaRepository.Verify(repository => repository.ActualizarCuenta(
                It.IsAny<string>(), It.IsAny<Cuenta>()), Times.Never);

            _mockTransaccionRepository.Verify(repository => repository.GuardarTransaccion(
                It.IsAny<string>(), It.IsAny<Transaccion>()), Times.Never);

            Assert.Equal((int)TipoExcepcionNegocio.TransaccionFallidaCuentaInactiva, exception.code);
        }

        [Fact]
        public async Task TransferirDeCuentaCorrientePorMontoMayorAlSaldoDisponibleMasSobregiroRetornaExcepcion()
        {
            string idCliente = "1035222333";
            string idCuentaOrigen = "2311111111";
            string idCuentaDestino = "2322222222";
            decimal monto = 2989000;
            string mensaje = "Pago de Honorarios abogado";

            _mockClienteRepository
                .Setup(repository => repository.ClienteExisteConDocumentoDeIdentidad(It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockClienteRepository
                .Setup(repository => repository.ClienteConDocumentoDeIdentidadTieneCuentaConId(
                    It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockCuentaRepository
                .Setup(repository => repository.ExisteCuentaConId(It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockCuentaRepository
                .Setup(repository => repository.ObtenerCuentaPorId(idCuentaOrigen))
                .ReturnsAsync(GetCuentaCorrienteOrigenOk());

            _mockCuentaRepository
                .Setup(repository => repository.ObtenerCuentaPorId(idCuentaDestino))
                .ReturnsAsync(GetCuentaDestinoOk());

            BusinessException exception =
               await Assert.ThrowsAsync<BusinessException>(async () => await _transferirUseCase.Transferir(
                   idCliente, idCuentaOrigen, idCuentaDestino, monto, mensaje));

            _mockRepository.VerifyAll();

            _mockClienteRepository.Verify(repository => repository.ClienteConDocumentoDeIdentidadTieneCuentaConId(
                   idCliente, idCuentaDestino), Times.Never);

            _mockCuentaRepository.Verify(repository => repository.ActualizarCuenta(
                It.IsAny<string>(), It.IsAny<Cuenta>()), Times.Never);

            _mockTransaccionRepository.Verify(repository => repository.GuardarTransaccion(
                It.IsAny<string>(), It.IsAny<Transaccion>()), Times.Never);

            Assert.Equal((int)TipoExcepcionNegocio.SaldoInsuficiente, exception.code);
        }

        [Fact]
        public async Task TransferirDeCuentaCorrienteACorrienteDelMismoDueñoPorMontoMayorAlSaldoDisponibleMasSobregiroRetornaExcepcion()
        {
            string idCliente = "1035222333";
            string idCuentaOrigen = "2311111111";
            string idCuentaDestino = "2322222222";
            decimal monto = 3150000;
            string mensaje = "Pago de Honorarios abogado";

            _mockClienteRepository
                .Setup(repository => repository.ClienteExisteConDocumentoDeIdentidad(It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockClienteRepository
                .Setup(repository => repository.ClienteConDocumentoDeIdentidadTieneCuentaConId(
                    It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockCuentaRepository
                .Setup(repository => repository.ExisteCuentaConId(It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockCuentaRepository
                .Setup(repository => repository.ObtenerCuentaPorId(idCuentaOrigen))
                .ReturnsAsync(GetCuentaCorrienteOrigenOk());

            _mockCuentaRepository
                .Setup(repository => repository.ObtenerCuentaPorId(idCuentaDestino))
                .ReturnsAsync(GetCuentaCorrienteDestinoOk());

            _mockClienteRepository
                .Setup(repository => repository.ClienteConDocumentoDeIdentidadTieneCuentaConId(
                    It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            BusinessException exception =
               await Assert.ThrowsAsync<BusinessException>(async () => await _transferirUseCase.Transferir(
                   idCliente, idCuentaOrigen, idCuentaDestino, monto, mensaje));

            _mockRepository.VerifyAll();

            _mockCuentaRepository.Verify(repository => repository.ActualizarCuenta(
               It.IsAny<string>(), It.IsAny<Cuenta>()), Times.Never);

            _mockTransaccionRepository.Verify(repository => repository.GuardarTransaccion(
                It.IsAny<string>(), It.IsAny<Transaccion>()), Times.Never);

            Assert.Equal((int)TipoExcepcionNegocio.SaldoInsuficiente, exception.code);
        }

        private static Cuenta GetCuentaOrigenOk()
        {
            return new CuentaTestBuilder()
                .ConId("2311111111")
                .ConSaldo(1000)
                .ConSobregiro(0)
                .ConSaldoDisponible(996m)
                .EsNoGravable(false)
                .ConEstadoDeCuenta(EstadosDeCuenta.ACTIVA)
                .ConTipoDeCuenta(TiposDeCuenta.AHORROS)
                .Build();
        }

        private static Cuenta GetCuentaCorrienteOrigenOk()
        {
            return new CuentaTestBuilder()
                .ConId("2311111111")
                .ConSaldo(1000)
                .ConSobregiro(0)
                .ConSaldoDisponible(996m)
                .EsNoGravable(false)
                .ConEstadoDeCuenta(EstadosDeCuenta.ACTIVA)
                .ConTipoDeCuenta(TiposDeCuenta.CORRIENTE)
                .Build();
        }

        private static Cuenta GetCuentaOrigenCancelada()
        {
            return new CuentaTestBuilder()
                .ConId("2311111111")
                .ConSaldo(1000)
                .ConSobregiro(0)
                .ConSaldoDisponible(996m)
                .EsNoGravable(false)
                .ConEstadoDeCuenta(EstadosDeCuenta.CANCELADA)
                .ConTipoDeCuenta(TiposDeCuenta.AHORROS)
                .Build();
        }

        private static Cuenta GetCuentaOrigenInactiva()
        {
            return new CuentaTestBuilder()
                .ConId("2311111111")
                .ConSaldo(1000)
                .ConSobregiro(0)
                .ConSaldoDisponible(996m)
                .EsNoGravable(false)
                .ConEstadoDeCuenta(EstadosDeCuenta.INACTIVA)
                .ConTipoDeCuenta(TiposDeCuenta.AHORROS)
                .Build();
        }

        private static Cuenta GetCuentaOrigenAhorrosActualizada()
        {
            return new CuentaTestBuilder()
                .ConId("2311111111")
                .ConSaldo(498)
                .ConSobregiro(0)
                .ConSaldoDisponible(496.008m)
                .EsNoGravable(false)
                .ConEstadoDeCuenta(EstadosDeCuenta.ACTIVA)
                .ConTipoDeCuenta(TiposDeCuenta.AHORROS)
                .Build();
        }

        private static Cuenta GetCuentaDestinoOk()
        {
            return new CuentaTestBuilder()
                .ConId("2322222222")
                .ConSaldo(1000)
                .ConSobregiro(0)
                .ConSaldoDisponible(996m)
                .EsNoGravable(false)
                .ConEstadoDeCuenta(EstadosDeCuenta.ACTIVA)
                .ConTipoDeCuenta(TiposDeCuenta.AHORROS)
                .Build();
        }

        private static Cuenta GetCuentaCorrienteDestinoOk()
        {
            return new CuentaTestBuilder()
                .ConId("2322222222")
                .ConSaldo(1000)
                .ConSobregiro(0)
                .ConSaldoDisponible(996m)
                .EsNoGravable(false)
                .ConEstadoDeCuenta(EstadosDeCuenta.ACTIVA)
                .ConTipoDeCuenta(TiposDeCuenta.CORRIENTE)
                .Build();
        }

        private static Cuenta GetCuentaDestinoAhorrosActualizada()
        {
            return new CuentaTestBuilder()
                .ConId("2322222222")
                .ConSaldo(1500)
                .ConSobregiro(0)
                .ConSaldoDisponible(1494m)
                .EsNoGravable(false)
                .ConEstadoDeCuenta(EstadosDeCuenta.ACTIVA)
                .ConTipoDeCuenta(TiposDeCuenta.AHORROS)
                .Build();
        }

        private static Transaccion GetTransaccionAhorros()
        {
            return new TransaccionTestBuilder()
                .ConId("11111")
                .ConMonto(500)
                .ConTipoDeTransaccion(TipoDeTransacciones.TRANSACCION_ENTRE_CUENTAS)
                .ConIdCuentaDeDestino("2322222222")
                .ConMensaje("Pago de Honorarios abogado")
                .ConGravamenDelMovimiento(2)
                .Build();
        }
    }
}