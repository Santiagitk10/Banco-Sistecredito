using credinet.exception.middleware.models;

using Domain.Model.Entities;
using Domain.Model.Interfaces.Gateways;
using Domain.Model.Interfaces.UseCases.Clientes;

using FluentAssertions;

using Helpers.Commons.Exceptions;
using Helpers.Domain.EntityBuilders;

using Moq;

using Xunit;

namespace Domain.UseCase.Clientes.Tests;

public class CrearClienteUseCaseTests
{
    private readonly Mock<IClienteRepository> _repositorioDeClientesMock;
    private readonly Mock<IClienteEventsRepository> _clienteEventsRepository;
    private readonly ICrearClienteUseCase _useCase;

    public CrearClienteUseCaseTests()
    {
        _repositorioDeClientesMock = new Mock<IClienteRepository>();
        _clienteEventsRepository= new Mock<IClienteEventsRepository>();
        _useCase = new CrearClienteUseCase(_repositorioDeClientesMock.Object, _clienteEventsRepository.Object);
    }

    [Fact(DisplayName = "#CrearCliente debería lanzar una excepción de negocio cuando el cliente es menor de edad.")]
    public void CrearCliente_DeberiaFallar_CuandoElClienteEsMenorDeEdad()
    {
        // Arrange
        DateTime fechaDeNacimiento = new(DateTime.UtcNow.Year - 17, 3, 2);
        Cliente cliente = new ClienteTestBuilder()
            .ConFechaDeNacimiento(fechaDeNacimiento)
            .Build();

        // Act & Assert
        _useCase
            .Invoking(useCase => useCase.CrearCliente(cliente))
            .Should()
            .ThrowAsync<BusinessException>()
            .Where(exception => exception.code == Convert.ToInt32(TipoExcepcionNegocio.ClienteMenorDeEdad));

        _repositorioDeClientesMock
            .Verify(repository => repository.GuardarCliente(cliente), Times.Never);
    }

    [Fact(DisplayName = "#CrearCliente debería guardar el registro de un cliente en el repositorio y retornarlo.")]
    public async Task CrearCliente_DeberiaGuardarElRegistroDelClienteYRetornarlo_CuandoSeaExitoso()
    {
        // Arrange
        Cliente cliente = new ClienteTestBuilder()
            .ConFechaDeNacimiento(new(2000, 1, 1))
            .Build();
        _repositorioDeClientesMock
            .Setup(repository => repository.GuardarCliente(cliente))
            .ReturnsAsync((Cliente cliente) =>
            {
                cliente.Id = Guid.NewGuid().ToString();
                return cliente;
            });

        // Act
        Cliente clienteGuardado = await _useCase.CrearCliente(cliente);

        // Assert
        clienteGuardado.Should().NotBeNull();
        clienteGuardado.Id.Should().NotBeNullOrEmpty();
        _repositorioDeClientesMock
            .Verify(repository => repository.GuardarCliente(cliente), Times.Once);
    }
}