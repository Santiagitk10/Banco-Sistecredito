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
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Domain.UseCase.Tests.Transacciones
{
    public class RetirarUseCaseTest
    {
        private readonly MockRepository _mockRepository;
        private readonly Mock<ITransaccionRepository> _mockTransaccionRepository;
        private readonly Mock<ICuentaRepository> _mockCuentaRepository;
        private readonly Mock<IClienteRepository> _mockClienteRepository;
        private readonly RetirarUseCase _retirarUseCase;

        public RetirarUseCaseTest()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _mockTransaccionRepository = _mockRepository.Create<ITransaccionRepository>();
            _mockCuentaRepository = _mockRepository.Create<ICuentaRepository>();
            _mockClienteRepository = _mockRepository.Create<IClienteRepository>();
            _retirarUseCase = new(_mockTransaccionRepository.Object, _mockCuentaRepository.Object, _mockClienteRepository.Object);
        }

        [Fact]
        public async Task RetirarExitoso()
        {
            string idCliente = "1035222333";
            string idCuenta = "2311111111";
            decimal monto = 500;

            _mockClienteRepository
                .Setup(repository => repository.ClienteExisteConDocumentoDeIdentidad(It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockClienteRepository
                .Setup(repository => repository.ClienteConDocumentoDeIdentidadTieneCuentaConId(
                    It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockCuentaRepository
                .Setup(repository => repository.ObtenerCuentaPorId(It.IsAny<string>()))
                .ReturnsAsync(GetCuentaOk());

            _mockCuentaRepository
                .Setup(repository => repository.ActualizarCuenta(It.IsAny<string>(), It.IsAny<Cuenta>()))
                .ReturnsAsync(GetCuentaActualizadaAhorros());

            _mockTransaccionRepository
                .Setup(repository => repository.GuardarTransaccion(It.IsAny<string>(), It.IsAny<Transaccion>()))
                .ReturnsAsync(GetTransaccionAhorros());

            Transaccion transaccionCreada = await _retirarUseCase.Retirar(idCliente, idCuenta, monto);

            _mockRepository.VerifyAll();

            Assert.NotNull(transaccionCreada);
            Assert.NotNull(transaccionCreada.Id);
            Assert.Equal(monto, transaccionCreada.Monto);
            Assert.Equal(TipoDeTransacciones.RETIRO, transaccionCreada.TipoDeTransaccion);
            Assert.Equal(2m, transaccionCreada.GravamenDelMovimiento);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task RetirarConMontoIgualOInferiorACeroRetornaExcepcion(decimal monto)
        {
            string idCliente = "1035222333";
            string idCuenta = "2311111111";

            BusinessException exception =
                await Assert.ThrowsAsync<BusinessException>(async () => await _retirarUseCase.Retirar(
                    idCliente, idCuenta, monto));

            _mockRepository.VerifyAll();

            _mockClienteRepository.Verify(repository => repository.ClienteExisteConDocumentoDeIdentidad(
                It.IsAny<string>()), Times.Never);

            _mockClienteRepository.Verify(repository => repository.ClienteConDocumentoDeIdentidadTieneCuentaConId(
                    It.IsAny<string>(), It.IsAny<string>()), Times.Never);

            _mockCuentaRepository.Verify(repository => repository.ObtenerCuentaPorId(It.IsAny<string>()), Times.Never);

            _mockCuentaRepository.Verify(repository => repository.ActualizarCuenta(
                It.IsAny<string>(), It.IsAny<Cuenta>()), Times.Never);

            _mockTransaccionRepository.Verify(repository => repository.GuardarTransaccion(
                It.IsAny<string>(), It.IsAny<Transaccion>()), Times.Never);

            Assert.Equal((int)TipoExcepcionNegocio.TransaccionFallidaMontoInvalido, exception.code);
        }

        [Fact]
        public async Task RetirarConClienteConDocumentoDeIdentidadInexistenteRetornaExcepcion()
        {
            string idCliente = "1035222333";
            string idCuenta = "2311111111";
            decimal monto = 500;

            _mockClienteRepository
                .Setup(repository => repository.ClienteExisteConDocumentoDeIdentidad(It.IsAny<string>()))
                .ReturnsAsync(false);

            BusinessException exception =
                await Assert.ThrowsAsync<BusinessException>(async () => await _retirarUseCase.Retirar(
                    idCliente, idCuenta, monto));

            _mockRepository.VerifyAll();

            _mockClienteRepository.Verify(repository => repository.ClienteConDocumentoDeIdentidadTieneCuentaConId(
                    It.IsAny<string>(), It.IsAny<string>()), Times.Never);

            _mockCuentaRepository.Verify(repository => repository.ObtenerCuentaPorId(It.IsAny<string>()), Times.Never);

            _mockCuentaRepository.Verify(repository => repository.ActualizarCuenta(
                It.IsAny<string>(), It.IsAny<Cuenta>()), Times.Never);

            _mockTransaccionRepository.Verify(repository => repository.GuardarTransaccion(
                It.IsAny<string>(), It.IsAny<Transaccion>()), Times.Never);

            Assert.Equal((int)TipoExcepcionNegocio.ClienteConDocumentoDeIdentidadNoExiste, exception.code);
        }

        [Fact]
        public async Task RetirarConClienteQueNoTieneLaCuentaRetornaExcepcion()
        {
            string idCliente = "1035222333";
            string idCuenta = "2311111111";
            decimal monto = 500;

            _mockClienteRepository
                .Setup(repository => repository.ClienteExisteConDocumentoDeIdentidad(It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockClienteRepository
                .Setup(repository => repository.ClienteConDocumentoDeIdentidadTieneCuentaConId(
                    It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            BusinessException exception =
                await Assert.ThrowsAsync<BusinessException>(async () => await _retirarUseCase.Retirar(
                    idCliente, idCuenta, monto));

            _mockRepository.VerifyAll();

            _mockCuentaRepository.Verify(repository => repository.ObtenerCuentaPorId(It.IsAny<string>()), Times.Never);

            _mockCuentaRepository.Verify(repository => repository.ActualizarCuenta(
                It.IsAny<string>(), It.IsAny<Cuenta>()), Times.Never);

            _mockTransaccionRepository.Verify(repository => repository.GuardarTransaccion(
                It.IsAny<string>(), It.IsAny<Transaccion>()), Times.Never);

            Assert.Equal((int)TipoExcepcionNegocio.CuentaNoExisteEnCliente, exception.code);
        }

        [Fact]
        public async Task RetirarConCuentaEnEstadoCanceladoRetornaExcepcion()
        {
            string idCliente = "1035222333";
            string idCuenta = "2311111111";
            decimal monto = 500;

            _mockClienteRepository
                .Setup(repository => repository.ClienteExisteConDocumentoDeIdentidad(It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockClienteRepository
                .Setup(repository => repository.ClienteConDocumentoDeIdentidadTieneCuentaConId(
                    It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockCuentaRepository
                .Setup(repository => repository.ObtenerCuentaPorId(It.IsAny<string>()))
                .ReturnsAsync(GetCuentaCancelada());

            BusinessException exception =
                await Assert.ThrowsAsync<BusinessException>(async () => await _retirarUseCase.Retirar(
                    idCliente, idCuenta, monto));

            _mockRepository.VerifyAll();

            _mockCuentaRepository.Verify(repository => repository.ActualizarCuenta(
               It.IsAny<string>(), It.IsAny<Cuenta>()), Times.Never);

            _mockTransaccionRepository.Verify(repository => repository.GuardarTransaccion(
                It.IsAny<string>(), It.IsAny<Transaccion>()), Times.Never);

            Assert.Equal((int)TipoExcepcionNegocio.TransaccionFallidaCuentaCancelada, exception.code);
        }

        [Fact]
        public async Task RetirarConCuentaEnEstadoInactivoRetornaExcepcion()
        {
            string idCliente = "1035222333";
            string idCuenta = "2311111111";
            decimal monto = 500;

            _mockClienteRepository
                .Setup(repository => repository.ClienteExisteConDocumentoDeIdentidad(It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockClienteRepository
                .Setup(repository => repository.ClienteConDocumentoDeIdentidadTieneCuentaConId(
                    It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockCuentaRepository
                .Setup(repository => repository.ObtenerCuentaPorId(It.IsAny<string>()))
                .ReturnsAsync(GetCuentaInactiva());

            BusinessException exception =
                await Assert.ThrowsAsync<BusinessException>(async () => await _retirarUseCase.Retirar(
                    idCliente, idCuenta, monto));

            _mockRepository.VerifyAll();

            _mockCuentaRepository.Verify(repository => repository.ActualizarCuenta(
               It.IsAny<string>(), It.IsAny<Cuenta>()), Times.Never);

            _mockTransaccionRepository.Verify(repository => repository.GuardarTransaccion(
                It.IsAny<string>(), It.IsAny<Transaccion>()), Times.Never);

            Assert.Equal((int)TipoExcepcionNegocio.TransaccionFallidaCuentaInactiva, exception.code);
        }

        [Fact]
        public async Task RetiroDeCuentaCorrientePorMontoMayorAlSaldoDisponibleMasSobregiroRetornaExcepcion()
        {
            string idCliente = "1035222333";
            string idCuenta = "2311111111";
            decimal monto = 5000000;

            _mockClienteRepository
                .Setup(repository => repository.ClienteExisteConDocumentoDeIdentidad(It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockClienteRepository
                .Setup(repository => repository.ClienteConDocumentoDeIdentidadTieneCuentaConId(
                    It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockCuentaRepository
                .Setup(repository => repository.ObtenerCuentaPorId(It.IsAny<string>()))
                .ReturnsAsync(GetCuentaOkCorriente());

            BusinessException exception =
                await Assert.ThrowsAsync<BusinessException>(async () => await _retirarUseCase.Retirar(
                    idCliente, idCuenta, monto));

            _mockRepository.VerifyAll();

            _mockCuentaRepository.Verify(repository => repository.ActualizarCuenta(
               It.IsAny<string>(), It.IsAny<Cuenta>()), Times.Never);

            _mockTransaccionRepository.Verify(repository => repository.GuardarTransaccion(
                It.IsAny<string>(), It.IsAny<Transaccion>()), Times.Never);

            Assert.Equal((int)TipoExcepcionNegocio.SaldoInsuficiente, exception.code);
        }

        [Fact]
        public async Task RetiroDeCuentaAhorrosPorMontoMayorAlSaldoDisponibleRetornaExcepcion()
        {
            string idCliente = "1035222333";
            string idCuenta = "2311111111";
            decimal monto = 997;

            _mockClienteRepository
                .Setup(repository => repository.ClienteExisteConDocumentoDeIdentidad(It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockClienteRepository
                .Setup(repository => repository.ClienteConDocumentoDeIdentidadTieneCuentaConId(
                    It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockCuentaRepository
                .Setup(repository => repository.ObtenerCuentaPorId(It.IsAny<string>()))
                .ReturnsAsync(GetCuentaOk());

            BusinessException exception =
                await Assert.ThrowsAsync<BusinessException>(async () => await _retirarUseCase.Retirar(
                    idCliente, idCuenta, monto));

            _mockRepository.VerifyAll();

            _mockCuentaRepository.Verify(repository => repository.ActualizarCuenta(
               It.IsAny<string>(), It.IsAny<Cuenta>()), Times.Never);

            _mockTransaccionRepository.Verify(repository => repository.GuardarTransaccion(
                It.IsAny<string>(), It.IsAny<Transaccion>()), Times.Never);

            Assert.Equal((int)TipoExcepcionNegocio.SaldoInsuficiente, exception.code);
        }

        private Cuenta GetCuentaOk()
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

        private Cuenta GetCuentaOkCorriente()
        {
            return new CuentaTestBuilder()
                .ConId("2311111111")
                .ConSaldo(1000m)
                .ConSobregiro(2900m)
                .ConSaldoDisponible(996m)
                .EsNoGravable(false)
                .ConEstadoDeCuenta(EstadosDeCuenta.ACTIVA)
                .ConTipoDeCuenta(TiposDeCuenta.CORRIENTE)
                .Build();
        }

        private Cuenta GetCuentaActualizadaAhorros()
        {
            return new CuentaTestBuilder()
                .ConId("2311111111")
                .ConSaldo(500)
                .ConSobregiro(0)
                .ConSaldoDisponible(498m)
                .EsNoGravable(false)
                .ConEstadoDeCuenta(EstadosDeCuenta.ACTIVA)
                .ConTipoDeCuenta(TiposDeCuenta.AHORROS)
                .Build();
        }

        private Transaccion GetTransaccionAhorros()
        {
            return new TransaccionTestBuilder()
                .ConId("11111")
                .ConMonto(500)
                .ConTipoDeTransaccion(TipoDeTransacciones.RETIRO)
                .ConGravamenDelMovimiento(2)
                .Build();
        }

        private Cuenta GetCuentaCancelada()
        {
            return new CuentaTestBuilder()
                .ConId("2311111111")
                .ConSaldo(0)
                .ConSobregiro(0)
                .ConSaldoDisponible(0)
                .EsNoGravable(false)
                .ConEstadoDeCuenta(EstadosDeCuenta.CANCELADA)
                .ConTipoDeCuenta(TiposDeCuenta.AHORROS)
                .Build();
        }

        private Cuenta GetCuentaInactiva()
        {
            return new CuentaTestBuilder()
                .ConId("2311111111")
                .ConSaldo(1000)
                .ConSobregiro(200)
                .ConSaldoDisponible(996m)
                .EsNoGravable(false)
                .ConEstadoDeCuenta(EstadosDeCuenta.INACTIVA)
                .ConTipoDeCuenta(TiposDeCuenta.AHORROS)
                .Build();
        }
    }
}