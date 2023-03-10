using Domain.Model.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.Interfaces.UseCases.Clientes
{
    /// <summary>
    /// Interface crear cliente
    /// </summary>
    public interface ICrearClienteUseCase
    {
        /// <summary>
        /// Creacion del cliente
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        Task<Cliente> CrearCliente(Cliente cliente);
    }
}