using credinet.exception.middleware.models;
using Domain.Model.Entities;
using Domain.Model.Enums;
using Domain.Model.Interfaces.Gateways;
using Domain.Model.Interfaces.UseCases.Transacciones;
using Helpers.Commons.Exceptions;
using Helpers.ObjectsUtils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.UseCase.Transacciones
{
    /// <summary>
    /// Retirar UseCase
    /// </summary>
    public class RetirarUseCase : IRetirarUseCase
    {
        public readonly ITransaccionRepository _transaccionRepository;
        public readonly ICuentaRepository _cuentaRepository;
        public readonly IClienteRepository _clienteRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RetirarUseCase"/> class.
        /// </summary>
        /// <param name="transaccionRepository"></param>
        /// <param name="cuentaRepository"></param>
        /// <param name="clienteRepository"></param>
        public RetirarUseCase(
            ITransaccionRepository transaccionRepository,
            ICuentaRepository cuentaRepository,
            IClienteRepository clienteRepository)
        {
            _transaccionRepository = transaccionRepository;
            _cuentaRepository = cuentaRepository;
            _clienteRepository = clienteRepository;
        }

        /// <summary>
        /// <see cref="IRetirarUseCase.Retirar(string, string, decimal, string?)"/>
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idCuenta"></param>
        /// <param name="monto"></param>
        /// <param name="mensaje"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public async Task<Transaccion> Retirar(string idCliente, string idCuenta, decimal monto, string? mensaje = "")
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

            if (!await _clienteRepository.ClienteConDocumentoDeIdentidadTieneCuentaConId(idCliente, idCuenta))
            {
                throw new BusinessException(TipoExcepcionNegocio.CuentaNoExisteEnCliente.GetDescription(),
                    (int)TipoExcepcionNegocio.CuentaNoExisteEnCliente);
            }

            Cuenta cuenta = await _cuentaRepository.ObtenerCuentaPorId(idCuenta);

            if (cuenta.EstadoDeCuenta == EstadosDeCuenta.CANCELADA)
            {
                throw new BusinessException(TipoExcepcionNegocio.TransaccionFallidaCuentaCancelada.GetDescription(),
                    (int)TipoExcepcionNegocio.TransaccionFallidaCuentaCancelada);
            }

            if (cuenta.EstadoDeCuenta == EstadosDeCuenta.INACTIVA)
            {
                throw new BusinessException(TipoExcepcionNegocio.TransaccionFallidaCuentaInactiva.GetDescription(),
                    (int)TipoExcepcionNegocio.TransaccionFallidaCuentaInactiva);
            }

            if (cuenta.TipoDeCuenta == TiposDeCuenta.CORRIENTE &&
                monto > cuenta.SaldoDisponible + (3000000m - cuenta.Sobregiro - ((3000000m - cuenta.Sobregiro) * 0.004m)))
            {
                throw new BusinessException(TipoExcepcionNegocio.SaldoInsuficiente.GetDescription(),
                    (int)TipoExcepcionNegocio.SaldoInsuficiente);
            }

            if (cuenta.TipoDeCuenta == TiposDeCuenta.AHORROS && monto > cuenta.SaldoDisponible)
            {
                throw new BusinessException(TipoExcepcionNegocio.SaldoInsuficiente.GetDescription(),
                    (int)TipoExcepcionNegocio.SaldoInsuficiente);
            }

            var montoSobregiroMovimiento = monto > cuenta.SaldoDisponible ? monto - cuenta.SaldoDisponible : 0;
            var gravamenMovimiento = cuenta.EsNoGravable ? 0 : (monto - montoSobregiroMovimiento) * 0.004m;
            var gravamenMovimientoSobregiro = montoSobregiroMovimiento == 0 || cuenta.EsNoGravable ? 0 : montoSobregiroMovimiento * 0.004m;

            cuenta.ActualizarSaldo(monto + gravamenMovimiento >= cuenta.Saldo ? 0 : cuenta.Saldo - monto - gravamenMovimiento);
            cuenta.ActualizarSobregiro(cuenta.Sobregiro + montoSobregiroMovimiento + gravamenMovimientoSobregiro);
            cuenta.CalcularSaldoDisponible();

            await _cuentaRepository.ActualizarCuenta(cuenta.Id, cuenta);

            Transaccion transaccion = new()
            {
                Monto = monto,
                TipoDeTransaccion = TipoDeTransacciones.RETIRO,
                Mensaje = mensaje,
                FechaDelMovimiento = DateTime.UtcNow,
                GravamenDelMovimiento = gravamenMovimiento + gravamenMovimientoSobregiro
            };

            return await _transaccionRepository.GuardarTransaccion(idCuenta, transaccion);
        }
    }
}