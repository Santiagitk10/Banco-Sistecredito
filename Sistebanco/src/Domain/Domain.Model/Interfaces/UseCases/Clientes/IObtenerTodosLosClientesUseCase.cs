using System.Collections.Generic;
using System.Threading.Tasks;

using Domain.Model.Entities;

namespace Domain.Model.Interfaces.UseCases.Clientes;

public interface IObtenerTodosLosClientesUseCase
{
    public Task<IList<Cliente>> ObtenerTodos();
}