using System;
using System.Threading.Tasks;

using credinet.exception.middleware.models;

using Domain.Model.Entities;
using Domain.Model.Interfaces.Gateways;
using Domain.Model.Interfaces.UseCases.Clientes;

using Helpers.Commons.Exceptions;
using Helpers.ObjectsUtils.Extensions;

namespace Domain.UseCase.Clientes;

public class ObtenerPorDocumentoDeIdentidadUseCase : IObtenerPorDocumentoDeIdentidadUseCase
{
    private readonly IClienteRepository _repositorioDeClientes;

    public ObtenerPorDocumentoDeIdentidadUseCase(IClienteRepository repositorioDeClientes)
    {
        _repositorioDeClientes = repositorioDeClientes;
    }

    public async Task<Cliente> ObtenerPorDocumentoDeIdentidad(string documentoDeIdentidad)
    {
        Cliente cliente = await _repositorioDeClientes.ObtenerClientePorDocumento(documentoDeIdentidad);

        return cliente is null
            ? throw new BusinessException(
                TipoExcepcionNegocio.ClienteConDocumentoDeIdentidadNoExiste.GetDescription(),
                Convert.ToInt32(TipoExcepcionNegocio.ClienteConDocumentoDeIdentidadNoExiste)
            )
            : cliente;
    }
}