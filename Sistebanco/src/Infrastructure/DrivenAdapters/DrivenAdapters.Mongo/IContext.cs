using DrivenAdapters.Mongo.Entities;
using MongoDB.Driver;

namespace DrivenAdapters.Mongo
{
    /// <summary>
    /// Interfaz Mongo context contract.
    /// </summary>
    public interface IContext
    {
        /// <summary>
        /// Transacciones
        /// </summary>
        public IMongoCollection<TransaccionEntity> Transacciones { get; }
        /// <summary>
        /// Cuentas
        /// </summary>
        public IMongoCollection<CuentaEntity> Cuentas { get; }
        /// <summary>
        /// Clientes
        /// Clientes
        /// </summary>
        public IMongoCollection<ClienteEntity> Clientes { get; }
    }
}