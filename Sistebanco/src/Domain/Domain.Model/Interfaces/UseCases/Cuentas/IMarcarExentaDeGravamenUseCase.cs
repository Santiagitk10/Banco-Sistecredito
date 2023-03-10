using Domain.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.Interfaces.UseCases.Cuentas
{
    /// <summary>
    /// Interface marcar la cuenta como exenta de gravamen
    /// </summary>
    public interface IMarcarExentaDeGravamenUseCase
    {
        /// <summary>
        /// Marcar cuenta como exenta de 4x1000
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idCuenta"></param>
        /// <returns></returns>
        Task<Cuenta> MarcarExentaDeGravamen(string idCliente, string idCuenta);
    }
}
