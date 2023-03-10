using Domain.Model.Entities;
using Domain.Model.Interfaces.Gateways;
using Domain.Model.Interfaces.UseCases.Clientes;

using Moq;

using Xunit;
using FluentAssertions;
using Helpers.Domain.EntityBuilders;

namespace Domain.UseCase.Clientes.Tests;

public class ObtenerTodosLosClientesTests
{
    private readonly Mock<IClienteRepository> _repositorioDeClientesMock;

    private readonly IObtenerTodosLosClientesUseCase _useCase;

    public ObtenerTodosLosClientesTests()
    {
        _repositorioDeClientesMock = new();
        _useCase = new ObtenerTodosLosClientesUseCase(_repositorioDeClientesMock.Object);
    }

    [Fact(DisplayName = "#ObtenerTodos debería retornar una lista vacía cuando no hay clientes registrados.")]
    public async Task ObtenerTodos_DeberiaRetornarUnaListaVacia_CuandoNoHayClientesRegistrados()
    {
        // Arrange
        _repositorioDeClientesMock
            .Setup(repository => repository.ObtenerTodosLosClientes())
            .ReturnsAsync(new List<Cliente>());

        // Act
        IEnumerable<Cliente> results = await _useCase.ObtenerTodos();

        // Assert
        results.Should().BeEmpty();
        _repositorioDeClientesMock.Verify(mock => mock.ObtenerTodosLosClientes(), Times.Once);
    }

    [Fact(DisplayName = "#ObtenerTodos debería retornar una lista de clientes cuando haya al menos un cliente registrado.")]
    public async Task ObtenerTodos_DeberiaRetornarUnaListaDeClientes_CuandoHayaAlMenosUnClienteRegistrado()
    {
        // Arrange
        List<Cliente> clientes = new List<Cliente>()
        {
            new ClienteTestBuilder()
                .Build()
        };
        _repositorioDeClientesMock
            .Setup(repository => repository.ObtenerTodosLosClientes())
            .ReturnsAsync(clientes);

        // Act
        IEnumerable<Cliente> results = await _useCase.ObtenerTodos();

        // Assert
        results.Should().NotBeEmpty();
        results.Should().HaveCount(clientes.Count);
        _repositorioDeClientesMock.Verify(mock => mock.ObtenerTodosLosClientes(), Times.Once);
    }
}