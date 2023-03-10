using Domain.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.Interfaces.Gateways
{
    /// <summary>
    /// Interface cliente
    /// </summary>
    public interface IClienteRepository
    {
        /// <summary>
        /// Obtener un cliente por documento de identidad
        /// </summary>
        /// <param name="documento"></param>
        /// <returns></returns>
        Task<Cliente> ObtenerClientePorDocumento(string documento);

        /// <summary>
        /// Obtener todos los clientes
        /// </summary>
        /// <returns></returns>
        Task<IList<Cliente>> ObtenerTodosLosClientes();

        /// <summary>
        /// Guardar un cliente
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        Task<Cliente> GuardarCliente(Cliente cliente);

        /// <summary>
        /// Actualizar datos del cliente
        /// </summary>
        /// <param name="documento"></param>
        /// <param name="cambios"></param>
        /// <returns></returns>
        Task<Cliente> ActualizarDatos(string documento, Cliente cambios);

        /// <summary>
        /// Verificar si el cliente existe
        /// </summary>
        /// <param name="documento"></param>
        /// <returns></returns>
        Task<bool> ClienteExisteConDocumentoDeIdentidad(string documento);

        /// <summary>
        /// Verificar si el cliente es el dueño de la cuenta
        /// </summary>
        /// <param name="documentoDeIdentidad"></param>
        /// <param name="idCuenta"></param>
        /// <returns></returns>
        Task<bool> ClienteConDocumentoDeIdentidadTieneCuentaConId(string documentoDeIdentidad, string idCuenta);
    }
}