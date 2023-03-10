using Domain.Model.Entities;
using Domain.Model.Enums;
using Domain.Model.Interfaces.Gateways;
using Domain.Model.Interfaces.UseCases.Cuentas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.UseCase.Cuentas
{
    /// <summary>
    /// Caso de uso para la creación de cuenta
    /// </summary>
    public class CrearCuentaUseCase : ICrearCuentaUseCase
    {
        private readonly ICuentaRepository _repositorioCuenta;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cuentaRepository"></param>
        public CrearCuentaUseCase(ICuentaRepository cuentaRepository)
        {
            _repositorioCuenta = cuentaRepository;
        }
        /// <summary>
        /// Crear cuenta
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="cuenta"></param>
        /// <returns></returns>
        public async Task<Cuenta> CrearCuenta(string idCliente, Cuenta cuenta)
        {
            do
            {
                var generatedId = GenerarId(8);
                if (cuenta.TipoDeCuenta.Equals(TiposDeCuenta.AHORROS))
                {
                    cuenta.Id = "46" + generatedId;
                }

                if (cuenta.TipoDeCuenta.Equals(TiposDeCuenta.CORRIENTE))
                {
                    cuenta.Id = "23" + generatedId;
                }
                
            } while (await _repositorioCuenta.ExisteCuentaConId(cuenta.Id));

            cuenta.CambiarEstadoGravable(false);
            cuenta.CambiarEstadoDeCuenta(EstadosDeCuenta.ACTIVA);
            cuenta.Transacciones = new List<Transaccion>();

            if ((await _repositorioCuenta.EncontrarCuentasPorDocumentoDelUsuario(idCliente)).Count == 0
                && cuenta.TipoDeCuenta.Equals(TiposDeCuenta.AHORROS))
            {
                cuenta.CambiarEstadoGravable(true);
            }
      
            await _repositorioCuenta.GuardarCuenta(idCliente, cuenta);

            return cuenta;
        }

        private string GenerarId(int length)
        {
            const string chars = "0123456789";
            var sb = new StringBuilder(length);
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                sb.Append(chars[random.Next(0,chars.Length)]);
            }
            return sb.ToString();
        }

    }
}
