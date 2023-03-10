using Domain.Model.Entities;
using Domain.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.Request
{
    public class ClienteRequest
    {
        /// <summary>
        /// Id del cliente
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Nombres
        /// </summary>
        public string Nombres { get; set; }

        /// <summary>
        /// Apellidos
        /// </summary>
        public string Apellidos { get; set; }

        /// <summary>
        /// Tipo de documento
        /// </summary>
        public DocumentosDeIdentidad TipoDeDocumento { get; set; }

        /// <summary>
        /// Documento de identidad
        /// </summary>
        public string DocumentoDeIdentidad { get; set; }

        /// <summary>
        /// Correo electronico
        /// </summary>
        public string CorreoElectronico { get; set; }

        /// <summary>
        /// Fecha de nacimiento
        /// </summary>
        public DateTime? FechaDeNacimiento { get; set; }

        /// <summary>
        /// Fecha de creacion del usuario en base de datos
        /// </summary>
        public DateTime FechaDeCreacion { get; set; }

        /// <summary>
        /// Fecha de ultima modificacion del usuario en la base de datos
        /// </summary>
        public DateTime? FechaUltimaModificacion { get; set; }

    }
}
