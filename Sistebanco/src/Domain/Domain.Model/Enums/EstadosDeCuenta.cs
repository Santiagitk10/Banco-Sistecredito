using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.Enums
{
    /// <summary>
    /// Posibles estados de la cuenta
    /// </summary>
    public enum EstadosDeCuenta
    {
        /// <summary>
        /// Activa
        /// </summary>
        ACTIVA = 0,
        /// <summary>
        /// Cancelada
        /// </summary>
        CANCELADA = 1,
        /// <summary>
        /// Inactiva
        /// </summary>
        INACTIVA = 2
    }
}
