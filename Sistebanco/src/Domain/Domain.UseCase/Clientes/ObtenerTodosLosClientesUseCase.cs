using System.Collections.Generic;
using System.Threading.Tasks;

using Domain.Model.Entities;
using Domain.Model.Interfaces.Gateways;
using Domain.Model.Interfaces.UseCases.Clientes;

namespace Domain.UseCase.Clientes;

public class ObtenerTodosLosClientesUseCase : IObtenerTodosLosClientesUseCase
{
    private readonly IClienteRepository _repositorioDeClientes;

    public ObtenerTodosLosClientesUseCase(IClienteRepository repositorioDeClientes)
    {
        _repositorioDeClientes = repositorioDeClientes;
    }

    public Task<IList<Cliente>> ObtenerTodos()
    {
        return _repositorioDeClientes.ObtenerTodosLosClientes();
    }
}