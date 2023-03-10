using Domain.Model.Entities;
using Domain.Model.Enums;
using Domain.Model.Interfaces.Gateways;
using DrivenAdapters.Mongo.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace DrivenAdapters.Mongo
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly IMongoCollection<ClienteEntity> _coleccionCliente;
        private readonly IMongoCollection<CuentaEntity> _coleccionCuenta;
        private readonly IMongoCollection<TransaccionEntity> _coleccionTransaccion;

        public ClienteRepository(IContext mongodb)
        {
            _coleccionCliente = mongodb.Clientes;
            _coleccionCuenta = mongodb.Cuentas;
            _coleccionTransaccion = mongodb.Transacciones;
        }

        public async Task<Cliente> ActualizarDatos(string documento, Cliente cambios)
        {
            ClienteEntity clienteEntity = new()
            {
                Id = cambios.Id,
                DocumentoDeIdentidad = documento,
                Apellidos = cambios.Apellidos,
                Nombres = cambios.Nombres,
                TipoDeDocumento = cambios.TipoDeDocumento,
                CorreoElectronico = cambios.CorreoElectronico,
                FechaDeCreacion = cambios.FechaDeCreacion,
                FechaDeNacimiento = cambios?.FechaDeNacimiento,
                FechaUltimaModificacion = DateTime.UtcNow
            };
            FindOneAndReplaceOptions<ClienteEntity> options = new()
            {
                ReturnDocument = ReturnDocument.After
            };

            ClienteEntity after = await _coleccionCliente.FindOneAndReplaceAsync(Builders<ClienteEntity>.Filter.Eq(c => c.DocumentoDeIdentidad, documento),
                clienteEntity);

            return after.AsEntity();
        }

        public async Task<bool> ClienteConDocumentoDeIdentidadTieneCuentaConId(string documentoDeIdentidad, string idCuenta)
        {
            return (await _coleccionCuenta.CountDocumentsAsync(Builders<CuentaEntity>.Filter.Where(c => c.IdCliente == documentoDeIdentidad && c.Id == idCuenta))) > 0;
        }

        public async Task<bool> ClienteExisteConDocumentoDeIdentidad(string documento)
        {
            return (await _coleccionCliente.CountDocumentsAsync(Builders<ClienteEntity>.Filter.Where(c => c.DocumentoDeIdentidad == documento))) > 0;
        }

        public async Task<Cliente> GuardarCliente(Cliente cliente)
        {
            ClienteEntity clienteEntity = new()
            {
                Id = cliente.Id,
                DocumentoDeIdentidad = cliente.DocumentoDeIdentidad,
                Apellidos = cliente.Apellidos,
                Nombres = cliente.Nombres,
                TipoDeDocumento = cliente.TipoDeDocumento,
                CorreoElectronico = cliente.CorreoElectronico,
                FechaDeCreacion = DateTime.UtcNow,
                FechaDeNacimiento = cliente.FechaDeNacimiento,
            };

            await _coleccionCliente.InsertOneAsync(clienteEntity);
            var clienteRetorno = clienteEntity.AsEntity();
            clienteRetorno.Cuentas = new List<Cuenta>();
            return clienteRetorno;
        }

        public async Task<Cliente> ObtenerClientePorDocumento(string documento)
        {
            ClienteEntity result = await (await _coleccionCliente.FindAsync(Builders<ClienteEntity>.Filter.Eq(c => c.DocumentoDeIdentidad, documento))).FirstOrDefaultAsync();

            if (result is null)
                return null;

            Cliente cliente = result.AsEntity();

            List<Task<Cuenta>> cuentas = (await _coleccionCuenta.FindAsync(Builders<CuentaEntity>.Filter.Eq(a => a.IdCliente, documento)))
                .ToList()
                .Select(c => c.AsEntity())
                .Select(async cuent =>
                {
                    cuent.Transacciones = (await _coleccionTransaccion.FindAsync(Builders<TransaccionEntity>.Filter.Where(t => t.IdCuenta == cuent.Id || t.IdCuentaDeDestino == cuent.Id)))
                    .ToList()
                    .Select(t => t.AsEntity())
                    .ToList();

                    return cuent;
                })
                .ToList();

            cliente.Cuentas = await Task.WhenAll(cuentas);
            
            return cliente;
        }

        public async Task<IList<Cliente>> ObtenerTodosLosClientes()
        {
            List<ClienteEntity> clientes = (await _coleccionCliente.FindAsync(Builders<ClienteEntity>.Filter.Empty)).ToList();

            List<Task<Cliente>> cli = clientes.Select(async c =>
            {
                Cliente clienteDominio = c.AsEntity();

                IEnumerable<Task<Cuenta>> cuentasDelCliente = (await _coleccionCuenta.FindAsync(Builders<CuentaEntity>.Filter.Eq(a => a.IdCliente, c.DocumentoDeIdentidad)))
                .ToList()
                .Select(c => c.AsEntity())
                .ToList()
                .Select(async cuent =>
                {
                    cuent.Transacciones = (await _coleccionTransaccion.FindAsync(Builders<TransaccionEntity>.Filter.Where(t => t.IdCuenta == cuent.Id || t.IdCuentaDeDestino == cuent.Id)))
                    .ToList()
                    .Select(t => t.AsEntity())
                    .ToList();

                    return cuent;
                });

                clienteDominio.Cuentas = await Task.WhenAll(cuentasDelCliente);

                return clienteDominio;
            }).ToList();

            return await Task.WhenAll(cli);
        }
    }
}