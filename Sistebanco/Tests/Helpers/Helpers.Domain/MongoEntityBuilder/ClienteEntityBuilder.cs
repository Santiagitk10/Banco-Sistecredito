using Domain.Model.Enums;
using DrivenAdapters.Mongo.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Domain.MongoEntityBuilder
{
    public class ClienteEntityBuilder
    {
        private readonly ClienteEntity _cliente = new();

        public ClienteEntityBuilder ConId(string id)
        {
            _cliente.Id = id;
            return this;
        }
        public ClienteEntityBuilder ConNombres(string nombres)
        {
            _cliente.Nombres = nombres;
            return this;
        }
        public ClienteEntityBuilder ConApellidos(string apellidos)
        {
            _cliente.Apellidos = apellidos;
            return this;
        }
        public ClienteEntityBuilder ConTipoDeDocumento(DocumentosDeIdentidad tipoDeDocumento)
        {
            _cliente.TipoDeDocumento = tipoDeDocumento;
            return this;
        }
        public ClienteEntityBuilder ConDocumento(string documento)
        {
            _cliente.DocumentoDeIdentidad = documento;
            return this;
        }
        public ClienteEntityBuilder ConCorreoElectronico(string correo)
        {
            _cliente.CorreoElectronico = correo;
            return this;
        }
        public ClienteEntityBuilder ConFechaDeNacimiento(DateTime fechaDeNacimiento)
        {
            _cliente.FechaDeNacimiento = fechaDeNacimiento;
            return this;
        }
        public ClienteEntityBuilder ConFechaDeCreacion(DateTime fechaDeCreacion)
        {
            _cliente.FechaDeCreacion = fechaDeCreacion;
            return this;
        }
        public ClienteEntityBuilder ConFechaDeUltimaModificacion(DateTime fechaDeModificiacion)
        {
            _cliente.FechaUltimaModificacion = fechaDeModificiacion;
            return this;
        }
        public ClienteEntity Build()
        {
            return _cliente;
        }
    }
}
