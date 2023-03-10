using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using credinet.exception.middleware.models;

using Domain.Model.Entities;
using Domain.Model.Interfaces.Gateways;
using Domain.Model.Interfaces.UseCases.Clientes;

using Helpers.Commons.Exceptions;
using Helpers.ObjectsUtils.Extensions;

namespace Domain.UseCase.Clientes;

public class CrearClienteUseCase : ICrearClienteUseCase
{
    private readonly IClienteRepository _repositorioDeClientes;
    private readonly IClienteEventsRepository _clienteEventsRepository;

    public CrearClienteUseCase(IClienteRepository repositorioDeClientes, IClienteEventsRepository clienteEventsRepository)
    {
        _repositorioDeClientes = repositorioDeClientes;
        _clienteEventsRepository = clienteEventsRepository;
    }

    public async Task<Cliente> CrearCliente(Cliente cliente)
    {
        if (cliente.FechaDeNacimiento is not null)
        {
            double edad = (DateTime.UtcNow - (DateTime)cliente.FechaDeNacimiento).TotalDays / 365.242199;
            if (edad < 18f)
                throw new BusinessException(TipoExcepcionNegocio.ClienteMenorDeEdad.GetDescription(), Convert.ToInt32(TipoExcepcionNegocio.ClienteMenorDeEdad));
        }
        cliente.Cuentas = new List<Cuenta>();
        var clienteretorno = await CrearClienteSafe(cliente);
        await _clienteEventsRepository.InformarClienteCreadoEmail(clienteretorno);

        return clienteretorno;
    }

    private Task<Cliente> CrearClienteSafe(Cliente cliente)
    {
        return _repositorioDeClientes.GuardarCliente(cliente);
    }
}