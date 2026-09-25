using FluentAssertions;
using Moq;
using poc_sdd_net_core_api.Application.Clients.Commands.DeleteClient;
using poc_sdd_net_core_api.Application.Common;
using poc_sdd_net_core_api.Domain.Entities;
using poc_sdd_net_core_api.Domain.Interfaces;
using Xunit;

namespace UnitTests.Application.Clients.Commands;

public class DeleteClientCommandHandlerTests
{
    private readonly Mock<IClientRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly DeleteClientCommandHandler _sut;

    public DeleteClientCommandHandlerTests()
    {
        _repositoryMock = new Mock<IClientRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _sut = new DeleteClientCommandHandler(_repositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldDelete_WhenExists()
    {
        // Arrange
        var client = Client.Create("John Doe", "john@example.com");
        _repositoryMock.Setup(r => r.GetByIdAsync(client.Id)).ReturnsAsync(client);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _sut.Handle(new DeleteClientCommand(client.Id), CancellationToken.None);

        // Assert
        result.Should().Be(MediatR.Unit.Value);
        _repositoryMock.Verify(r => r.DeleteAsync(client), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowClientNotFound_WhenNotExists()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Client?)null);

        // Act
        var act = () => _sut.Handle(new DeleteClientCommand(Guid.NewGuid()), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ClientNotFoundException>();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
