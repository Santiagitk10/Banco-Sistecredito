using Domain.Model.Entities;
using Domain.Model.Enums;
using DrivenAdapters.Mongo.Entities.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrivenAdapters.Mongo.Entities
{
    /// <summary>
    /// Entidad Cuenta de mongo
    /// </summary>
    public class CuentaEntity: IDomainEntity<Cuenta>
    {
        /// <summary>
        /// 
        /// </summary>
        [BsonId]
        public string Id { get; set; }
        /// <summary>
        /// Saldo total en la cuenta
        /// </summary>
        [BsonElement(elementName: "saldo")]
        public decimal Saldo { get; set; }

        /// <summary>
        /// Saldo disponible despues de gravamen (Lo que puede retirar)
        /// </summary>
        [BsonElement(elementName: "saldo_disponible")]
        public decimal SaldoDisponible { get; set; }

        /// <summary>
        /// Es gravable por 4x1000
        [BsonElement(elementName: "es_no_gravable")]
        public bool EsNoGravable { get; set; }

        /// <summary>
        /// Estado de la cuenta
        /// </summary>
        [BsonElement(elementName: "estado_de_cuenta")]
        public EstadosDeCuenta EstadoDeCuenta { get; set; }

        /// <summary>
        /// Tipo de cuenta
        /// </summary>
        [BsonElement(elementName: "tipo_de_cuenta")]
        public TiposDeCuenta TipoDeCuenta { get; set; }

        /// <summary>
        /// Sobregiro
        /// </summary>
        [BsonElement(elementName: "sobregiro")]
        public decimal Sobregiro { get; set; }

        /// <summary>
        /// Id Cliente al que pertenece la cuenta
        /// </summary>
        [BsonElement(elementName: "id_cliente")]
        public string IdCliente { get; set; }

        /// <summary>
        /// Convertir en entidad de dominio
        /// </summary>
        /// <returns></returns>
        public Cuenta AsEntity()
        {
            return new()
            {
                Id = Id,
                Saldo = Saldo,
                SaldoDisponible = SaldoDisponible,
                EsNoGravable = EsNoGravable,
                EstadoDeCuenta = EstadoDeCuenta,
                TipoDeCuenta = TipoDeCuenta,
                Sobregiro = Sobregiro,
            };
        }
    }
}
