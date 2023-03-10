using credinet.comun.api;
using credinet.exception.middleware.models;
using Domain.Model.Entities;
using Domain.Model.Enums;
using Domain.Model.Interfaces.Gateways;
using Domain.UseCase.Transacciones;
using Helpers.Commons.Exceptions;
using Helpers.Domain.EntityBuilders;
using MongoDB.Bson.Serialization.IdGenerators;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Domain.UseCase.Tests.Transacciones
{
    public class ConsignarUseCaseTest
    {
        private readonly MockRepository _mockRepository;
        private readonly Mock<ITransaccionRepository> _mockTransaccionRepository;
        private readonly Mock<ICuentaRepository> _mockCuentaRepository;
        private readonly Mock<IClienteRepository> _mockClienteRepository;
        private readonly ConsignarUseCase _consignarUseCase;

        public ConsignarUseCaseTest()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _mockTransaccionRepository = _mockRepository.Create<ITransaccionRepository>();
            _mockCuentaRepository = _mockRepository.Create<ICuentaRepository>();
            _mockClienteRepository = _mockRepository.Create<IClienteRepository>();
            _consignarUseCase = new(_mockTransaccionRepository.Object, _mockCuentaRepository.Object, _mockClienteRepository.Object);
        }

        [Fact]
        public async Task ConsignarExitoso()
        {
            string idCliente = "1035222333";
            string idCuenta = "2311111111";
            decimal monto = 1000;

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
                .ReturnsAsync(GetCuentaActualizada());

            _mockTransaccionRepository
                .Setup(repository => repository.GuardarTransaccion(It.IsAny<string>(), It.IsAny<Transaccion>()))
                .ReturnsAsync(GetTransaccion());

            Transaccion transaccionCreada = await _consignarUseCase.Consignar(idCliente, idCuenta, monto);

            _mockRepository.VerifyAll();

            Assert.NotNull(transaccionCreada);
            Assert.NotNull(transaccionCreada.Id);
            Assert.Equal(monto, transaccionCreada.Monto);
            Assert.Equal(TipoDeTransacciones.CONSIGNACION, transaccionCreada.TipoDeTransaccion);
            Assert.Equal(0, transaccionCreada.GravamenDelMovimiento);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task ConsignarConMontoIgualOInferiorACeroRetornaExcepcion(decimal monto)
        {
            string idCliente = "1035222333";
            string idCuenta = "2311111111";

            BusinessException exception =
                await Assert.ThrowsAsync<BusinessException>(async () => await _consignarUseCase.Consignar(
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
        public async Task ConsignarConClienteConDocumentoDeIdentidadInexistenteRetornaExcepcion()
        {
            string idCliente = "1035222333";
            string idCuenta = "2311111111";
            decimal monto = 1000;

            _mockClienteRepository
                .Setup(repository => repository.ClienteExisteConDocumentoDeIdentidad(It.IsAny<string>()))
                .ReturnsAsync(false);

            BusinessException exception =
                await Assert.ThrowsAsync<BusinessException>(async () => await _consignarUseCase.Consignar(
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
        public async Task ConsignarConClienteQueNoTieneLaCuentaRetornaExcepcion()
        {
            string idCliente = "1035222333";
            string idCuenta = "2311111111";
            decimal monto = 1000;

            _mockClienteRepository
                .Setup(repository => repository.ClienteExisteConDocumentoDeIdentidad(It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockClienteRepository
                .Setup(repository => repository.ClienteConDocumentoDeIdentidadTieneCuentaConId(
                    It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            BusinessException exception =
                await Assert.ThrowsAsync<BusinessException>(async () => await _consignarUseCase.Consignar(
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
        public async Task ConsignarConCuentaEnEstadoCanceladoRetornaExcepcion()
        {
            string idCliente = "1035222333";
            string idCuenta = "2311111111";
            decimal monto = 1000;

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
                await Assert.ThrowsAsync<BusinessException>(async () => await _consignarUseCase.Consignar(
                    idCliente, idCuenta, monto));

            _mockRepository.VerifyAll();

            _mockCuentaRepository.Verify(repository => repository.ActualizarCuenta(
                It.IsAny<string>(), It.IsAny<Cuenta>()), Times.Never);

            _mockTransaccionRepository.Verify(repository => repository.GuardarTransaccion(
                It.IsAny<string>(), It.IsAny<Transaccion>()), Times.Never);

            Assert.Equal((int)TipoExcepcionNegocio.TransaccionFallidaCuentaCancelada, exception.code);
        }

        private Cuenta GetCuentaOk()
        {
            return new CuentaTestBuilder()
                .ConId("2311111111")
                .ConSaldo(200)
                .ConSobregiro(0)
                .ConSaldoDisponible(199.2m)
                .EsNoGravable(false)
                .ConEstadoDeCuenta(EstadosDeCuenta.ACTIVA)
                .ConTipoDeCuenta(TiposDeCuenta.AHORROS)
                .Build();
        }

        private Cuenta GetCuentaActualizada()
        {
            return new CuentaTestBuilder()
                .ConId("2311111111")
                .ConSaldo(1200)
                .ConSobregiro(0)
                .ConSaldoDisponible(1195.2m)
                .EsNoGravable(false)
                .ConEstadoDeCuenta(EstadosDeCuenta.ACTIVA)
                .ConTipoDeCuenta(TiposDeCuenta.AHORROS)
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

        private Transaccion GetTransaccion()
        {
            return new TransaccionTestBuilder()
                .ConId("11111")
                .ConMonto(1000)
                .ConTipoDeTransaccion(TipoDeTransacciones.CONSIGNACION)
                .ConGravamenDelMovimiento(0)
                .Build();
        }
    }
}