using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.Enums
{
    /// <summary>
    /// Enum para el tipo de transaccion
    /// </summary>
    public enum TipoDeTransacciones
    {
        /// <summary>
        /// Retiro
        /// </summary>
        RETIRO = 0,
        /// <summary>
        /// Consignacion
        /// </summary>
        CONSIGNACION = 1,
        /// <summary>
        /// Transaccion entre cuentas
        /// </summary>
        TRANSACCION_ENTRE_CUENTAS = 2
    }
}
