using Domain.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.Entities
{
    /// <summary>
    /// Cuenta
    /// </summary>
    public class Cuenta
    {
        /// <summary>
        /// Id de la cuenta
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Saldo total en la cuenta
        /// </summary>
        public decimal Saldo { get; set; }

        /// <summary>
        /// Saldo disponible despues de gravamen (Lo que puede retirar)
        /// </summary>
        public decimal SaldoDisponible { get; set; }

        /// <summary>
        /// Lista de transacciones
        /// </summary>
        public IList<Transaccion> Transacciones { get; set; }

        /// <summary>
        /// Es gravable por 4x1000
        /// </summary>
        public bool EsNoGravable { get; set; }

        /// <summary>
        /// Estado de la cuenta
        /// </summary>
        public EstadosDeCuenta EstadoDeCuenta { get; set; }

        /// <summary>
        /// Tipo de cuenta
        /// </summary>
        public TiposDeCuenta TipoDeCuenta { get; set; }

        /// <summary>
        /// Sobregiro
        /// </summary>
        public decimal Sobregiro { get; set; }

        /// <summary>
        /// Cambiar el estado de la cuenta
        /// </summary>
        /// <param name="estado"></param>
        public void CambiarEstadoDeCuenta(EstadosDeCuenta estado)
        {
            EstadoDeCuenta = estado;
        }

        /// <summary>
        /// Calcular el saldo descontando el gravamen
        /// </summary>
        public void CalcularSaldoDisponible()
        {
            SaldoDisponible = EsNoGravable ? Saldo : Saldo - (Saldo * 0.004m);
        }

        /// <summary>
        /// Añadir una transacción
        /// </summary>
        /// <param name="transaccion"></param>
        public void AdicionarTransaccion(Transaccion transaccion)
        {
            Transacciones.Add(transaccion);
        }

        /// <summary>
        /// Seleccionar como cuenta no gravable
        /// </summary>
        public void CambiarEstadoGravable(bool estado)
        {
            EsNoGravable = estado;
        }

        /// <summary>
        /// Actualizar saldo del usuario
        /// </summary>
        /// <param name="saldo"></param>
        public void ActualizarSaldo(decimal saldo)
        {
            Saldo = saldo;
        }

        public void ActualizarSobregiro(decimal sobregiro)
        {
            Sobregiro = sobregiro;
        }
    }
}