using Domain.Model.Entities;
using Domain.Model.Interfaces.Gateways;
using DrivenAdapters.Mongo.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrivenAdapters.Mongo
{
    /// <summary>
    /// Repositorio mongo cuentas
    /// </summary>
    public class CuentaRepository : ICuentaRepository
    {
        private readonly IMongoCollection<CuentaEntity> _coleccionCuentas;
        private readonly IMongoCollection<TransaccionEntity> _coleccionTransaccion;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="mongodb"></param>
        public CuentaRepository(IContext mongodb)
        {
            _coleccionCuentas = mongodb.Cuentas;
            _coleccionTransaccion = mongodb.Transacciones;
        }
        /// <summary>
        /// Actualizar una cuenta
        /// </summary>
        /// <param name="idCuenta"></param>
        /// <param name="cambios"></param>
        /// <returns></returns>
        public async Task<Cuenta> ActualizarCuenta(string idCuenta, Cuenta cambios)
        {
            CuentaEntity cuentaObjetivo = await (await _coleccionCuentas.FindAsync(Builders<CuentaEntity>.Filter.Eq(c => c.Id, idCuenta))).FirstOrDefaultAsync();
            FindOneAndReplaceOptions<CuentaEntity> options = new();
            options.ReturnDocument = ReturnDocument.After;
            CuentaEntity cambiosEntity = new()
            {
                Id = idCuenta,
                Saldo = cambios.Saldo,
                SaldoDisponible = cambios.SaldoDisponible,
                Sobregiro = cambios.Sobregiro,
                EstadoDeCuenta = cambios.EstadoDeCuenta,
                TipoDeCuenta = cambios.TipoDeCuenta,
                EsNoGravable = cambios.EsNoGravable,
                IdCliente = cuentaObjetivo.IdCliente
            };

            CuentaEntity cuenta = await _coleccionCuentas.FindOneAndReplaceAsync(Builders<CuentaEntity>.Filter.Eq(c => c.Id, idCuenta),
                cambiosEntity, options);

            return cuenta.AsEntity();
        }
        /// <summary>
        /// Encontrar todas las cuentas de un usuario 
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        public async Task<IList<Cuenta>> EncontrarCuentasPorDocumentoDelUsuario(string idUsuario)
        {

            var cuentas = (await _coleccionCuentas.FindAsync(Builders<CuentaEntity>.Filter.Eq(c => c.IdCliente, idUsuario)))
                .ToList()
                .Select(cuentaEntity => cuentaEntity.AsEntity())
                .ToList()
                .Select(async cuenta =>
                {
                    var transacciones = (await _coleccionTransaccion.FindAsync(Builders<TransaccionEntity>.Filter.Eq(t => t.IdCuenta, cuenta.Id))).ToList();
                    if (transacciones.Count == 0)
                    {
                        cuenta.Transacciones = new List<Transaccion>();
                        return cuenta;
                    }
                        

                    cuenta.Transacciones = transacciones
                    .ToList()
                    .Select(t => t.AsEntity())
                    .ToList();

                    return cuenta;
                });

            return await Task.WhenAll(cuentas);
            
        }
        /// <summary>
        /// Mirar si existe la cuenta
        /// </summary>
        /// <param name="idCuenta"></param>
        /// <returns></returns>
        public async Task<bool> ExisteCuentaConId(string idCuenta)
        {
            return (await _coleccionCuentas.CountDocumentsAsync(Builders<CuentaEntity>.Filter.Eq(c => c.Id, idCuenta))) > 0;
        }
        /// <summary>
        /// Guardar una cuenta
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="cuenta"></param>
        /// <returns></returns>
        public async Task<Cuenta> GuardarCuenta(string idCliente, Cuenta cuenta)
        {
            var cuentaEntity = new CuentaEntity()
            {
                Id = cuenta.Id,
                Saldo = cuenta.Saldo,
                SaldoDisponible = cuenta.SaldoDisponible,
                EsNoGravable = cuenta.EsNoGravable,
                EstadoDeCuenta = cuenta.EstadoDeCuenta,
                Sobregiro = cuenta.Sobregiro,
                TipoDeCuenta = cuenta.TipoDeCuenta,
                IdCliente = idCliente
            };

            await _coleccionCuentas.InsertOneAsync(cuentaEntity);

            return cuenta;
        }
        /// <summary>
        /// Obtener cuenta por id
        /// </summary>
        /// <param name="idCuenta"></param>
        /// <returns></returns>
        public async Task<Cuenta> ObtenerCuentaPorId(string idCuenta)
        {
            CuentaEntity cuentaObjetivo = await (await _coleccionCuentas.FindAsync(Builders<CuentaEntity>.Filter.Eq(c => c.Id, idCuenta))).FirstOrDefaultAsync();

            Cuenta result = cuentaObjetivo.AsEntity();

            var transacciones = (await _coleccionTransaccion.FindAsync(Builders<TransaccionEntity>.Filter.Eq(t => t.IdCuenta, idCuenta)));

            if(transacciones is null)
            {
                result.Transacciones = new List<Transaccion>();
                return result;
            }

            result.Transacciones = transacciones
                .ToList()
                .Select(t => t.AsEntity())
                .ToList();

            return result;
        }
    }
}
