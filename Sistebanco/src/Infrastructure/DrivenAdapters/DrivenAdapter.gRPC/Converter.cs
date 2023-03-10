using Google.Protobuf.WellKnownTypes;
using GrpcServicioTransaccion;
using GrpcServicioCuenta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Transaccion = GrpcServicioCliente.Transaccion;
using Cuenta = GrpcServicioCliente.Cuenta;
using Cliente = GrpcServicioCliente.Cliente;
using Domain.Model.Enums;
using GrpcServicioCliente;

namespace DrivenAdapter.gRPC
{
    public static class Converter
    {
        //public static Transaccion FromDomainTransaccionToGrpcTransaccion(Domain.Model.Entities.Transaccion transaccionDominio)
        //{
        //    return new()
        //    {
        //        Id = transaccionDominio.Id,
        //        TipoTransaccion = transaccionDominio.TipoDeTransaccion.ToString(),
        //        Mensaje = transaccionDominio.Mensaje,
        //        Monto = transaccionDominio.Monto.ToString(),
        //        IdCuentaDestino = transaccionDominio.IdCuentaDeDestino,
        //        FechaDelMovimiento = Timestamp.FromDateTime(transaccionDominio.FechaDelMovimiento),
        //        GravamenMovimiento = transaccionDominio.GravamenDelMovimiento.ToString()
        //    };
        //}

        public static Transaccion FromDomainTransaccionToGrpcTransaccion(Domain.Model.Entities.Transaccion transaccionDominio)
        {
            return new()
            {
                Id = transaccionDominio.Id,
                TipoTransaccion = transaccionDominio.TipoDeTransaccion.ToString(),
                Monto = transaccionDominio.Monto.ToString(),
                FechaDelMovimiento = Timestamp.FromDateTime(transaccionDominio.FechaDelMovimiento),
                GravamenDelMovimiento = transaccionDominio.GravamenDelMovimiento.ToString()
            };
        }

        public static Cuenta FromDomainCuentaToGrpcCuenta(Domain.Model.Entities.Cuenta cuentaDominio)
        {
            GrpcServicioCliente.Transacciones transacciones = new();

            if (cuentaDominio.Transacciones.Count > 0)
            {
                transacciones.Transacciones_.AddRange(
                cuentaDominio.Transacciones.Select(c => FromDomainTransaccionToGrpcTransaccion(c)));
            }

            return new()
            {
                Id = cuentaDominio.Id,
                Saldo = cuentaDominio.Saldo.ToString(),
                SaldoDisponible = cuentaDominio.SaldoDisponible.ToString(),

                EsNoGravable = cuentaDominio.EsNoGravable,
                EstadoDeCuenta = cuentaDominio.EstadoDeCuenta.ToString(),
                TipoDeCuenta = cuentaDominio.TipoDeCuenta.ToString(),
                Sobregiro = cuentaDominio.Sobregiro.ToString(),
                Transacciones = transacciones
            };
        }

        public static Cliente FromDomainClienteToGrpcCliente(Domain.Model.Entities.Cliente clienteDominio)
        {
            GrpcServicioCliente.Cuentas cuentas = new();
            Timestamp? fechaUltimaModificacion = null;
            Timestamp? FechaDeNacimiento = null;
            if (clienteDominio.FechaUltimaModificacion is not null)
            {
                fechaUltimaModificacion = Timestamp.FromDateTime((DateTime)clienteDominio.FechaUltimaModificacion);
            }

            if (clienteDominio.FechaDeNacimiento is not null)
            {
                FechaDeNacimiento = Timestamp.FromDateTime((DateTime)clienteDominio.FechaDeNacimiento);
            }

            if (clienteDominio.Cuentas is null)
            {
                clienteDominio.Cuentas = new List<Domain.Model.Entities.Cuenta>();
            }

            if (clienteDominio.Cuentas.Count > 0)
            {
                cuentas.Cuentas_.AddRange(
                    clienteDominio.Cuentas.Select(c => FromDomainCuentaToGrpcCuenta(c)));
            }

            return new()
            {
                Id = clienteDominio.Id,
                Nombres = clienteDominio.Nombres,
                Appellidos = clienteDominio.Apellidos,
                CorreoElectronico = clienteDominio.CorreoElectronico,
                Cuentas = cuentas,
                FechaDeCreacion = Timestamp.FromDateTime(clienteDominio.FechaDeCreacion),
                DocumentoIdentidad = clienteDominio.DocumentoDeIdentidad,
                TipoDocumento = clienteDominio.TipoDeDocumento.ToString(),
                FechaDeModificacion = fechaUltimaModificacion,
                FechaDeNacimiento = FechaDeNacimiento
            };
        }

        public static Domain.Model.Entities.Cliente FromGrpcCrearClienteToDomainCliente(ClienteCrear cliente)
        {
            System.Enum.TryParse<DocumentosDeIdentidad>(cliente.TipoDocumento, true, out DocumentosDeIdentidad tipo);

            var listaCuentas = new List<Domain.Model.Entities.Cuenta>();

            DateTime? fechaDeNacimiento = null;

            if (cliente.FechaDeNacimiento is not null)
            {
                fechaDeNacimiento = cliente.FechaDeNacimiento.ToDateTime();
            }

            if (cliente.Cuentas is not null)
            {
                listaCuentas = cliente.Cuentas.Cuentas_.Select(c => FromGrpcCuentaToDomainCuenta(c)).ToList();
            }

            return new()
            {
                CorreoElectronico = cliente.CorreoElectronico,
                DocumentoDeIdentidad = cliente.DocumentoIdentidad,
                TipoDeDocumento = tipo,
                Nombres = cliente.Nombres,
                Apellidos = cliente.Appellidos,
                Cuentas = listaCuentas,
                FechaDeNacimiento = fechaDeNacimiento
            };
        }

        public static Domain.Model.Entities.Cliente FromGrpcClienteToDomainCliente(Cliente cliente)
        {
            System.Enum.TryParse<DocumentosDeIdentidad>(cliente.TipoDocumento, true, out DocumentosDeIdentidad tipo);

            return new()
            {
                Id = cliente.Id,
                CorreoElectronico = cliente.CorreoElectronico,
                DocumentoDeIdentidad = cliente.DocumentoIdentidad,
                TipoDeDocumento = tipo,
                Nombres = cliente.Nombres,
                Apellidos = cliente.Appellidos,
                Cuentas = cliente.Cuentas.Cuentas_.Select(c => FromGrpcCuentaToDomainCuenta(c)).ToList(),
                FechaDeCreacion = cliente.FechaDeCreacion.ToDateTime(),
                FechaDeNacimiento = cliente.FechaDeNacimiento.ToDateTime(),
                FechaUltimaModificacion = cliente.FechaDeModificacion.ToDateTime()
            };
        }

        public static Domain.Model.Entities.Cuenta FromGrpcCuentaToDomainCuenta(GrpcServicioCliente.Cuenta cuenta)
        {
            System.Enum.TryParse<TiposDeCuenta>(cuenta.TipoDeCuenta, true, out TiposDeCuenta tipo);
            System.Enum.TryParse<EstadosDeCuenta>(cuenta.EstadoDeCuenta, true, out EstadosDeCuenta estadoDeCuenta);
            Decimal.TryParse(cuenta.Saldo, out decimal saldo);
            Decimal.TryParse(cuenta.SaldoDisponible, out decimal saldoDisponible);
            Decimal.TryParse(cuenta.Sobregiro, out decimal sobregiro);

            return new()
            {
                Id = cuenta.Id,
                Saldo = saldo,
                SaldoDisponible = saldoDisponible,
                Transacciones = cuenta.Transacciones.Transacciones_.Select(t => FromGrpcTransaccionToDomainTransaccion(t)).ToList(),
                EsNoGravable = cuenta.EsNoGravable,
                EstadoDeCuenta = estadoDeCuenta,
                Sobregiro = sobregiro,
                TipoDeCuenta = tipo,
            };
        }

        public static Domain.Model.Entities.Transaccion FromGrpcTransaccionToDomainTransaccion(GrpcServicioCliente.Transaccion transaccionDominio)
        {
            System.Enum.TryParse<TipoDeTransacciones>(transaccionDominio.TipoTransaccion, true, out TipoDeTransacciones tipo);
            Decimal.TryParse(transaccionDominio.Monto, out decimal monto);
            Decimal.TryParse(transaccionDominio.GravamenDelMovimiento, out decimal gravamen);

            return new()
            {
                Id = transaccionDominio.Id,
                Monto = monto,
                TipoDeTransaccion = tipo,
                IdCuentaDeDestino = transaccionDominio.IdCuentaDestino,
                Mensaje = transaccionDominio.Mensaje,
                FechaDelMovimiento = transaccionDominio.FechaDelMovimiento.ToDateTime(),
                GravamenDelMovimiento = gravamen
            };
        }

        public static Domain.Model.Entities.Cuenta FromGrpcCuentaRequestToDomainCuenta(CuentaRequest cuentaRequest)
        {
            System.Enum.TryParse<TiposDeCuenta>(cuentaRequest.TipoDeCuenta, true, out TiposDeCuenta tipo);

            return new()
            {
                Id = "",
                Saldo = 0,
                SaldoDisponible = 0,
                Transacciones = new List<Domain.Model.Entities.Transaccion>(),
                EsNoGravable = false,
                EstadoDeCuenta = EstadosDeCuenta.ACTIVA,
                Sobregiro = 0,
                TipoDeCuenta = tipo
            };
        }
    }
}