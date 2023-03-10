using Domain.Model.Entities;
using Domain.Model.Enums;

namespace Helpers.Domain.EntityBuilders;

public class CuentaTestBuilder
{
    private readonly Cuenta _cuenta;

    public CuentaTestBuilder()
    {
        _cuenta = new();
    }

    public CuentaTestBuilder ConId(string id)
    {
        _cuenta.Id = id;
        return this;
    }

    public CuentaTestBuilder ConSaldo(decimal saldo)
    {
        _cuenta.Saldo = saldo;
        return this;
    }
    public CuentaTestBuilder ConSobregiro(decimal sobregiro)
    {
        _cuenta.Sobregiro = sobregiro;
        return this;
    }

    public CuentaTestBuilder ConSaldoDisponible(decimal saldoDisponible)
    {
        _cuenta.SaldoDisponible = saldoDisponible;
        return this;
    }

    public CuentaTestBuilder ConTransacciones(IList<Transaccion> transacciones)
    {
        _cuenta.Transacciones = transacciones;
        return this;
    }

    public CuentaTestBuilder EsNoGravable(bool esNoGravable)
    {
        _cuenta.EsNoGravable = esNoGravable;
        return this;
    }

    public CuentaTestBuilder ConEstadoDeCuenta(EstadosDeCuenta estadoDeCuenta)
    {
        _cuenta.EstadoDeCuenta = estadoDeCuenta;
        return this;
    }

    public CuentaTestBuilder ConTipoDeCuenta(TiposDeCuenta tipoDeCuenta)
    {
        _cuenta.TipoDeCuenta = tipoDeCuenta;
        return this;
    }

    public CuentaTestBuilder ConTransaccion(Transaccion transaccion)
    {
        _cuenta.AdicionarTransaccion(transaccion);
        return this;
    }

    public Cuenta Build()
    {
        return _cuenta;
    }
}