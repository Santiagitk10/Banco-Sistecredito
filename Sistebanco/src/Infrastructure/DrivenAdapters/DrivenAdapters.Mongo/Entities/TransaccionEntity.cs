using Domain.Model.Entities;
using Domain.Model.Enums;
using DrivenAdapters.Mongo.Entities.Base;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrivenAdapters.Mongo.Entities
{
    public class TransaccionEntity : EntityBase, IDomainEntity<Transaccion>
    {
        /// <summary>
        /// Id Cuenta
        /// </summary>
        [BsonElement(elementName: "IdCuenta")]
        public string IdCuenta { get; set; }

        /// <summary>
        /// Monto de la transacción
        /// </summary>
        [BsonElement(elementName: "monto")]
        public decimal Monto { get; set; }

        /// <summary>
        /// Tipo d e transacción
        /// </summary>
        [BsonElement(elementName: "tipoDeTransaccion")]
        public TipoDeTransacciones TipoDeTransaccion { get; set; }

        /// <summary>
        /// Cuenta de destino (Operación entre cuentas)
        /// </summary>
        [BsonElement(elementName: "idCuentaDeDestino")]
        public string? IdCuentaDeDestino { get; set; }

        /// <summary>
        /// Mensaje de transacción
        /// </summary>
        [BsonElement(elementName: "mensaje")]
        public string? Mensaje { get; set; }

        /// <summary>
        /// Fecha del movimiento
        /// </summary>
        [BsonElement(elementName: "fechaDeMovimiento")]
        public DateTime FechaDelMovimiento { get; set; }

        /// <summary>
        /// Valor del gravamen al monto de transacción
        /// </summary>
        [BsonElement(elementName: "gravamenDelMovimiento")]
        public decimal GravamenDelMovimiento { get; set; }

        /// <summary>
        /// Convertir a entidad de dominio
        /// </summary>
        /// <returns></returns>
        public Transaccion AsEntity() =>
            new()
            {
                Id = Id,
                Monto = Monto,
                TipoDeTransaccion = TipoDeTransaccion,
                IdCuentaDeDestino = IdCuentaDeDestino,
                Mensaje = Mensaje,
                FechaDelMovimiento = FechaDelMovimiento,
                GravamenDelMovimiento = GravamenDelMovimiento
            };
    }
}