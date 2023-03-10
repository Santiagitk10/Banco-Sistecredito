using System.ComponentModel;

namespace Helpers.Commons.Exceptions;

/// <summary>
/// ResponseError
/// </summary>
public enum TipoExcepcionNegocio
{
    /// <summary>
    /// Tipo de exception no controlada
    /// </summary>
    [Description("Excepción de negocio no controlada")]
    ExceptionNoControlada = 0,

    [Description("El cliente con el documento de identidad ingresado no existe.")]
    ClienteConDocumentoDeIdentidadNoExiste = 1,

    [Description("La cuenta con el id ingresado no existe dentro del cliente.")]
    CuentaNoExisteEnCliente = 2,

    [Description("La operación que se intentó realizar fallo por saldo insuficiente.")]
    SaldoInsuficiente = 3,

    [Description("La cuenta destino no se encuentra disponible.")]
    CuentaDestinoNoDisponible = 4,

    [Description("La cuenta no se puede cancelar debido a que se encuentra en un estado invalido.")]
    CancelacionFallidaPorEstadoDeCuentaInvalido = 5,

    [Description("Un menor de edad no puede ser cliente.")]
    ClienteMenorDeEdad = 6,

    [Description("El monto ingresado debe que ser mayor a $0")]
    TransaccionFallidaMontoInvalido = 7,

    [Description("Transacción Fallida. La cuenta se encuentra cancelada")]
    TransaccionFallidaCuentaCancelada = 8,

    [Description("Transacción Fallida. La cuenta se encuentra inactiva")]
    TransaccionFallidaCuentaInactiva = 9,

    [Description("Transacción Fallida. La cuenta destino con el ID ingresado no existe")]
    TransaccionFallidaCuentaDestinoInexistente = 10,
}