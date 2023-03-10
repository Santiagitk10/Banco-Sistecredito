using Domain.Model.Enums;
using Domain.Model.Interfaces.UseCases.Clientes;
using DrivenAdapter.gRPC.Clientes;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcServicioCliente;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrivenAdapter.gRPC.Tests
{
    public class GrpcClienteTest
    {
        private readonly Mock<ServerCallContext> _serverCallContext;
        private readonly Mock<ICrearClienteUseCase> _crearUseCase;
        private readonly Mock<IModificarClienteUseCase> _modificarUseCase;
        private readonly Mock<IObtenerPorDocumentoDeIdentidadUseCase> _obtenerPorDocumentoUseCase;
        private readonly Mock<IObtenerTodosLosClientesUseCase> _obtenerTodosLosClientesUseCase;

        public GrpcClienteTest()
        {
            _serverCallContext = new();
            _crearUseCase = new();
            _modificarUseCase = new();
            _obtenerPorDocumentoUseCase = new();
            _obtenerTodosLosClientesUseCase = new();
        }

        [Fact]
        public async Task CrearCliente_Exitoso()
        {
            _crearUseCase.Setup(c => c.CrearCliente(It.IsAny<Domain.Model.Entities.Cliente>()))
                .ReturnsAsync(GetCliente());

            var grpc = new DrivenAdapter.gRPC.Clientes.ServicioCliente(
                _crearUseCase.Object, 
                _modificarUseCase.Object, 
                _obtenerPorDocumentoUseCase.Object,
                _obtenerTodosLosClientesUseCase.Object);

            GrpcServicioCliente.Cuentas cuentas = new();

            GrpcServicioCliente.Transacciones transacciones = new();
            GrpcServicioCliente.Transaccion transaccion = new()
            {
                FechaDelMovimiento = new Google.Protobuf.WellKnownTypes.Timestamp() { Nanos = 1000, Seconds = 100 }
            };
            transacciones.Transacciones_.Add(transaccion);

            GrpcServicioCliente.Cuenta cuenta = new()
            {
                Transacciones = transacciones
            };

            cuentas.Cuentas_.Add(cuenta);

            

            ClienteCrear clienteCrear = new ClienteCrear()
            {
                Cuentas = cuentas,
                Nombres = "Carlos",
                FechaDeNacimiento = new() { Seconds= 1000, Nanos = 1000 },
            };

            var result = await grpc.CrearCliente(clienteCrear, _serverCallContext.Object);

            Assert.Equal(2023, result.Cuentas.Cuentas_.First().Transacciones.Transacciones_.First().FechaDelMovimiento.ToDateTime().Year);
        }
        [Fact]
        public async Task ModificarCliente_Exitoso()
        {
            _modificarUseCase.Setup(c => c.ModificarCliente(It.IsAny<string>(),It.IsAny<Domain.Model.Entities.Cliente>()))
               .ReturnsAsync(GetCliente());

            var grpc = new DrivenAdapter.gRPC.Clientes.ServicioCliente(
                _crearUseCase.Object,
                _modificarUseCase.Object,
                _obtenerPorDocumentoUseCase.Object,
                _obtenerTodosLosClientesUseCase.Object);

            Cliente cliente = new()
            {
                Id = "fdsad",
                DocumentoIdentidad = "ewqew",
                TipoDocumento = DocumentosDeIdentidad.CEDULA.ToString(),
                Appellidos = "Romero",
                Nombres = "Carlos",
                FechaDeCreacion = new() { Seconds = 1000, Nanos = 1000 },
                FechaDeModificacion = Timestamp.FromDateTime(DateTime.UtcNow),
                FechaDeNacimiento= new() { Seconds = 1000},
                CorreoElectronico = "dsnadunsa@nsudad.c",
                Cuentas = new() { }

            };

            var result = await grpc.ModificarCliente(cliente, _serverCallContext.Object);

            Assert.Equal(2023, result.FechaDeModificacion.ToDateTime().Year);
         
        }
        [Fact]
        public async Task ObtenerPorDocumento_Exitoso()
        {
            _obtenerPorDocumentoUseCase.Setup(c => c.ObtenerPorDocumentoDeIdentidad(It.IsAny<string>()))
               .ReturnsAsync(GetCliente());

            var grpc = new DrivenAdapter.gRPC.Clientes.ServicioCliente(
                _crearUseCase.Object,
                _modificarUseCase.Object,
                _obtenerPorDocumentoUseCase.Object,
                _obtenerTodosLosClientesUseCase.Object);

            DocumentoIdentidad doc = new()
            {
                DocumentoIdentidad_ = "dsadsad"
            };

            var result = await grpc.ObtenerClientePorDocumentoIdentidad(doc, _serverCallContext.Object);

            Assert.Equal("Carlos", result.Nombres);
        }
        [Fact]
        public async Task ObtenerTodos_Exitoso()
        {
            _obtenerTodosLosClientesUseCase.Setup(c => c.ObtenerTodos())
               .ReturnsAsync(GetClienteLista());

            var grpc = new DrivenAdapter.gRPC.Clientes.ServicioCliente(
                _crearUseCase.Object,
                _modificarUseCase.Object,
                _obtenerPorDocumentoUseCase.Object,
                _obtenerTodosLosClientesUseCase.Object);

            GrpcServicioCliente.Empty empty = new();

            var result = await grpc.ObtenerTodosLosClientes(empty, _serverCallContext.Object);

            Assert.Equal("Carlos", result.Clientes_.First().Nombres);

        }

        private Domain.Model.Entities.Cliente GetCliente()
        {
            return new()
            {
                Id = "dsa",
                Nombres = "Carlos",
                CorreoElectronico = "edsfda",
                FechaUltimaModificacion = DateTime.UtcNow,
                Cuentas = new List<Domain.Model.Entities.Cuenta>()
                {
                    new()
                    {
                        Id= "id",
                        Transacciones = new List<Domain.Model.Entities.Transaccion>()
                        {
                            new()
                            {
                                Id ="fds",
                                Mensaje = "Hola",
                                FechaDelMovimiento = new DateTime(2023,01,12,15,20,20, DateTimeKind.Utc),
                                IdCuentaDeDestino ="dsa"
                            }
                        }
                    }
                },
                DocumentoDeIdentidad = "dsadas",
                FechaDeCreacion= new DateTime(2023, 01, 12, 15, 20, 20, DateTimeKind.Utc),
                FechaDeNacimiento = new DateTime(2023, 01, 12, 15, 20, 20, DateTimeKind.Utc),
                Apellidos = "Romero",
                TipoDeDocumento =DocumentosDeIdentidad.CEDULA
            };
        }

        private List<Domain.Model.Entities.Cliente> GetClienteLista()
        {
            return new()
                {
                    new()
                        {
                        Id = "",
                        Nombres = "Carlos",
                        CorreoElectronico = "edsfda",
                        FechaUltimaModificacion = DateTime.UtcNow,
                        Cuentas = new List<Domain.Model.Entities.Cuenta>()
                        {
                            new()
                            {
                                Id= "id",
                                Transacciones = new List<Domain.Model.Entities.Transaccion>()
                                {
                                    new()
                                    {
                                        Id ="fds",
                                        Mensaje = "Hola",
                                        FechaDelMovimiento = new DateTime(2023,01,12,15,20,20, DateTimeKind.Utc),
                                        IdCuentaDeDestino ="dsa"
                                    }
                                }
                            }
                        },
                        DocumentoDeIdentidad = "dsadas",
                        FechaDeCreacion= new DateTime(2023, 01, 12, 15, 20, 20, DateTimeKind.Utc),
                        FechaDeNacimiento = new DateTime(2023, 01, 12, 15, 20, 20, DateTimeKind.Utc),
                        Apellidos = "Romero",
                        TipoDeDocumento =DocumentosDeIdentidad.CEDULA
                    }
            };
        }

    }
}
