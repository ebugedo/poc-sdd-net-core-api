using AutoMapper;
using FluentAssertions;
using Moq;
using poc_sdd_net_core_api.Application.Clients.Queries.GetAllClients;
using poc_sdd_net_core_api.Application.Mappings;
using poc_sdd_net_core_api.Domain.Entities;
using poc_sdd_net_core_api.Domain.Interfaces;
using Xunit;

namespace UnitTests.Application.Clients.Queries;

public class GetAllClientsQueryHandlerTests
{
    private readonly Mock<IClientRepository> _repositoryMock;
    private readonly GetAllClientsQueryHandler _sut;

    public GetAllClientsQueryHandlerTests()
    {
        _repositoryMock = new Mock<IClientRepository>();
        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<ClientMappingProfile>()).CreateMapper();
        _sut = new GetAllClientsQueryHandler(_repositoryMock.Object, mapper);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllClients()
    {
        // Arrange
        var clients = new List<Client>
        {
            Client.Create("John Doe", "john@example.com", "+123"),
            Client.Create("Jane Doe", "jane@example.com")
        };
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(clients);

        // Act
        var result = await _sut.Handle(new GetAllClientsQuery(), CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result[0].Name.Should().Be("John Doe");
        _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoClients()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Client>());

        // Act
        var result = await _sut.Handle(new GetAllClientsQuery(), CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}
