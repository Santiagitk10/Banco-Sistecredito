using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.Enums
{
    /// <summary>
    /// Tipos de documentos de identidad
    /// </summary>
    public enum DocumentosDeIdentidad
    {
        /// <summary>
        /// Cédula
        /// </summary>
        CEDULA = 0,
        /// <summary>
        /// Cédula de extranjería
        /// </summary>
        CEDULA_DE_EXTRANJERIA = 1,
        /// <summary>
        /// Pasaporte
        /// </summary>
        PASAPORTE = 2
    }
}
