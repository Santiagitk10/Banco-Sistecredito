using Domain.Model.Entities;
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

namespace DrivenAdapters.Mongo.Tests.Clientes
{
    public class ClienteRepositoryTest
    {
        private readonly Mock<IContext> _mockContext;
        private readonly Mock<IMongoCollection<ClienteEntity>> _mockColeccionCliente;
        private readonly Mock<IMongoCollection<CuentaEntity>> _mockColeccionCuenta;
        private readonly Mock<IMongoCollection<TransaccionEntity>> _mockColeccionTransaccion;
        private readonly Mock<IAsyncCursor<ClienteEntity>> _clienteCursor;
        private readonly Mock<IAsyncCursor<CuentaEntity>> _cuentaCursor;

        public ClienteRepositoryTest()
        {
            _mockContext = new();
            _mockColeccionCliente = new();
            _mockColeccionCuenta = new();
            _mockColeccionTransaccion = new();
            _clienteCursor = new();
            _cuentaCursor = new();

            _mockColeccionCliente.Object.InsertMany(GetClientesTest());

            _cuentaCursor.SetupSequence(user => user.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);

            _cuentaCursor.SetupSequence(user => user.MoveNextAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true))
                .Returns(Task.FromResult(false));

            _clienteCursor.SetupSequence(user => user.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);

            _clienteCursor.SetupSequence(user => user.MoveNextAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true))
                .Returns(Task.FromResult(false));
        }
        [Fact]
        public async Task ClienteConDocumentoDeIdentidadTieneCuentaConId_True()
        {
            _mockColeccionCuenta.Setup(collection => collection
            .CountDocumentsAsync(
                It.IsAny<FilterDefinition<CuentaEntity>>(),
                It.IsAny<CountOptions>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _mockContext.Setup(c => c.Clientes).Returns(_mockColeccionCliente.Object);
            _mockContext.Setup(c => c.Cuentas).Returns(_mockColeccionCuenta.Object);

            var repo = new ClienteRepository(_mockContext.Object);

            var result = await repo.ClienteConDocumentoDeIdentidadTieneCuentaConId("2222", "1");

            Assert.True(result);
        }

        [Fact]
        public async Task ClienteConDocumentoDeIdentidadTieneCuentaConId_False()
        {
            _mockColeccionCuenta.Setup(collection => collection
            .CountDocumentsAsync(
                It.IsAny<FilterDefinition<CuentaEntity>>(),
                It.IsAny<CountOptions>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            _mockContext.Setup(c => c.Clientes).Returns(_mockColeccionCliente.Object);
            _mockContext.Setup(c => c.Cuentas).Returns(_mockColeccionCuenta.Object);

            var repo = new ClienteRepository(_mockContext.Object);

            var result = await repo.ClienteConDocumentoDeIdentidadTieneCuentaConId("2222", "1");

            Assert.False(result);
        }
        [Fact]
        public async Task ClienteExisteConDocumentoDeIdentidad_True()
        {
            _mockColeccionCliente.Setup(collection => collection
            .CountDocumentsAsync(
                It.IsAny<FilterDefinition<ClienteEntity>>(),
                It.IsAny<CountOptions>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _mockContext.Setup(c => c.Clientes).Returns(_mockColeccionCliente.Object);
            _mockContext.Setup(c => c.Cuentas).Returns(_mockColeccionCuenta.Object);

            var repo = new ClienteRepository(_mockContext.Object);

            var result = await repo.ClienteExisteConDocumentoDeIdentidad("1");

            Assert.True(result);
        }
        [Fact]
        public async Task ClienteExisteConDocumentoDeIdentidad_False()
        {
            _mockColeccionCliente.Setup(collection => collection
            .CountDocumentsAsync(
                It.IsAny<FilterDefinition<ClienteEntity>>(),
                It.IsAny<CountOptions>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            _mockContext.Setup(c => c.Clientes).Returns(_mockColeccionCliente.Object);
            _mockContext.Setup(c => c.Cuentas).Returns(_mockColeccionCuenta.Object);

            var repo = new ClienteRepository(_mockContext.Object);

            var result = await repo.ClienteExisteConDocumentoDeIdentidad("1");

            Assert.False(result);
        }
        [Fact]
        public async Task GuardarCliente()
        {
            _mockColeccionCliente.Setup(c => c.InsertOneAsync(
                It.IsAny<ClienteEntity>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()
                ));

            _mockContext.Setup(c => c.Clientes).Returns(_mockColeccionCliente.Object);
            _mockContext.Setup(c => c.Cuentas).Returns(_mockColeccionCuenta.Object);

            var repo = new ClienteRepository(_mockContext.Object);

            var result = await repo.GuardarCliente(GetCliente());

            _mockColeccionCliente.Verify(c => c.InsertOneAsync(
                It.IsAny<ClienteEntity>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal("2222", result.DocumentoDeIdentidad);
            Assert.Equal(DateTime.UtcNow.Day, result.FechaDeCreacion.Day);
        }

        [Fact]
        public async Task ActualizarDatos()
        {
            _mockColeccionCliente.Setup(c => c.FindOneAndReplaceAsync(
                            It.IsAny<FilterDefinition<ClienteEntity>>(),
                            It.IsAny<ClienteEntity>(),
                            It.IsAny<FindOneAndReplaceOptions<ClienteEntity>>(),
                            It.IsAny<CancellationToken>()
                            )).ReturnsAsync(GetClienteEntityActualizado());

            _mockContext.Setup(c => c.Clientes).Returns(_mockColeccionCliente.Object);
            _mockContext.Setup(c => c.Cuentas).Returns(_mockColeccionCuenta.Object);

            var repo = new ClienteRepository(_mockContext.Object);

            var result = await repo.ActualizarDatos("2222", GetClienteActualizado());

            Assert.Equal("Gil", result.Apellidos);
            Assert.Equal(DateTime.Now.Year, result.FechaUltimaModificacion.Value.Year);
        }
        [Fact]
        public async Task ObtenerClientePorDocumento()
        {
            _clienteCursor.Setup(c => c.Current).Returns(GetClientesTest().Where(c => c.DocumentoDeIdentidad =="2222"));

                _mockColeccionCliente.Setup(t => t.FindAsync(
                It.IsAny<FilterDefinition<ClienteEntity>>(),
                It.IsAny<FindOptions<ClienteEntity>>(),
                It.IsAny<CancellationToken>())
                ).ReturnsAsync(_clienteCursor.Object);

            _mockColeccionCuenta.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<CuentaEntity>>(),
                It.IsAny<FindOptions<CuentaEntity>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(_cuentaCursor.Object);

            _mockContext.Setup(c => c.Clientes).Returns(_mockColeccionCliente.Object);
            _mockContext.Setup(c => c.Cuentas).Returns(_mockColeccionCuenta.Object);

            var repo = new ClienteRepository(_mockContext.Object);

            var result = await repo.ObtenerClientePorDocumento("2222");

            Assert.Equal("carlos.romero@sofka.co", result.CorreoElectronico);
        }
        [Fact]
        public async Task ObtenerClientePorDocumento_Fallido()
        {
            _clienteCursor.Setup(c => c.Current).Returns(GetClientesTest().Where(c => c.DocumentoDeIdentidad == "5"));

            _mockColeccionCliente.Setup(t => t.FindAsync(
            It.IsAny<FilterDefinition<ClienteEntity>>(),
            It.IsAny<FindOptions<ClienteEntity>>(),
            It.IsAny<CancellationToken>())
            ).ReturnsAsync(_clienteCursor.Object);

            _mockColeccionCuenta.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<CuentaEntity>>(),
                It.IsAny<FindOptions<CuentaEntity>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(_cuentaCursor.Object);

            _mockContext.Setup(c => c.Clientes).Returns(_mockColeccionCliente.Object);
            _mockContext.Setup(c => c.Cuentas).Returns(_mockColeccionCuenta.Object);

            var repo = new ClienteRepository(_mockContext.Object);

            var result = await repo.ObtenerClientePorDocumento("2222");

            Assert.Null(result);
        }
        [Fact]
        public async Task ObtenerTodosLosClientes()
        {
            _clienteCursor.Setup(c => c.Current).Returns(GetClientesTest());

            _mockColeccionCliente.Setup(t => t.FindAsync(
            It.IsAny<FilterDefinition<ClienteEntity>>(),
            It.IsAny<FindOptions<ClienteEntity>>(),
            It.IsAny<CancellationToken>())
            ).ReturnsAsync(_clienteCursor.Object);

            _mockColeccionCuenta.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<CuentaEntity>>(),
                It.IsAny<FindOptions<CuentaEntity>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(_cuentaCursor.Object);

            _mockContext.Setup(c => c.Clientes).Returns(_mockColeccionCliente.Object);
            _mockContext.Setup(c => c.Cuentas).Returns(_mockColeccionCuenta.Object);

            var repo = new ClienteRepository(_mockContext.Object);

            var result = await repo.ObtenerTodosLosClientes();

            Assert.Collection(result,
                c1 => Assert.Equal("Carlos", c1.Nombres),
                c2 => Assert.Equal("santiago.sierra@sofka.co", c2.CorreoElectronico));
        }
        private List<ClienteEntity> GetClientesTest()
        {
            return new()
            {
                new ClienteEntityBuilder()
                .ConDocumento("2222")
                .ConNombres("Carlos")
                .ConApellidos("Romero")
                .ConCorreoElectronico("carlos.romero@sofka.co")
                .Build(),

                new ClienteEntityBuilder()
                .ConDocumento("5555")
                .ConNombres("Santiago")
                .ConApellidos("Sierra")
                .ConCorreoElectronico("santiago.sierra@sofka.co")
                .Build()
            };
        }

        private Cliente GetCliente()
        {
            return new ClienteTestBuilder()
                .ConDocumentoDeIdentidad("2222")
                .ConNombres("Carlos")
                .ConApellidos("Romero")
                .ConCorreoElectronico("carlos.romero@sofka.co")
                .Build();
        }

        private Cliente GetClienteActualizado()
        {
            return new ClienteTestBuilder()
                .ConDocumentoDeIdentidad("2222")
                .ConNombres("Carlos")
                .ConApellidos("Gil")
                .ConCorreoElectronico("carlos.romero@sofka.co")
                .Build();
        }
        private ClienteEntity GetClienteEntityActualizado()
        {
            return new ClienteEntityBuilder()
                .ConDocumento("2222")
                .ConNombres("Carlos")
                .ConApellidos("Gil")
                .ConFechaDeUltimaModificacion(DateTime.Now)
                .ConCorreoElectronico("carlos.romero@sofka.co")
                .Build();
        }
    }
}
