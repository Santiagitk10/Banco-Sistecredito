using System;
using System.Threading.Tasks;

using credinet.exception.middleware.models;

using Domain.Model.Entities;
using Domain.Model.Interfaces.Gateways;
using Domain.Model.Interfaces.UseCases.Clientes;

using Helpers.Commons.Exceptions;
using Helpers.ObjectsUtils.Extensions;

namespace Domain.UseCase.Clientes;

public class ModificarClienteUseCase : IModificarClienteUseCase
{
    private readonly IClienteRepository _repositorioDeClientes;

    public ModificarClienteUseCase(IClienteRepository repositorioDeClientes)
    {
        _repositorioDeClientes = repositorioDeClientes;
    }

    public async Task<Cliente> ModificarCliente(string documentoDeIdentidad, Cliente cambios)
    {
        bool clienteExiste = await _repositorioDeClientes.ClienteExisteConDocumentoDeIdentidad(documentoDeIdentidad);
        if (!clienteExiste)
            throw new BusinessException(
                TipoExcepcionNegocio.ClienteConDocumentoDeIdentidadNoExiste.GetDescription(),
                Convert.ToInt32(TipoExcepcionNegocio.ClienteConDocumentoDeIdentidadNoExiste)
            );

        return await ModificarClienteSafe(documentoDeIdentidad, cambios);
    }

    public async Task<Cliente> ModificarClienteSafe(string documentoDeIdentidad, Cliente cambios)
    {
        Cliente cliente = await _repositorioDeClientes.ObtenerClientePorDocumento(documentoDeIdentidad);
        cliente.CorreoElectronico = cambios.CorreoElectronico;

        return await _repositorioDeClientes.ActualizarDatos(documentoDeIdentidad, cambios);
    }
}