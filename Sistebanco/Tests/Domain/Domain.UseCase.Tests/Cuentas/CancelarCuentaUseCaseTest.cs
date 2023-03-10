using credinet.exception.middleware.models;
using Domain.Model.Entities;
using Domain.Model.Interfaces.Gateways;
using Domain.UseCase.Cuentas;
using Helpers.Commons.Exceptions;
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
    public class CancelarCuentaUseCaseTest
    {
        private readonly Mock<ICuentaRepository> _mockRepositorioCuenta;

        public CancelarCuentaUseCaseTest()
        {
            _mockRepositorioCuenta = new();
        }
        [Fact]
        public async Task CancelarCuentaSatisfactorio_Test() {

            _mockRepositorioCuenta.Setup(r => r.ExisteCuentaConId(It.IsAny<string>()))
                .ReturnsAsync(true);
            _mockRepositorioCuenta.Setup(r => r.ActualizarCuenta(It.IsAny<string>(), It.IsAny<Cuenta>()))
                .ReturnsAsync(GetCuentaCancelada());
            _mockRepositorioCuenta.Setup(r => r.ObtenerCuentaPorId(It.IsAny<string>()))
                .ReturnsAsync(GetCuentaActiva());

            var useCase = new CancelarCuentaUseCase(_mockRepositorioCuenta.Object);

            await useCase.CancelarCuenta("1");

            _mockRepositorioCuenta.Verify(m => m.ActualizarCuenta(It.IsAny<string>(), It.IsAny<Cuenta>()),Times.Once());
        }
        [Fact]
        public async Task CancelarCuentaUseCaseTest_FallidoPorCuentaConSaldoMayorQueCero()
        {
            _mockRepositorioCuenta.Setup(r => r.ExisteCuentaConId(It.IsAny<string>()))
                .ReturnsAsync(true);
            _mockRepositorioCuenta.Setup(r => r.ObtenerCuentaPorId(It.IsAny<string>()))
                .ReturnsAsync(GetCuentaConSaldomayorQueCero());

            var useCase = new CancelarCuentaUseCase(_mockRepositorioCuenta.Object);

            var result = await Assert.ThrowsAsync<BusinessException>(async () => await useCase.CancelarCuenta("1"));

            Assert.Equal(5, (int)TipoExcepcionNegocio.CancelacionFallidaPorEstadoDeCuentaInvalido);
        }
        [Fact]
        public async Task CancelarCuentaUseCaseTest_FallidoPorCuentaConSobregiroMayorQueCero()
        {
            _mockRepositorioCuenta.Setup(r => r.ExisteCuentaConId(It.IsAny<string>()))
                .ReturnsAsync(true);
            _mockRepositorioCuenta.Setup(r => r.ObtenerCuentaPorId(It.IsAny<string>()))
                .ReturnsAsync(GetCuentaConSobregiroMayorQueCero());

            var useCase = new CancelarCuentaUseCase(_mockRepositorioCuenta.Object);

            var result = await Assert.ThrowsAsync<BusinessException>(async () => await useCase.CancelarCuenta("1"));

            Assert.Equal(5, (int)TipoExcepcionNegocio.CancelacionFallidaPorEstadoDeCuentaInvalido);
        }
        [Fact]
        public async Task CancelarCuentaUseCaseTest_FallidoPorNoEncontrarCuenta()
        {
            _mockRepositorioCuenta.Setup(r => r.ExisteCuentaConId(It.IsAny<string>()))
                .ReturnsAsync(false);

            var useCase = new CancelarCuentaUseCase(_mockRepositorioCuenta.Object);

            var result = await Assert.ThrowsAsync<BusinessException>(async () => await useCase.CancelarCuenta("1"));

            Assert.Equal(2, (int)TipoExcepcionNegocio.CuentaNoExisteEnCliente);
        }

        private Cuenta GetCuentaCancelada()
        {
            return new CuentaTestBuilder()
                .ConId("1")
                .ConSaldo(0)
                .ConEstadoDeCuenta(Model.Enums.EstadosDeCuenta.CANCELADA)
                .ConTipoDeCuenta(Model.Enums.TiposDeCuenta.AHORROS)
                .Build();
        }

        private Cuenta GetCuentaActiva()
        {
            return new CuentaTestBuilder()
                .ConId("1")
                .ConEstadoDeCuenta(Model.Enums.EstadosDeCuenta.CANCELADA)
                .ConSaldo(0)
                .ConTipoDeCuenta(Model.Enums.TiposDeCuenta.AHORROS)
                .Build();
        }

        private Cuenta GetCuentaConSaldomayorQueCero()
        {
            return new CuentaTestBuilder()
                .ConId("1")
                .ConEstadoDeCuenta(Model.Enums.EstadosDeCuenta.CANCELADA)
                .ConSaldo(20)
                .ConSobregiro(0)
                .ConTipoDeCuenta(Model.Enums.TiposDeCuenta.AHORROS)
                .Build();
        }
        private Cuenta GetCuentaConSobregiroMayorQueCero()
        {
            return new CuentaTestBuilder()
                .ConId("1")
                .ConEstadoDeCuenta(Model.Enums.EstadosDeCuenta.CANCELADA)
                .ConSaldo(0)
                .ConSobregiro(20)
                .ConTipoDeCuenta(Model.Enums.TiposDeCuenta.AHORROS)
                .Build();
        }
        private Cuenta GetCuentaConSobregiroYSaldoMayorQueCero()
        {
            return new CuentaTestBuilder()
                .ConId("1")
                .ConEstadoDeCuenta(Model.Enums.EstadosDeCuenta.CANCELADA)
                .ConSaldo(20)
                .ConSobregiro(20)
                .ConTipoDeCuenta(Model.Enums.TiposDeCuenta.AHORROS)
                .Build();
        }
    }
}
