using Domain.Model.Entities;
using Domain.Model.Enums;
using DrivenAdapters.Mongo.Entities;
using Helpers.Domain.EntityBuilders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Domain.MongoEntityBuilder
{
    public class TransaccionEntityBuilder
    {
        private readonly TransaccionEntity _transaccion;

        public TransaccionEntityBuilder()
        {
            _transaccion = new TransaccionEntity();
        }

        public TransaccionEntityBuilder ConId(string id)
        {
            _transaccion.Id = id;
            return this;
        }
        public TransaccionEntityBuilder ConCuentaId(string idCuenta)
        {
            _transaccion.IdCuenta = idCuenta;
            return this;
        }

        public TransaccionEntityBuilder ConMonto(decimal monto)
        {
            _transaccion.Monto = monto;
            return this;
        }

        public TransaccionEntityBuilder ConTipoDeTransaccion(TipoDeTransacciones tipo)
        {
            _transaccion.TipoDeTransaccion = tipo;
            return this;
        }

        public TransaccionEntityBuilder ConIdCuentaDeDestino(string idCuenta)
        {
            _transaccion.IdCuentaDeDestino = idCuenta;
            return this;
        }

        public TransaccionEntityBuilder ConMensaje(string mensaje)
        {
            _transaccion.Mensaje = mensaje;
            return this;
        }

        public TransaccionEntityBuilder ConFechaDelMovimiento(DateTime fecha)
        {
            _transaccion.FechaDelMovimiento = fecha;
            return this;
        }

        public TransaccionEntityBuilder ConGravamenDelMovimiento(decimal gravamen)
        {
            _transaccion.GravamenDelMovimiento = gravamen;
            return this;
        }

        public TransaccionEntity Build()
        {
            return _transaccion;
        }
    }
}
