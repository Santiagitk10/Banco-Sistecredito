using Domain.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.Interfaces.UseCases.Transacciones
{
    /// <summary>
    /// Transferir dinero a otra cuenta
    /// </summary>
    public interface ITransferirUseCase
    {
        /// <summary>
        /// Transferir a otra cuenta
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idCuentaOrigen"></param>
        /// <param name="idCuentaDestino"></param>
        /// <param name="monto"></param>
        /// <param name="mensaje"></param>
        /// <returns></returns>
        Task<Transaccion> Transferir(string idCliente, string idCuentaOrigen, string idCuentaDestino,
            decimal monto, string? mensaje = "");
    }
}