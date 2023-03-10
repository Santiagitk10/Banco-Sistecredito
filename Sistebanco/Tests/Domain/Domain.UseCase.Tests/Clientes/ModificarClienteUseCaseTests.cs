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

public class ModificarClienteUseCaseTests
{
    private readonly Mock<IClienteRepository> _repositoryDeClientesMock;

    private readonly IModificarClienteUseCase _useCase;

    public ModificarClienteUseCaseTests()
    {
        _repositoryDeClientesMock = new();
        _useCase = new ModificarClienteUseCase(_repositoryDeClientesMock.Object);
    }

    [Fact(DisplayName = "#ModificarCliente debería lanzar una excepción de negocio cuando el cliente con el documento de identidad ingresado no existe.")]
    public void ModificarCliente_DeberiaFallar_CuandoElClienteNoExiste()
    {
        // Arrange
        string documentoDeIdentidad = Guid.NewGuid().ToString();
        Cliente cliente = new ClienteTestBuilder()
            .ConDocumentoDeIdentidad(documentoDeIdentidad)
            .Build();
        _repositoryDeClientesMock
            .Setup(repository => repository.ClienteExisteConDocumentoDeIdentidad(documentoDeIdentidad))
            .ReturnsAsync(false);

        // Act & Assert
        _useCase
            .Invoking(useCase => useCase.ModificarCliente(documentoDeIdentidad, cliente))
            .Should()
            .ThrowAsync<BusinessException>()
            .Where(exception => exception.code == Convert.ToInt32(TipoExcepcionNegocio.ClienteConDocumentoDeIdentidadNoExiste));

        _repositoryDeClientesMock
            .Verify(repository => repository.ClienteExisteConDocumentoDeIdentidad(documentoDeIdentidad), Times.Once);
    }

    [Fact(DisplayName = "#ModificarCliente debería actualizar los datos del cliente con el documento de identidad ingresado.")]
    public async void ModificarCliente_DeberiaModificarAlCliente_CuandoSeaExitoso()
    {
        // Arrange
        string documentoDeIdentidad = Guid.NewGuid().ToString();
        Cliente cliente = new ClienteTestBuilder()
            .ConDocumentoDeIdentidad(documentoDeIdentidad)
            .Build();
        Cliente modificaciones = new ClienteTestBuilder()
            .ConCorreoElectronico("flaskdjñafkl2423@gmail.com")
            .Build();
        _repositoryDeClientesMock
            .Setup(repository => repository.ClienteExisteConDocumentoDeIdentidad(documentoDeIdentidad))
            .ReturnsAsync(true);
        _repositoryDeClientesMock
            .Setup(repository => repository.ObtenerClientePorDocumento(documentoDeIdentidad))
            .ReturnsAsync(cliente);
        _repositoryDeClientesMock
            .Setup(repository => repository.ActualizarDatos(documentoDeIdentidad, modificaciones))
            .ReturnsAsync((string _, Cliente cambios) => new()
            {
                DocumentoDeIdentidad = cliente.DocumentoDeIdentidad,
                CorreoElectronico = cambios.CorreoElectronico
            });

        // Act
        Cliente resultado = await _useCase.ModificarCliente(documentoDeIdentidad, modificaciones);

        // Assert
        resultado.Should().NotBeNull();
        resultado.CorreoElectronico.Should().Be(modificaciones.CorreoElectronico);
        _repositoryDeClientesMock
            .Verify(repository => repository.ActualizarDatos(documentoDeIdentidad, modificaciones), Times.Once);
    }
}