using Domain.Model.Interfaces.UseCases.Clientes;
using Grpc.Core;
using GrpcServicioCliente;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GrpcServicioCliente.ServicioCliente;

namespace DrivenAdapter.gRPC.Clientes
{
    public class ServicioCliente : ServicioClienteBase
    {
        private readonly ICrearClienteUseCase _crearClienteUseCase;
        private readonly IModificarClienteUseCase _modificarClienteUseCase;
        private readonly IObtenerPorDocumentoDeIdentidadUseCase _obtenerPorDocumentoUseCase;
        private readonly IObtenerTodosLosClientesUseCase _obtenerTodosLosClientesUseCase;

        public ServicioCliente(
            ICrearClienteUseCase crearClienteUseCase,
            IModificarClienteUseCase modificarClienteUseCase,
            IObtenerPorDocumentoDeIdentidadUseCase obtenerPorDocumentoUseCase,
            IObtenerTodosLosClientesUseCase obtenerTodosLosClientesUseCase)
        {
            _crearClienteUseCase = crearClienteUseCase;
            _modificarClienteUseCase = modificarClienteUseCase;
            _obtenerPorDocumentoUseCase = obtenerPorDocumentoUseCase;
            _obtenerTodosLosClientesUseCase = obtenerTodosLosClientesUseCase;
        }

        public override async Task<Cliente> CrearCliente(ClienteCrear request, ServerCallContext context)
            => await Handler.HandleRequestAsync(
                async () =>
                {
                    Domain.Model.Entities.Cliente cliente = await _crearClienteUseCase.CrearCliente(Converter.FromGrpcCrearClienteToDomainCliente(request));
                    return Converter.FromDomainClienteToGrpcCliente(cliente);
                }
                );

        public override async Task<Cliente> ModificarCliente(Cliente request, ServerCallContext context)
            => await Handler.HandleRequestAsync(
                async () =>
                {
                    Domain.Model.Entities.Cliente cliente = await _modificarClienteUseCase.ModificarCliente(request.DocumentoIdentidad, Converter.FromGrpcClienteToDomainCliente(request));
                    return Converter.FromDomainClienteToGrpcCliente(cliente);
                }
                );

        public override async Task<Cliente> ObtenerClientePorDocumentoIdentidad(DocumentoIdentidad request, ServerCallContext context)
            => await Handler.HandleRequestAsync(
                async () =>
                {
                    Domain.Model.Entities.Cliente cliente = await _obtenerPorDocumentoUseCase.ObtenerPorDocumentoDeIdentidad(request.DocumentoIdentidad_);
                    return Converter.FromDomainClienteToGrpcCliente(cliente);
                }
                );

        public override async Task<GrpcServicioCliente.Clientes> ObtenerTodosLosClientes(Empty request, ServerCallContext context)
            => await Handler.HandleRequestAsync(
                async () =>
                {
                    IList<Domain.Model.Entities.Cliente> clientes = await _obtenerTodosLosClientesUseCase.ObtenerTodos();
                    GrpcServicioCliente.Clientes clientesGrpc = new();
                    clientesGrpc.Clientes_.AddRange(clientes.Select(c => Converter.FromDomainClienteToGrpcCliente(c)));
                    return clientesGrpc;
                }
                );
    }
}