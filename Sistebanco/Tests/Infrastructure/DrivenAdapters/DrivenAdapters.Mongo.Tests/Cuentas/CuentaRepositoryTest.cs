using Domain.Model.Entities;
using Domain.Model.Enums;
using DrivenAdapters.Mongo.Entities;
using Helpers.Domain.EntityBuilders;
using Helpers.Domain.MongoEntityBuilder;
using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DrivenAdapters.Mongo.Tests.Cuentas
{
    public class CuentaRepositoryTest
    {
        private readonly Mock<IContext> _mockContext;
        private readonly Mock<IMongoCollection<CuentaEntity>> _mockColeccionCuenta;
        private readonly Mock<IMongoCollection<TransaccionEntity>> _mockColeccionTransaccion;
        private readonly Mock<IAsyncCursor<CuentaEntity>> _cuentaCursor;
        private readonly Mock<IAsyncCursor<TransaccionEntity>> _transaccionCursor;

        public CuentaRepositoryTest()
        {
            _mockContext = new();
            _mockColeccionCuenta = new();
            _mockColeccionTransaccion = new();
            _cuentaCursor = new();
            _transaccionCursor = new();

            _mockColeccionCuenta.Object.InsertMany(GetCuentasTest());

            _cuentaCursor.SetupSequence(user => user.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);

            _cuentaCursor.SetupSequence(user => user.MoveNextAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true))
                .Returns(Task.FromResult(false));

            _transaccionCursor.SetupSequence(user => user.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);

            _transaccionCursor.SetupSequence(user => user.MoveNextAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true))
                .Returns(Task.FromResult(false));
        }

        [Fact]
        public async Task GuardarCuentaTest_Exitoso()
        {
            _mockColeccionCuenta.Setup(collection => collection
            .InsertOneAsync(
                It.IsAny<CuentaEntity>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()))
             .Returns(Task.FromResult(GetCuentasTest()[0]));

            _mockContext.Setup(c => c.Cuentas).Returns(_mockColeccionCuenta.Object);

            var repo = new CuentaRepository(_mockContext.Object);

            var result = await repo.GuardarCuenta("1",GetCuentaConId());

            Assert.Equal(GetCuentaConId().EstadoDeCuenta, result.EstadoDeCuenta);
            Assert.Equal(GetCuentaConId().Id, result.Id);

        }

        [Fact]
        public async Task ExisteCuentaConId_True()
        {
            _mockColeccionCuenta.Setup(collection => collection
            .CountDocumentsAsync(
                It.IsAny<FilterDefinition<CuentaEntity>>(),
                It.IsAny<CountOptions>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _mockContext.Setup(c => c.Cuentas).Returns(_mockColeccionCuenta.Object);

            var repo = new CuentaRepository(_mockContext.Object);

            var result = await repo.ExisteCuentaConId("1");

            Assert.True(result);

        }
        [Fact]
        public async Task ExisteCuentaConId_False()
        {
            _mockColeccionCuenta.Setup(collection => collection
            .CountDocumentsAsync(
                It.IsAny<FilterDefinition<CuentaEntity>>(),
                It.IsAny<CountOptions>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            _mockContext.Setup(c => c.Cuentas).Returns(_mockColeccionCuenta.Object);

            var repo = new CuentaRepository(_mockContext.Object);

            var result = await repo.ExisteCuentaConId("1");

            Assert.False(result);

        }
        [Fact]
        public async Task EncontrarCuentasPorDocumentoDelUsuario()
        {
            _cuentaCursor.Setup(c => c.Current).Returns(GetCuentasTest().Where(c => c.IdCliente== "1"));

            _mockColeccionCuenta.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<CuentaEntity>>(),
                It.IsAny<FindOptions<CuentaEntity>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(_cuentaCursor.Object);

            _mockColeccionTransaccion.Setup(t => t.FindAsync(
                It.IsAny<FilterDefinition<TransaccionEntity>>(),
                It.IsAny<FindOptions<TransaccionEntity>>(),
                It.IsAny<CancellationToken>())
                ).ReturnsAsync(_transaccionCursor.Object);

            _mockContext.Setup(c => c.Cuentas).Returns(_mockColeccionCuenta.Object);
            _mockContext.Setup(t => t.Transacciones).Returns(_mockColeccionTransaccion.Object);

            var repo = new CuentaRepository(_mockContext.Object);

            var result = await repo.EncontrarCuentasPorDocumentoDelUsuario(It.IsAny<string>());

            Assert.Collection(result,
                c1 => Assert.Equal(TiposDeCuenta.AHORROS, c1.TipoDeCuenta),
                c2 => Assert.Equal(9999, c2.Saldo));
        }
        [Fact]
        public async Task ObtenerCuentaPorId()
        {
            _cuentaCursor.Setup(c => c.Current).Returns(GetCuentasTest().Where(c => c.Id == "3"));

            _mockColeccionCuenta.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<CuentaEntity>>(),
                It.IsAny<FindOptions<CuentaEntity>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(_cuentaCursor.Object);

            _mockColeccionTransaccion.Setup(t => t.FindAsync(
                It.IsAny<FilterDefinition<TransaccionEntity>>(),
                It.IsAny<FindOptions<TransaccionEntity>>(),
                It.IsAny<CancellationToken>())
                ).ReturnsAsync(_transaccionCursor.Object);
            _mockContext.Setup(t => t.Transacciones).Returns(_mockColeccionTransaccion.Object);
            _mockContext.Setup(c => c.Cuentas).Returns(_mockColeccionCuenta.Object);

            var repo = new CuentaRepository(_mockContext.Object);

            var result = await repo.ObtenerCuentaPorId("3");

            Assert.Equal(9999, result.Saldo);
        }
        [Fact]
        public async Task ActualizarCuenta()
        {
            _cuentaCursor.Setup(c => c.Current).Returns(GetCuentasTest().Where(c => c.Id == "3"));
            _mockColeccionCuenta.Setup(c => c.FindOneAndReplaceAsync(
                It.IsAny<FilterDefinition<CuentaEntity>>(),
                It.IsAny<CuentaEntity>(),
                It.IsAny<FindOneAndReplaceOptions<CuentaEntity>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(GetCuentaEntityActualizada());

            _mockColeccionCuenta.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<CuentaEntity>>(),
                It.IsAny<FindOptions<CuentaEntity>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(_cuentaCursor.Object);

            _mockContext.Setup(c => c.Cuentas).Returns(_mockColeccionCuenta.Object);

            var repo = new CuentaRepository(_mockContext.Object);

            var result = await repo.ActualizarCuenta(It.IsAny<string>(), GetCuentaActualizada());

            Assert.Equal(8888, result.Saldo);

        }

        private List<CuentaEntity> GetCuentasTest()
        {
            return new()
            {
                new CuentaEntityBuilder()
                .ConId("sdfasa")
                .ConIdCliente("1")
                .ConSaldo(0)
                .ConSaldoDisponible(0)
                .ConSobregiro(0)
                .ConEstadoDeCuenta(EstadosDeCuenta.ACTIVA)
                .ConTipoDeCuenta(TiposDeCuenta.AHORROS)
                .EsNoGravable(false)
                .Build(),

                new CuentaEntityBuilder()
                .ConId("2")
                .ConSaldo(0)
                .ConIdCliente("2")
                .ConSaldoDisponible(0)
                .ConSobregiro(0)
                .ConTipoDeCuenta(TiposDeCuenta.CORRIENTE)
                .ConEstadoDeCuenta(EstadosDeCuenta.ACTIVA)
                .EsNoGravable(false)
                .Build(),

                new CuentaEntityBuilder()
                .ConId("3")
                .ConSaldo(9999)
                .ConIdCliente("1")
                .ConSaldoDisponible(9888)
                .ConSobregiro(0)
                .ConTipoDeCuenta(TiposDeCuenta.AHORROS)
                .ConEstadoDeCuenta(EstadosDeCuenta.ACTIVA)
                .EsNoGravable(false)
                .Build()
            };
        }

        private Cuenta GetCuenta() {
            return new CuentaTestBuilder()
                .ConSaldo(0)
                .ConEstadoDeCuenta(EstadosDeCuenta.ACTIVA)
                .ConTipoDeCuenta(TiposDeCuenta.AHORROS)
                .Build();
        }

        private Cuenta GetCuentaConId()
        {
            return new CuentaTestBuilder()
                .ConId("sdfasa")
                .ConSaldo(0)
                .ConEstadoDeCuenta(EstadosDeCuenta.ACTIVA)
                .ConTipoDeCuenta(TiposDeCuenta.AHORROS)
                .Build();
        }
        private CuentaEntity GetCuentaEntityActualizada()
        {
            return new CuentaEntityBuilder()
                .ConId("3")
                .ConSaldo(8888)
                .ConIdCliente("1")
                .ConSaldoDisponible(8777)
                .ConSobregiro(0)
                .ConTipoDeCuenta(TiposDeCuenta.AHORROS)
                .ConEstadoDeCuenta(EstadosDeCuenta.ACTIVA)
                .EsNoGravable(false)
                .Build();
        }
        private Cuenta GetCuentaActualizada()
        {
            return new CuentaTestBuilder()
                .ConId("3")
                .ConSaldo(8888)
                .ConSaldoDisponible(8777)
                .ConSobregiro(0)
                .ConTipoDeCuenta(TiposDeCuenta.AHORROS)
                .ConEstadoDeCuenta(EstadosDeCuenta.ACTIVA)
                .EsNoGravable(false)
                .Build();
        }
    }
}
