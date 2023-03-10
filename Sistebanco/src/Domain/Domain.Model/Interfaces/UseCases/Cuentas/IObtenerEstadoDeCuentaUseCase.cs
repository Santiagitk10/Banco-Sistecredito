using Domain.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.Interfaces.UseCases.Cuentas
{
    /// <summary>
    /// Obtener el listado de transacciones
    /// </summary>
    public interface IObtenerEstadoDeCuentaUseCase
    {
        /// <summary>
        /// Obtener estado de cuenta (Listado de transacciones) (Obtener historial de enviado recibido)
        /// </summary>
        /// <param name="idCuenta"></param>
        /// <returns></returns>
        Task<Cuenta> ObtenerEstadoDeCuenta(string idCuenta);
    }
}
