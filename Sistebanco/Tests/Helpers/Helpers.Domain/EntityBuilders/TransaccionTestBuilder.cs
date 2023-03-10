using Domain.Model.Entities;
using Domain.Model.Enums;

namespace Helpers.Domain.EntityBuilders;

public class TransaccionTestBuilder
{
    private readonly Transaccion _transaccion;

    public TransaccionTestBuilder()
    {
        _transaccion = new Transaccion();
    }

    public TransaccionTestBuilder ConId(string id)
    {
        _transaccion.Id = id;
        return this;
    }

    public TransaccionTestBuilder ConMonto(decimal monto)
    {
        _transaccion.Monto = monto;
        return this;
    }

    public TransaccionTestBuilder ConTipoDeTransaccion(TipoDeTransacciones tipo)
    {
        _transaccion.TipoDeTransaccion = tipo;
        return this;
    }

    public TransaccionTestBuilder ConIdCuentaDeDestino(string idCuenta)
    {
        _transaccion.IdCuentaDeDestino = idCuenta;
        return this;
    }

    public TransaccionTestBuilder ConMensaje(string mensaje)
    {
        _transaccion.Mensaje = mensaje;
        return this;
    }

    public TransaccionTestBuilder ConFechaDelMovimiento(DateTime fecha)
    {
        _transaccion.FechaDelMovimiento = fecha;
        return this;
    }

    public TransaccionTestBuilder ConGravamenDelMovimiento(decimal gravamen)
    {
        _transaccion.GravamenDelMovimiento = gravamen;
        return this;
    }

    public Transaccion Build()
    {
        return _transaccion;
    }
}