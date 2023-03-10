using Domain.Model.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.Interfaces.UseCases.Clientes
{
    /// <summary>
    /// Interface obtener por documento de identidad
    /// </summary>
    public interface IObtenerPorDocumentoDeIdentidadUseCase
    {
        /// <summary>
        /// Obtener cliente por documento de identidad
        /// </summary>
        /// <param name="documentoDeIdentidad"></param>
        /// <returns></returns>
        Task<Cliente> ObtenerPorDocumentoDeIdentidad(string documentoDeIdentidad);
    }
}