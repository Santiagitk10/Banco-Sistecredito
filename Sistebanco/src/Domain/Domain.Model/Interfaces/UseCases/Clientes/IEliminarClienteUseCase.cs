using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.Interfaces.UseCases.Clientes
{
    /// <summary>
    /// Interface de eliminación de cliente
    /// </summary>
    public interface IEliminarClienteUseCase
    {
        /// <summary>
        /// Eliminar cliente
        /// </summary>
        /// <param name="documentoDeIdentidad"></param>
        /// <returns></returns>
        Task EliminarCliente(string documentoDeIdentidad);
    }
}
