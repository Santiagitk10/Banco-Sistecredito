using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.Interfaces.UseCases.Cuentas
{
    /// <summary>
    /// Cancelar cuenta
    /// </summary>
    public interface ICancelarCuentaUseCase
    {
        /// <summary>
        /// Cancelar cuenta
        /// </summary>
        /// <param name="idCuenta"></param>
        /// <returns></returns>
        Task CancelarCuenta(string idCuenta);
    }
}
