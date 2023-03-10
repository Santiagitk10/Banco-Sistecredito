using Domain.Model.Entities;
using Domain.Model.Enums;

namespace Helpers.Domain.EntityBuilders;

public class ClienteTestBuilder
{
    private readonly Cliente _cliente;

    public ClienteTestBuilder()
    {
        _cliente = new();
    }

    public ClienteTestBuilder ConId(string id)
    {
        _cliente.Id = id;
        return this;
    }

    public ClienteTestBuilder ConNombres(string nombres)
    {
        _cliente.Nombres = nombres;
        return this;
    }

    public ClienteTestBuilder ConApellidos(string apellidos)
    {
        _cliente.Apellidos = apellidos;
        return this;
    }

    public ClienteTestBuilder ConTipoDeDocumento(DocumentosDeIdentidad tipoDeDocumento)
    {
        _cliente.TipoDeDocumento = tipoDeDocumento;
        return this;
    }

    public ClienteTestBuilder ConDocumentoDeIdentidad(string documentoDeIdentidad)
    {
        _cliente.DocumentoDeIdentidad = documentoDeIdentidad;
        return this;
    }

    public ClienteTestBuilder ConCorreoElectronico(string correoElectronico)
    {
        _cliente.CorreoElectronico = correoElectronico;
        return this;
    }

    public ClienteTestBuilder ConCuentas(IList<Cuenta> cuentas)
    {
        _cliente.Cuentas = cuentas;
        return this;
    }

    public ClienteTestBuilder ConFechaDeNacimiento(DateTime fechaDeNacimiento)
    {
        _cliente.FechaDeNacimiento = fechaDeNacimiento;
        return this;
    }

    public ClienteTestBuilder ConFechaDeCreacion(DateTime fechaDeCreacion)
    {
        _cliente.FechaDeCreacion = fechaDeCreacion;
        return this;
    }

    public ClienteTestBuilder ConFechaUltimaModificacion(DateTime? fechaUltimaModificacion)
    {
        _cliente.FechaUltimaModificacion = fechaUltimaModificacion;
        return this;
    }

    public ClienteTestBuilder ConCuenta(Cuenta cuenta)
    {
        _cliente.AgregarCuenta(cuenta);
        return this;
    }

    public Cliente Build()
    {
        return _cliente;
    }
}