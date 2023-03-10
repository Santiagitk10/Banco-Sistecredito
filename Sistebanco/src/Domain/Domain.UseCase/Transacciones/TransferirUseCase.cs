using credinet.exception.middleware.models;
using Domain.Model.Entities;
using Domain.Model.Enums;
using Domain.Model.Interfaces.Gateways;
using Domain.Model.Interfaces.UseCases.Transacciones;
using Helpers.Commons.Exceptions;
using Helpers.ObjectsUtils.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Domain.UseCase.Transacciones
{
    public class TransferirUseCase : ITransferirUseCase
    {
        public readonly ITransaccionRepository _transaccionRepository;
        public readonly ICuentaRepository _cuentaRepository;
        public readonly IClienteRepository _clienteRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferirUseCase"/> class.
        /// </summary>
        /// <param name="transaccionRepository"></param>
        /// <param name="cuentaRepository"></param>
        /// <param name="clienteRepository"></param>
        public TransferirUseCase(
            ITransaccionRepository transaccionRepository,
            ICuentaRepository cuentaRepository,
            IClienteRepository clienteRepository)
        {
            _transaccionRepository = transaccionRepository;
            _cuentaRepository = cuentaRepository;
            _clienteRepository = clienteRepository;
        }

        /// <summary>
        /// <see cref="ITransferirUseCase.Transferir(string, string, string, decimal, string?)"/>
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idCuentaOrigen"></param>
        /// <param name="idCuentaDestino"></param>
        /// <param name="monto"></param>
        /// <param name="mensaje"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public async Task<Transaccion> Transferir(string idCliente, string idCuentaOrigen, string idCuentaDestino, decimal monto, string? mensaje = "")
        {
            if (monto <= 0)
            {
                throw new BusinessException(TipoExcepcionNegocio.TransaccionFallidaMontoInvalido.GetDescription(),
                    (int)TipoExcepcionNegocio.TransaccionFallidaMontoInvalido);
            }

            if (!await _clienteRepository.ClienteExisteConDocumentoDeIdentidad(idCliente))
            {
                throw new BusinessException(TipoExcepcionNegocio.ClienteConDocumentoDeIdentidadNoExiste
                    .GetDescription(), (int)TipoExcepcionNegocio.ClienteConDocumentoDeIdentidadNoExiste);
            }

            if (!await _clienteRepository.ClienteConDocumentoDeIdentidadTieneCuentaConId(idCliente, idCuentaOrigen))
            {
                throw new BusinessException(TipoExcepcionNegocio.CuentaNoExisteEnCliente.GetDescription(),
                    (int)TipoExcepcionNegocio.CuentaNoExisteEnCliente);
            }

            if (!await _cuentaRepository.ExisteCuentaConId(idCuentaDestino))
            {
                throw new BusinessException(TipoExcepcionNegocio.TransaccionFallidaCuentaDestinoInexistente.GetDescription(),
                    (int)TipoExcepcionNegocio.TransaccionFallidaCuentaDestinoInexistente);
            }

            Cuenta cuentaOrigen = await _cuentaRepository.ObtenerCuentaPorId(idCuentaOrigen);
            Cuenta cuentaDestino = await _cuentaRepository.ObtenerCuentaPorId(idCuentaDestino);

            if (cuentaOrigen.EstadoDeCuenta == EstadosDeCuenta.CANCELADA ||
                cuentaDestino.EstadoDeCuenta == EstadosDeCuenta.CANCELADA)
            {
                throw new BusinessException(TipoExcepcionNegocio.TransaccionFallidaCuentaCancelada.GetDescription(),
                    (int)TipoExcepcionNegocio.TransaccionFallidaCuentaCancelada);
            }

            if (cuentaOrigen.EstadoDeCuenta == EstadosDeCuenta.INACTIVA)
            {
                throw new BusinessException(TipoExcepcionNegocio.TransaccionFallidaCuentaInactiva.GetDescription(),
                    (int)TipoExcepcionNegocio.TransaccionFallidaCuentaInactiva);
            }

            //Aumentar monto a por ejemplo 10millons para lograr cobertura posterior
            if (cuentaOrigen.TipoDeCuenta == TiposDeCuenta.CORRIENTE &&
                monto > cuentaOrigen.SaldoDisponible + (3000000 - cuentaOrigen.Sobregiro - ((3000000 - cuentaOrigen.Sobregiro) * 0.004m)))
            {
                throw new BusinessException(TipoExcepcionNegocio.SaldoInsuficiente.GetDescription(),
                    (int)TipoExcepcionNegocio.SaldoInsuficiente);
            }

            var cuentaDestinoEsDelMismoCliente = await _clienteRepository
                .ClienteConDocumentoDeIdentidadTieneCuentaConId(idCliente, idCuentaDestino);

            var ambasCuentasSonCorrientesDelMismoDueño = cuentaDestinoEsDelMismoCliente &&
                cuentaOrigen.TipoDeCuenta == TiposDeCuenta.CORRIENTE && cuentaDestino.TipoDeCuenta == TiposDeCuenta.CORRIENTE;

            //Aumentar monto a por ejemplo 10millons para lograr cobertura posterior
            if (ambasCuentasSonCorrientesDelMismoDueño && monto > cuentaOrigen.Saldo + (3000000m - cuentaOrigen.Sobregiro))
            {
                throw new BusinessException(TipoExcepcionNegocio.SaldoInsuficiente.GetDescription(),
                    (int)TipoExcepcionNegocio.SaldoInsuficiente);
            }

            if (cuentaOrigen.TipoDeCuenta == TiposDeCuenta.AHORROS && monto > cuentaOrigen.SaldoDisponible)
            {
                throw new BusinessException(TipoExcepcionNegocio.SaldoInsuficiente.GetDescription(),
                    (int)TipoExcepcionNegocio.SaldoInsuficiente);
            }

            var gravamenMovimiento = cuentaOrigen.EsNoGravable || ambasCuentasSonCorrientesDelMismoDueño ? 0 : monto * 0.004m;
            decimal montoSobregiroMovimiento;
            if (ambasCuentasSonCorrientesDelMismoDueño)
            {
                montoSobregiroMovimiento = monto > cuentaOrigen.Saldo ? monto - cuentaOrigen.Saldo : 0;
            }
            else
            {
                montoSobregiroMovimiento = monto > cuentaOrigen.SaldoDisponible ? monto - cuentaOrigen.SaldoDisponible : 0;
            }

            var gravamenMovimientoSobregiro = montoSobregiroMovimiento == 0 || cuentaOrigen.EsNoGravable || ambasCuentasSonCorrientesDelMismoDueño ? 0 : montoSobregiroMovimiento * 0.004m;

            cuentaOrigen.ActualizarSaldo(monto + gravamenMovimiento >= cuentaOrigen.Saldo ? 0 : cuentaOrigen.Saldo - monto - gravamenMovimiento);
            cuentaOrigen.ActualizarSobregiro(cuentaOrigen.Sobregiro + montoSobregiroMovimiento + gravamenMovimientoSobregiro);
            cuentaOrigen.CalcularSaldoDisponible();

            await _cuentaRepository.ActualizarCuenta(cuentaOrigen.Id, cuentaOrigen);

            ///Funciona independientemente si la cuenta es corriente(tiene sobregiro) o no
            cuentaDestino.ActualizarSaldo(monto - cuentaDestino.Sobregiro >= 0 ? cuentaDestino.Saldo + (monto - cuentaDestino.Sobregiro) : cuentaDestino.Saldo);
            cuentaDestino.ActualizarSobregiro(monto - cuentaDestino.Sobregiro <= 0 ? cuentaDestino.Sobregiro - monto : 0);
            cuentaDestino.CalcularSaldoDisponible();

            await _cuentaRepository.ActualizarCuenta(cuentaDestino.Id, cuentaDestino);

            Transaccion transaccion = new()
            {
                Monto = monto,
                TipoDeTransaccion = TipoDeTransacciones.TRANSACCION_ENTRE_CUENTAS,
                IdCuentaDeDestino = idCuentaDestino,
                Mensaje = mensaje,
                FechaDelMovimiento = DateTime.UtcNow,
                GravamenDelMovimiento = gravamenMovimiento
            };

            return await _transaccionRepository.GuardarTransaccion(idCuentaOrigen, transaccion);
        }
    }
}