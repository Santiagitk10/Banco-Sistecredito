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

public class ObtenerPorDocumentoDeIdentidadUseCaseTests
{
    private readonly Mock<IClienteRepository> _repositorioDeClientesMock;

    private readonly IObtenerPorDocumentoDeIdentidadUseCase _useCase;

    public ObtenerPorDocumentoDeIdentidadUseCaseTests()
    {
        _repositorioDeClientesMock = new Mock<IClienteRepository>();
        _useCase = new ObtenerPorDocumentoDeIdentidadUseCase(_repositorioDeClientesMock.Object);
    }

    [Fact(DisplayName = "#ObtenerPorDocumentoDeIdentidad debería lanzar una excepción de negocio cuando el cliente no existe.")]
    public void ObtenerPorDocumentoDeIdentidad_DeberiaFallar_CuandoElClienteNoExiste()
    {
        // Arrange
        string documentoDeIdentidad = "";
        _repositorioDeClientesMock
            .Setup(mock => mock.ObtenerClientePorDocumento(documentoDeIdentidad))
            .ReturnsAsync(() => null);

        // Act & Assert
        _useCase
            .Invoking(useCase => useCase.ObtenerPorDocumentoDeIdentidad(documentoDeIdentidad))
            .Should()
            .ThrowAsync<BusinessException>()
            .Where(exception => exception.code == Convert.ToInt32(TipoExcepcionNegocio.ClienteConDocumentoDeIdentidadNoExiste));
        _repositorioDeClientesMock
            .Verify(mock => mock.ObtenerClientePorDocumento(documentoDeIdentidad), Times.Once);
    }

    [Fact(DisplayName = "#ObtenerPorDocumentoDeIdentidad debería retornar el registro del cliente con el documento de identidad dado.")]
    public async Task ObtenerPorDocumentoDeIdentidad_DeberiaRetornarUnCliente_CuandoEsteExista()
    {
        // Arrange
        Cliente cliente = new ClienteTestBuilder()
            .ConDocumentoDeIdentidad(Guid.NewGuid().ToString())
            .Build();
        _repositorioDeClientesMock
            .Setup(repository => repository.ObtenerClientePorDocumento(cliente.DocumentoDeIdentidad))
            .ReturnsAsync(() => cliente);

        // Act
        Cliente clienteObtenido = await _useCase.ObtenerPorDocumentoDeIdentidad(cliente.DocumentoDeIdentidad);

        // Assert
        clienteObtenido.Should().NotBeNull();
        clienteObtenido.DocumentoDeIdentidad.Should().Be(cliente.DocumentoDeIdentidad);

        _repositorioDeClientesMock
            .Verify(repository => repository.ObtenerClientePorDocumento(cliente.DocumentoDeIdentidad), Times.Once);
    }
}