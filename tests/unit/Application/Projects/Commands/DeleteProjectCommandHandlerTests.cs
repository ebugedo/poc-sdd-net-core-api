using FluentAssertions;
using MediatR;
using Moq;
using poc_sdd_net_core_api.Application.Common;
using poc_sdd_net_core_api.Application.Projects.Commands.DeleteProject;
using poc_sdd_net_core_api.Domain.Entities;
using poc_sdd_net_core_api.Domain.Interfaces;
using Xunit;

namespace UnitTests.Application.Projects.Commands.DeleteProject;

public class DeleteProjectCommandHandlerTests
{
    private readonly Mock<IProjectRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly DeleteProjectCommandHandler _sut;

    public DeleteProjectCommandHandlerTests()
    {
        _repositoryMock = new Mock<IProjectRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _sut = new DeleteProjectCommandHandler(_repositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldDelete_WhenExists()
    {
        // Arrange
        var project = Project.Create(Guid.NewGuid(), Guid.NewGuid(), "Portal", "Intranet", ".NET 8", DateTime.UtcNow);
        _repositoryMock.Setup(r => r.GetByIdAsync(project.Id)).ReturnsAsync(project);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _sut.Handle(new DeleteProjectCommand(project.Id), CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
        _repositoryMock.Verify(r => r.DeleteAsync(project), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowProjectNotFoundException_WhenNotExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Project?)null);

        // Act
        var act = () => _sut.Handle(new DeleteProjectCommand(id), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ProjectNotFoundException>();
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Project>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
