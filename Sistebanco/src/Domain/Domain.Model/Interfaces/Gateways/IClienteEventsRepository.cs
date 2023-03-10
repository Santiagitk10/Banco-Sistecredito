using credinet.comun.models.Credits;
using Domain.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.Interfaces.Gateways
{
    /// <summary>
    /// Contrato de notificaciones de eventos de los Clientes
    /// </summary>
    public interface IClienteEventsRepository
    {
        /// <summary>
        /// Informar Detalles Crédito Email
        /// </summary>
        /// <param name="credito"></param>
        /// <returns></returns>
        Task InformarClienteCreadoEmail(Cliente cliente);
    }
}