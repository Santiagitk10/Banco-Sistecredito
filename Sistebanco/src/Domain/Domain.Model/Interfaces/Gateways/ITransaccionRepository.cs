using Domain.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.Interfaces.Gateways
{
    /// <summary>
    /// Interface transacción
    /// </summary>
    public interface ITransaccionRepository
    {
        /// <summary>
        /// Obtener una cuenta por id
        /// </summary>
        /// <param name="idCuenta"></param>
        /// <returns></returns>
        Task<IList<Transaccion>> ObtenerPorIdDeCuenta(string idCuenta);

        /// <summary>
        /// Guardar una transacción
        /// </summary>
        /// <param name="transaccion"></param>
        /// <returns></returns>
        Task<Transaccion> GuardarTransaccion(string idCuenta, Transaccion transaccion);
    }
}