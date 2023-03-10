using credinet.exception.middleware.models;
using Domain.Model.Entities;
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
    public class MarcarExentaDeGravamenUseCaseTest
    {
        private readonly Mock<ICuentaRepository> _repositoryMock;

        public MarcarExentaDeGravamenUseCaseTest()
        {
            _repositoryMock = new();
        } 
        [Fact]
        public async Task MarcarCuentaComoExentaDeGravamen_Exitoso()
        {
            _repositoryMock.Setup(r => r.EncontrarCuentasPorDocumentoDelUsuario(It.IsAny<string>()))
                .ReturnsAsync(GetCuentas());

            var useCase = new MarcarExentaDeGravamenUseCase(_repositoryMock.Object);

            var result = await useCase.MarcarExentaDeGravamen("1", "1");

            Assert.True(result.EsNoGravable);
        }

        [Fact]
        public async Task MarcarCuentaComoExentaDeGravamen_FalloAlNoEncontrarCuentaConIdActiva()
        {
            _repositoryMock.Setup(r => r.EncontrarCuentasPorDocumentoDelUsuario(It.IsAny<string>()))
                .ReturnsAsync(GetCuentas());

            var useCase = new MarcarExentaDeGravamenUseCase(_repositoryMock.Object);

            await Assert.ThrowsAsync<BusinessException>(async () => await useCase.MarcarExentaDeGravamen("1", "3"));
           
        }

        [Fact]
        public async Task MarcarCuentaComoExentaDeGravamen_FalloAlNoEncontrarCuentaConId()
        {
            _repositoryMock.Setup(r => r.EncontrarCuentasPorDocumentoDelUsuario(It.IsAny<string>()))
                .ReturnsAsync(GetCuentas());

            var useCase = new MarcarExentaDeGravamenUseCase(_repositoryMock.Object);

            await Assert.ThrowsAsync<BusinessException>(async () => await useCase.MarcarExentaDeGravamen("1", "4"));

        }

        private List<Cuenta> GetCuentas()
        {
            return new List<Cuenta>
            {
                new CuentaTestBuilder()
                .ConId("1")
                .ConEstadoDeCuenta(Model.Enums.EstadosDeCuenta.ACTIVA)
                .EsNoGravable(false)
                .Build(),
                
                new CuentaTestBuilder()
                .ConId("2")
                .ConEstadoDeCuenta(Model.Enums.EstadosDeCuenta.ACTIVA)
                .EsNoGravable(true)
                .Build(),
                
                new CuentaTestBuilder()
                .ConId("3")
                .ConEstadoDeCuenta(Model.Enums.EstadosDeCuenta.INACTIVA)
                .EsNoGravable(true)
                .Build()
            };
        }

        private Cuenta GetCuentaConGravamen()
        {
            return new CuentaTestBuilder()
                .ConId("1")
                .ConSaldo(0)
                .EsNoGravable(true)
                .ConEstadoDeCuenta(Model.Enums.EstadosDeCuenta.CANCELADA)
                .ConTipoDeCuenta(Model.Enums.TiposDeCuenta.AHORROS)
                .Build();
        }

        private Cuenta GetCuentaSinGravamen() 
        {
            return new CuentaTestBuilder()
                .ConId("1")
                .ConSaldo(0)
                .EsNoGravable(false)
                .ConEstadoDeCuenta(Model.Enums.EstadosDeCuenta.CANCELADA)
                .ConTipoDeCuenta(Model.Enums.TiposDeCuenta.AHORROS)
                .Build();
        }

    }
}
