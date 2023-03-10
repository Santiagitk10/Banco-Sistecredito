using Domain.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.Interfaces.UseCases.Transacciones
{
    /// <summary>
    /// Consignar dinero
    /// </summary>
    public interface IConsignarUseCase
    {
        /// <summary>
        /// Consignar
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idCuenta"></param>
        /// <param name="monto"></param>
        /// <param name="mensaje"></param>
        /// <returns></returns>
        Task<Transaccion> Consignar(string idCliente, string idCuenta, decimal monto, string? mensaje = "");
    }
}