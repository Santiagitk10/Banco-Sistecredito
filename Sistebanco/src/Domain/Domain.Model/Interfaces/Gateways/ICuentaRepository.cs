using Domain.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.Interfaces.Gateways
{
    /// <summary>
    /// Interface de repositorio de cuenta
    /// </summary>
    public interface ICuentaRepository
    {
        /// <summary>
        /// Encontrar todas las cuentas asociadas a un usuario
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        Task<IList<Cuenta>> EncontrarCuentasPorDocumentoDelUsuario(string idUsuario);
        /// <summary>
        /// Actualizar datos en la cuenta
        /// </summary>
        /// <param name="idCuenta"></param>
        /// <param name="cambios"></param>
        /// <returns></returns>
        Task<Cuenta> ActualizarCuenta(string idCuenta, Cuenta cambios);
        /// <summary>
        /// Guardar una cuenta
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="cuenta"></param>
        /// <returns></returns>
        Task<Cuenta> GuardarCuenta(string idCliente,Cuenta cuenta);
        /// <summary>
        /// Obtener una cuenta por id
        /// </summary>
        /// <param name="idCuenta"></param>
        /// <returns></returns>
        Task<Cuenta> ObtenerCuentaPorId(string idCuenta);
        /// <summary>
        /// Verificar si existe la cuenta
        /// </summary>
        /// <param name="idCuenta"></param>
        /// <returns></returns>
        Task<bool> ExisteCuentaConId(string idCuenta);
    }
}
