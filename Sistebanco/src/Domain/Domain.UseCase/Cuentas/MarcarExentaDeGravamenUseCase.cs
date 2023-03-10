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
    /// Marcar una cuenta exenta de gravamen 4x1000
    /// </summary>
    public class MarcarExentaDeGravamenUseCase : IMarcarExentaDeGravamenUseCase
    {
        private readonly ICuentaRepository _cuentaRepository;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cuentaRepository"></param>
        public MarcarExentaDeGravamenUseCase(ICuentaRepository cuentaRepository)
        {
            _cuentaRepository = cuentaRepository;
        }
        /// <summary>
        /// Marcar cuenta como exenta de 4x1000
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idCuenta"></param>
        /// <returns></returns>
        public async Task<Cuenta> MarcarExentaDeGravamen(string idCliente, string idCuenta)
        {
            List<Cuenta> cuentasDelUsuario = (await _cuentaRepository.EncontrarCuentasPorDocumentoDelUsuario(idCliente)).ToList();

            if (!cuentasDelUsuario.Any(c => c.Id == idCuenta && c.EstadoDeCuenta.Equals(EstadosDeCuenta.ACTIVA)))
            {
                throw new BusinessException(TipoExcepcionNegocio.CuentaNoExisteEnCliente.GetDescription(), (int)TipoExcepcionNegocio.CuentaNoExisteEnCliente);
            }

            if (cuentasDelUsuario.Any(c => c.EsNoGravable == true))
            {
                Cuenta cuentaDesmarcada = cuentasDelUsuario.Find(c => c.EsNoGravable == true);
                cuentaDesmarcada.CambiarEstadoGravable(false);
                await _cuentaRepository.ActualizarCuenta(cuentaDesmarcada.Id, cuentaDesmarcada);
            }
            
            Cuenta cuentaMarcada = cuentasDelUsuario.Find(c => c.Id == idCuenta && c.EstadoDeCuenta.Equals(EstadosDeCuenta.ACTIVA));
            cuentaMarcada.CambiarEstadoGravable(true);
            await _cuentaRepository.ActualizarCuenta(cuentaMarcada.Id, cuentaMarcada);

            return cuentaMarcada;
        }
    }
}
