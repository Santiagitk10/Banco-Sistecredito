using Domain.Model.Entities;
using Domain.Model.Enums;
using Domain.Model.Interfaces.Gateways;
using Domain.UseCase.Cuentas;
using Helpers.Domain.EntityBuilders;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Domain.UseCase.Tests.Cuentas
{
    public class CrearCuentaUseCaseTest
    {
        private readonly Mock<ICuentaRepository> _repositoryMock;

        public CrearCuentaUseCaseTest()
        {
            _repositoryMock = new();
        }

        [Fact]
        public async Task CrearCuentaUseCase_FallidoPrimerCicloPorIdRepetido()
        {
            _repositoryMock.SetupSequence(r => r.ExisteCuentaConId(It.IsAny<string>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            _repositoryMock.Setup(r => r.EncontrarCuentasPorDocumentoDelUsuario(It.IsAny<string>()))
                .ReturnsAsync(new List<Cuenta>() { });
            _repositoryMock.Setup(r => r.GuardarCuenta(It.IsAny<string>(), It.IsAny<Cuenta>()))
                .ReturnsAsync(GetCuentaAhorrosActivaConId());

            var useCase = new CrearCuentaUseCase(_repositoryMock.Object);

            var result = await useCase.CrearCuenta("1", GetCuentaAhorrosActivaSinId());

            Assert.Equal(0, result.Saldo);
            Assert.Equal("46", result.Id[..2]);
        }
        [Fact]
        public async Task CrearCuentaAhorrosUseCase_ExitoIdNoRepetido()
        {
            _repositoryMock.Setup(r => r.ExisteCuentaConId(It.IsAny<string>()))
                .ReturnsAsync(false);
            _repositoryMock.Setup(r => r.EncontrarCuentasPorDocumentoDelUsuario(It.IsAny<string>()))
                .ReturnsAsync(new List<Cuenta>() { });
            _repositoryMock.Setup(r => r.GuardarCuenta(It.IsAny<string>(), It.IsAny<Cuenta>()))
                .ReturnsAsync(GetCuentaAhorrosActivaConId());

            var useCase = new CrearCuentaUseCase(_repositoryMock.Object);

            var result = await useCase.CrearCuenta("1", GetCuentaAhorrosActivaSinId());

            Assert.Equal(0, result.Saldo);
            Assert.Equal("46", result.Id[..2]);
        }
        [Fact]
        public async Task CrearCuentaCorrienteUseCase_ExitoIdNoRepetido()
        {
            _repositoryMock.Setup(r => r.ExisteCuentaConId(It.IsAny<string>()))
                .ReturnsAsync(false);
            _repositoryMock.Setup(r => r.EncontrarCuentasPorDocumentoDelUsuario(It.IsAny<string>()))
                .ReturnsAsync(new List<Cuenta>() { });
            _repositoryMock.Setup(r => r.GuardarCuenta(It.IsAny<string>(), It.IsAny<Cuenta>()))
                .ReturnsAsync(GetCuentaCorrienteActivaConId());

            var useCase = new CrearCuentaUseCase(_repositoryMock.Object);

            var result = await useCase.CrearCuenta("1", GetCuentaCorrienteActivaSinId());

            Assert.Equal(0, result.Saldo);
            Assert.Equal("23", result.Id[..2]);
        }

        [Fact]
        public async Task CrearPrimeraCuentaAhorros_VerificarCreadaComoNoGravable()
        {
            _repositoryMock.Setup(r => r.ExisteCuentaConId(It.IsAny<string>()))
                .ReturnsAsync(false);
            _repositoryMock.Setup(r => r.EncontrarCuentasPorDocumentoDelUsuario(It.IsAny<string>()))
                .ReturnsAsync(new List<Cuenta>() { });
            _repositoryMock.Setup(r => r.GuardarCuenta(It.IsAny<string>(), It.IsAny<Cuenta>()))
                .ReturnsAsync(GetCuentaAhorrosActivaConId());

            var useCase = new CrearCuentaUseCase(_repositoryMock.Object);

            var result = await useCase.CrearCuenta("1", GetCuentaAhorrosActivaSinId());

            Assert.Equal(EstadosDeCuenta.ACTIVA, result.EstadoDeCuenta);
            Assert.True(result.EsNoGravable);

        }
        [Fact]
        public async Task CrearSegundaCuentaAhorros_VerificarCreadaComoGravable()
        {
            _repositoryMock.Setup(r => r.ExisteCuentaConId(It.IsAny<string>()))
                .ReturnsAsync(false);
            _repositoryMock.Setup(r => r.EncontrarCuentasPorDocumentoDelUsuario(It.IsAny<string>()))
                .ReturnsAsync(new List<Cuenta>() { GetCuentaAhorrosActivaConId() });
            _repositoryMock.Setup(r => r.GuardarCuenta(It.IsAny<string>(), It.IsAny<Cuenta>()))
                .ReturnsAsync(GetCuentaAhorrosActivaConId());

            var useCase = new CrearCuentaUseCase(_repositoryMock.Object);

            var result = await useCase.CrearCuenta("1", GetCuentaAhorrosActivaSinId());

            Assert.Equal(EstadosDeCuenta.ACTIVA, result.EstadoDeCuenta);
            Assert.False(result.EsNoGravable);
        }
        [Fact]
        public async Task CrearCuentaCorriente_VerificarCreadaComoGravable()
        {
            _repositoryMock.Setup(r => r.ExisteCuentaConId(It.IsAny<string>()))
                .ReturnsAsync(false);
            _repositoryMock.Setup(r => r.EncontrarCuentasPorDocumentoDelUsuario(It.IsAny<string>()))
                .ReturnsAsync(new List<Cuenta>() { });
            _repositoryMock.Setup(r => r.GuardarCuenta(It.IsAny<string>(), It.IsAny<Cuenta>()))
                .ReturnsAsync(GetCuentaCorrienteActivaConId());

            var useCase = new CrearCuentaUseCase(_repositoryMock.Object);

            var result = await useCase.CrearCuenta("1", GetCuentaCorrienteActivaSinId());

            Assert.Equal(EstadosDeCuenta.ACTIVA, result.EstadoDeCuenta);
            Assert.False(result.EsNoGravable);

        }
        private Cuenta GetCuentaAhorrosActivaSinId()
        {
            return new CuentaTestBuilder()
                .ConEstadoDeCuenta(Model.Enums.EstadosDeCuenta.CANCELADA)
                .ConSaldo(0)
                .ConTipoDeCuenta(Model.Enums.TiposDeCuenta.AHORROS)
                .Build();
        }
        private Cuenta GetCuentaAhorrosActivaConId()
        {
            return new CuentaTestBuilder()
                .ConEstadoDeCuenta(Model.Enums.EstadosDeCuenta.CANCELADA)
                .ConId("46")
                .ConSaldo(0)
                .ConTipoDeCuenta(Model.Enums.TiposDeCuenta.AHORROS)
                .Build();
        }
        private Cuenta GetCuentaCorrienteActivaSinId()
        {
            return new CuentaTestBuilder()
                .ConEstadoDeCuenta(Model.Enums.EstadosDeCuenta.CANCELADA)
                .ConSaldo(0)
                .ConTipoDeCuenta(Model.Enums.TiposDeCuenta.CORRIENTE)
                .Build();
        }
        private Cuenta GetCuentaCorrienteActivaConId()
        {
            return new CuentaTestBuilder()
                .ConEstadoDeCuenta(Model.Enums.EstadosDeCuenta.CANCELADA)
                .ConSaldo(0)
                .ConId("23")
                .ConTipoDeCuenta(Model.Enums.TiposDeCuenta.CORRIENTE)
                .Build();
        }
    }
}
