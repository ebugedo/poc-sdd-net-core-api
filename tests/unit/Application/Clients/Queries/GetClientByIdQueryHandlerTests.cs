using AutoMapper;
using FluentAssertions;
using Moq;
using poc_sdd_net_core_api.Application.Clients.Queries.GetClientById;
using poc_sdd_net_core_api.Application.Common;
using poc_sdd_net_core_api.Application.Mappings;
using poc_sdd_net_core_api.Domain.Entities;
using poc_sdd_net_core_api.Domain.Interfaces;
using Xunit;

namespace UnitTests.Application.Clients.Queries;

public class GetClientByIdQueryHandlerTests
{
    private readonly Mock<IClientRepository> _repositoryMock;
    private readonly GetClientByIdQueryHandler _sut;

    public GetClientByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<IClientRepository>();
        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<ClientMappingProfile>()).CreateMapper();
        _sut = new GetClientByIdQueryHandler(_repositoryMock.Object, mapper);
    }

    [Fact]
    public async Task Handle_ShouldReturnClient_WhenExists()
    {
        // Arrange
        var client = Client.Create("John Doe", "john@example.com");
        _repositoryMock.Setup(r => r.GetByIdAsync(client.Id)).ReturnsAsync(client);

        // Act
        var result = await _sut.Handle(new GetClientByIdQuery(client.Id), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(client.Id);
        result.Name.Should().Be("John Doe");
        result.Email.Should().Be("john@example.com");
    }

    [Fact]
    public async Task Handle_ShouldThrowClientNotFound_WhenNotExists()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Client?)null);

        // Act
        var act = () => _sut.Handle(new GetClientByIdQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ClientNotFoundException>();
    }
}
