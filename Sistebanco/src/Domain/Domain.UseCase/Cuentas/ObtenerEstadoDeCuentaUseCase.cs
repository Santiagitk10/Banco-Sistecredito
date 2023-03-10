using credinet.exception.middleware.models;
using Domain.Model.Entities;
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
    /// Obtener estado de cuenta
    /// </summary>
    public class ObtenerEstadoDeCuentaUseCase : IObtenerEstadoDeCuentaUseCase
    {
        private readonly ICuentaRepository _repositorioCuenta;
        private readonly ITransaccionRepository _repositorioTransaccion;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repositorioCuenta"></param>
        /// <param name="repositorioTransaccion"></param>
        public ObtenerEstadoDeCuentaUseCase(ICuentaRepository repositorioCuenta, ITransaccionRepository repositorioTransaccion)
        {
            _repositorioCuenta = repositorioCuenta;
            _repositorioTransaccion = repositorioTransaccion;
        }
        /// <summary>
        /// Obtener estado de cuenta
        /// </summary>
        /// <param name="idCuenta"></param>
        /// <returns></returns>
        public async Task<Cuenta> ObtenerEstadoDeCuenta(string idCuenta)
        {
            if (!await _repositorioCuenta.ExisteCuentaConId(idCuenta))
            {
                throw new BusinessException(TipoExcepcionNegocio.CuentaNoExisteEnCliente.GetDescription(), (int)TipoExcepcionNegocio.CuentaNoExisteEnCliente);
            }

            Cuenta cuenta = await _repositorioCuenta.ObtenerCuentaPorId(idCuenta);

            return cuenta;
        }
    }
}
