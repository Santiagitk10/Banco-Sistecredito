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
    /// <summary>
    /// Entidad cliente de mongo
    /// </summary>
    public class ClienteEntity : EntityBase, IDomainEntity<Cliente>
    {
        /// <summary>
        /// Nombres
        /// </summary>
        [BsonElement(elementName: "nombres")]
        public string Nombres { get; set; }

        /// <summary>
        /// Apellidos
        /// </summary>
        [BsonElement(elementName: "apellidos")]
        public string Apellidos { get; set; }

        /// <summary>
        /// Tipo de documento
        /// </summary>
        [BsonElement(elementName: "tipo_documento")]
        public DocumentosDeIdentidad TipoDeDocumento { get; set; }

        /// <summary>
        /// Documento de identidad
        /// </summary>
        [BsonElement(elementName: "documento_de_identidad")]
        public string DocumentoDeIdentidad { get; set; }

        /// <summary>
        /// Correo electronico
        /// </summary>
        [BsonElement(elementName: "correo")]
        public string CorreoElectronico { get; set; }

        /// <summary>
        /// Fecha de nacimiento
        /// </summary>
        [BsonElement(elementName: "fecha_de_nacimiento")]
        public DateTime? FechaDeNacimiento { get; set; }

        /// <summary>
        /// Fecha de creacion del usuario en base de datos
        /// </summary>
        [BsonElement(elementName: "fecha_de_creacion")]
        public DateTime FechaDeCreacion { get; set; }

        /// <summary>
        /// Fecha de ultima modificacion del usuario en la base de datos

        [BsonElement(elementName: "fecha_de_ultima_modificacion")] public DateTime? FechaUltimaModificacion { get; set; }

        public Cliente AsEntity()
        {
            return new()
            {
                Id = Id,
                Nombres = Nombres,
                Apellidos = Apellidos,
                TipoDeDocumento = TipoDeDocumento,
                DocumentoDeIdentidad = DocumentoDeIdentidad,
                CorreoElectronico = CorreoElectronico,
                FechaDeCreacion = FechaDeCreacion,
                FechaDeNacimiento = FechaDeNacimiento,
                FechaUltimaModificacion = FechaUltimaModificacion
            };
        }

        /// <summary>
        /// Agregar Cuenta a la colección
        /// </summary>
    }
}