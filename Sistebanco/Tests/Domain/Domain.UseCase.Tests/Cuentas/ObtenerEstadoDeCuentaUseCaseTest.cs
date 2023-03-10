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
    public class ObtenerEstadoDeCuentaUseCaseTest
    {
        private readonly Mock<ICuentaRepository> _repositoryMock;
        private readonly Mock<ITransaccionRepository> _transaccionRepositoryMock;

        public ObtenerEstadoDeCuentaUseCaseTest()
        {
            _repositoryMock = new();
            _transaccionRepositoryMock = new();
        }

        [Fact]
        public async Task ObtenerEstadoDeCuentaUseCaseTest_Exitoso()
        {
            _repositoryMock.Setup(r => r.ExisteCuentaConId(It.IsAny<string>()))
                .ReturnsAsync(true);

            _repositoryMock.Setup(r => r.ObtenerCuentaPorId(It.IsAny<string>()))
                .ReturnsAsync(GetCuenta());

            _transaccionRepositoryMock.Setup(r => r.ObtenerPorIdDeCuenta(It.IsAny<string>()))
                .ReturnsAsync(new List<Transaccion>());

            var useCase = new ObtenerEstadoDeCuentaUseCase(_repositoryMock.Object, _transaccionRepositoryMock.Object);

            var result = await useCase.ObtenerEstadoDeCuenta("2");

            Assert.Equal(10000, result.Saldo);
            Assert.Equal(1000, result.Sobregiro);
        }

        [Fact]
        public async Task ObtenerEstadoDeCuentaUseCaseTest_FallidoPorIdNoEncontrado()
        {
            _repositoryMock.Setup(r => r.ExisteCuentaConId(It.IsAny<string>()))
                .ReturnsAsync(false);

            _transaccionRepositoryMock.Setup(r => r.ObtenerPorIdDeCuenta(It.IsAny<string>()))
                .ReturnsAsync(new List<Transaccion>());

            var useCase = new ObtenerEstadoDeCuentaUseCase(_repositoryMock.Object, _transaccionRepositoryMock.Object);

            var result = await Assert.ThrowsAsync<BusinessException>(async () => await useCase.ObtenerEstadoDeCuenta("2"));
            
            _repositoryMock.Verify(r => r.ObtenerCuentaPorId(It.IsAny<string>()), Times.Never());
            Assert.Equal(2, (int)TipoExcepcionNegocio.CuentaNoExisteEnCliente);
        }

        private Cuenta GetCuenta()
        {
            return new CuentaTestBuilder()
                .ConId("1")
                .ConSaldo(10000)
                .ConSobregiro(1000)
                .EsNoGravable(true)
                .ConEstadoDeCuenta(Model.Enums.EstadosDeCuenta.CANCELADA)
                .ConTipoDeCuenta(Model.Enums.TiposDeCuenta.AHORROS)
                .Build();
        }
    }
}
