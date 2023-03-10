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
    public class CuentaEntityBuilder
    {
        private readonly CuentaEntity _cuentaEntity;

        public CuentaEntityBuilder()
        {
            _cuentaEntity = new();
        }

        public CuentaEntityBuilder ConId(string id)
        {
            _cuentaEntity.Id = id;
            return this;
        }

        public CuentaEntityBuilder ConSaldo(decimal saldo)
        {
            _cuentaEntity.Saldo = saldo;
            return this;
        }
        public CuentaEntityBuilder ConSobregiro(decimal sobregiro)
        {
            _cuentaEntity.Sobregiro = sobregiro;
            return this;
        }

        public CuentaEntityBuilder ConSaldoDisponible(decimal saldoDisponible)
        {
            _cuentaEntity.SaldoDisponible = saldoDisponible;
            return this;
        }

        public CuentaEntityBuilder EsNoGravable(bool esNoGravable)
        {
            _cuentaEntity.EsNoGravable = esNoGravable;
            return this;
        }

        public CuentaEntityBuilder ConEstadoDeCuenta(EstadosDeCuenta estadoDeCuenta)
        {
            _cuentaEntity.EstadoDeCuenta = estadoDeCuenta;
            return this;
        }

        public CuentaEntityBuilder ConTipoDeCuenta(TiposDeCuenta tipoDeCuenta)
        {
            _cuentaEntity.TipoDeCuenta = tipoDeCuenta;
            return this;
        }

        public CuentaEntityBuilder ConIdCliente(string idCliente)
        {
            _cuentaEntity.IdCliente = idCliente;
            return this;
        }

        public CuentaEntity Build()
        {
            return _cuentaEntity;
        }
    }
}
