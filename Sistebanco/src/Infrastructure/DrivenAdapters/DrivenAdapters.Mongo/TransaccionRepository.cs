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
    public class TransaccionRepository : ITransaccionRepository
    {
        private readonly IMongoCollection<TransaccionEntity> _coleccionTransacciones;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransaccionRepository"/> class.
        /// </summary>
        /// <param name="mongodb"></param>
        public TransaccionRepository(IContext mongodb)
        {
            _coleccionTransacciones = mongodb.Transacciones;
        }

        /// <summary>
        /// <see cref="ITransaccionRepository.GuardarTransaccion(Transaccion)"/>
        /// </summary>
        /// <param name="idCuenta"></param>
        /// <param name="transaccion"></param>
        /// <returns></returns>
        public async Task<Transaccion> GuardarTransaccion(string idCuenta, Transaccion transaccion)
        {
            TransaccionEntity transaccionEntity = new()
            {
                IdCuenta = idCuenta,
                Monto = transaccion.Monto,
                TipoDeTransaccion = transaccion.TipoDeTransaccion,
                IdCuentaDeDestino = transaccion.IdCuentaDeDestino,
                Mensaje = transaccion.Mensaje,
                FechaDelMovimiento = transaccion.FechaDelMovimiento,
                GravamenDelMovimiento = transaccion.GravamenDelMovimiento
            };

            await _coleccionTransacciones.InsertOneAsync(transaccionEntity);

            return transaccionEntity.AsEntity();
        }

        public async Task<IList<Transaccion>> ObtenerPorIdDeCuenta(string idCuenta)
        {
            IAsyncCursor<TransaccionEntity> transaccionesEntity =
                await _coleccionTransacciones.FindAsync(Builders<TransaccionEntity>.Filter.Eq(transaccion =>
                transaccion.IdCuenta, idCuenta) | Builders<TransaccionEntity>.Filter.Eq(transaccion =>
                transaccion.IdCuentaDeDestino, idCuenta));

            List<Transaccion> transacciones = transaccionesEntity.ToEnumerable()
                .Select(transaccionEntity => transaccionEntity.AsEntity()).ToList();

            return transacciones;
        }
    }
}