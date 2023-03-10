using Domain.Model.Entities;
using Domain.Model.Enums;
using Domain.Model.Interfaces.Gateways;
using Domain.Model.Interfaces.UseCases.Transacciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using credinet.exception.middleware.models;
using Helpers.Commons.Exceptions;
using Helpers.ObjectsUtils.Extensions;
using DrivenAdapters.Mongo.Entities;

namespace Domain.UseCase.Transacciones
{
    public class ConsignarUseCase : IConsignarUseCase
    {
        public readonly ITransaccionRepository _transaccionRepository;
        public readonly ICuentaRepository _cuentaRepository;
        public readonly IClienteRepository _clienteRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsignarUseCase"/> class.
        /// </summary>
        /// <param name="transaccionRepository"></param>
        /// <param name="cuentaRepository"></param>
        /// <param name="clienteRepository"></param>
        public ConsignarUseCase(
            ITransaccionRepository transaccionRepository,
            ICuentaRepository cuentaRepository,
            IClienteRepository clienteRepository)
        {
            _transaccionRepository = transaccionRepository;
            _cuentaRepository = cuentaRepository;
            _clienteRepository = clienteRepository;
        }

        /// <summary>
        /// <see cref="IConsignarUseCase.Consignar(string, string, decimal, string?)"/>
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idCuenta"></param>
        /// <param name="monto"></param>
        /// <param name="mensaje"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public async Task<Transaccion> Consignar(string idCliente, string idCuenta, decimal monto, string? mensaje = "")
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

            ///Funciona independientemente si la cuenta es corriente(tiene sobregiro) o no
            cuenta.ActualizarSaldo(monto - cuenta.Sobregiro >= 0 ? cuenta.Saldo + (monto - cuenta.Sobregiro) : cuenta.Saldo);
            cuenta.ActualizarSobregiro(monto - cuenta.Sobregiro <= 0 ? cuenta.Sobregiro - monto : 0);
            cuenta.CalcularSaldoDisponible();

            await _cuentaRepository.ActualizarCuenta(cuenta.Id, cuenta);

            var gravamenMovimiento = 0;

            Transaccion transaccion = new()
            {
                Monto = monto,
                TipoDeTransaccion = TipoDeTransacciones.CONSIGNACION,
                Mensaje = mensaje,
                FechaDelMovimiento = DateTime.UtcNow,
                GravamenDelMovimiento = gravamenMovimiento
            };

            return await _transaccionRepository.GuardarTransaccion(idCuenta, transaccion);
        }
    }
}