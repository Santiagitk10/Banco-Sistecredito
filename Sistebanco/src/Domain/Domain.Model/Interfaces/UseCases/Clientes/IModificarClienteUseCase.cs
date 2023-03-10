using Domain.Model.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.Interfaces.UseCases.Clientes
{
    /// <summary>
    /// Interface modificar cliente
    /// </summary>
    public interface IModificarClienteUseCase
    {
        /// <summary>
        /// Modificar el cliente
        /// </summary>
        /// <param name="documentoDeIdentidad"></param>
        /// <param name="cambios"></param>
        /// <returns></returns>
        Task<Cliente> ModificarCliente(string documentoDeIdentidad, Cliente cambios);
    }
}