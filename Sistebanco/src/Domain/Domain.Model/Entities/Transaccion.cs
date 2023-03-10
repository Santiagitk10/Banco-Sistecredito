using Domain.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.Entities
{
    /// <summary>
    /// Transacción
    /// </summary>
    public class Transaccion
    {
        /// <summary>
        /// Id de la transacción
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Monto de la transacción
        /// </summary>
        public decimal Monto { get; set; }

        /// <summary>
        /// Tipo d e transacción
        /// </summary>
        public TipoDeTransacciones TipoDeTransaccion { get; set; }

        /// <summary>
        /// Cuenta de destino (Operación entre cuentas)
        /// </summary>
        public string? IdCuentaDeDestino { get; set; }

        /// <summary>
        /// Mensaje de transacción
        /// </summary>
        public string? Mensaje { get; set; }

        /// <summary>
        /// Fecha del movimiento
        /// </summary>
        public DateTime FechaDelMovimiento { get; set; }

        /// <summary>
        /// Valor del gravamen al monto de transacción
        /// </summary>
        public decimal GravamenDelMovimiento { get; set; }
    }
}