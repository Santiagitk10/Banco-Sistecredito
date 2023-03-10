using Domain.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.Interfaces.UseCases.Cuentas
{
    /// <summary>
    /// Interface para creación de cuenta
    /// </summary>
    public interface ICrearCuentaUseCase
    {
        /// <summary>
        /// Crear cuenta
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="cuenta"></param>
        /// <returns></returns>
        Task<Cuenta> CrearCuenta(string idCliente,Cuenta cuenta);
    }
}
