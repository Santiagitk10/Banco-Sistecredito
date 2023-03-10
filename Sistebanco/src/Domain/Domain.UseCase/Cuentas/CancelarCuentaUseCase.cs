using credinet.exception.middleware.models;
using Domain.Model.Entities;
using Domain.Model.Enums;
using Domain.Model.Interfaces.Gateways;
using Domain.Model.Interfaces.UseCases.Cuentas;
using Helpers.Commons.Exceptions;
using Helpers.ObjectsUtils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.UseCase.Cuentas
{
    /// <summary>
    /// Cancelar cuenta 
    /// </summary>
    public class CancelarCuentaUseCase : ICancelarCuentaUseCase
    {
        private readonly ICuentaRepository _repositorioCuenta;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cuentaRepository"></param>
        public CancelarCuentaUseCase(ICuentaRepository cuentaRepository)
        {
            _repositorioCuenta = cuentaRepository;
        }
        /// <summary>
        /// Cancelar cuenta
        /// </summary>
        /// <param name="idCuenta"></param>
        /// <returns></returns>
        public async Task CancelarCuenta(string idCuenta)
        {
            if (!await _repositorioCuenta.ExisteCuentaConId(idCuenta))
            {
                throw new BusinessException(TipoExcepcionNegocio.CuentaNoExisteEnCliente.GetDescription(),(int)TipoExcepcionNegocio.CuentaNoExisteEnCliente);
            }

            Cuenta cuenta = await _repositorioCuenta.ObtenerCuentaPorId(idCuenta);

            if (cuenta.Saldo > 0 || cuenta.Sobregiro > 0)
            {
                throw new BusinessException(TipoExcepcionNegocio.CancelacionFallidaPorEstadoDeCuentaInvalido.GetDescription(), (int)TipoExcepcionNegocio.CancelacionFallidaPorEstadoDeCuentaInvalido);
            }

            cuenta.CambiarEstadoDeCuenta(EstadosDeCuenta.CANCELADA);
            cuenta.CambiarEstadoGravable(false);

            await _repositorioCuenta.ActualizarCuenta(cuenta.Id, cuenta);
        }
    }
}
