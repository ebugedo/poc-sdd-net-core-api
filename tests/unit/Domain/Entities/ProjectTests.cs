using FluentAssertions;
using poc_sdd_net_core_api.Domain.Entities;
using Xunit;

namespace poc_sdd_net_core_api.Tests.Unit.Domain.Entities;

public class ProjectTests
{
    private static readonly Guid ClientId = Guid.NewGuid();

    [Fact]
    public void Create_ShouldCreateProject_WhenValidData()
    {
        // Arrange
        var startDate = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc);

        // Act
        var project = Project.Create(ClientId, "Portal", "Intranet", ".NET 8, PostgreSQL", startDate, 6);

        // Assert
        project.Should().NotBeNull();
        project.Id.Should().NotBeEmpty();
        project.ClientId.Should().Be(ClientId);
        project.Title.Should().Be("Portal");
        project.Description.Should().Be("Intranet");
        project.Technologies.Should().Be(".NET 8, PostgreSQL");
        project.StartDate.Should().Be(startDate);
        project.DurationMonths.Should().Be(6);
        project.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_ShouldCreateProject_WhenDurationIsNull()
    {
        // Act
        var project = Project.Create(ClientId, "Portal", "Intranet", ".NET 8", DateTime.UtcNow);

        // Assert
        project.DurationMonths.Should().BeNull();
    }

    [Fact]
    public void Create_ShouldThrowException_WhenClientIdIsEmpty()
    {
        // Arrange & Act
        var act = () => Project.Create(Guid.Empty, "Portal", "Intranet", ".NET 8", DateTime.UtcNow);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("clientId");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_ShouldThrowException_WhenTitleIsInvalid(string? invalidTitle)
    {
        // Arrange & Act
        var act = () => Project.Create(ClientId, invalidTitle!, "Intranet", ".NET 8", DateTime.UtcNow);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("title");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_ShouldThrowException_WhenDescriptionIsInvalid(string? invalidDescription)
    {
        // Arrange & Act
        var act = () => Project.Create(ClientId, "Portal", invalidDescription!, ".NET 8", DateTime.UtcNow);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("description");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_ShouldThrowException_WhenTechnologiesIsInvalid(string? invalidTechnologies)
    {
        // Arrange & Act
        var act = () => Project.Create(ClientId, "Portal", "Intranet", invalidTechnologies!, DateTime.UtcNow);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("technologies");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_ShouldThrowException_WhenDurationIsNotPositive(int invalidDuration)
    {
        // Arrange & Act
        var act = () => Project.Create(ClientId, "Portal", "Intranet", ".NET 8", DateTime.UtcNow, invalidDuration);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("durationMonths");
    }

    [Fact]
    public void Update_ShouldUpdateProject_WhenValidData()
    {
        // Arrange
        var project = Project.Create(ClientId, "Portal", "Intranet", ".NET 8", DateTime.UtcNow, 6);
        var newClientId = Guid.NewGuid();
        var newStartDate = new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act
        project.Update(newClientId, "Nuevo", "Nueva descripcion", "React", newStartDate, 12);

        // Assert
        project.ClientId.Should().Be(newClientId);
        project.Title.Should().Be("Nuevo");
        project.Description.Should().Be("Nueva descripcion");
        project.Technologies.Should().Be("React");
        project.StartDate.Should().Be(newStartDate);
        project.DurationMonths.Should().Be(12);
    }

    [Fact]
    public void Update_ShouldClearDuration_WhenDurationIsNull()
    {
        // Arrange
        var project = Project.Create(ClientId, "Portal", "Intranet", ".NET 8", DateTime.UtcNow, 6);

        // Act
        project.Update(ClientId, "Portal", "Intranet", ".NET 8", DateTime.UtcNow, null);

        // Assert
        project.DurationMonths.Should().BeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void Update_ShouldThrowException_WhenDurationIsNotPositive(int invalidDuration)
    {
        // Arrange
        var project = Project.Create(ClientId, "Portal", "Intranet", ".NET 8", DateTime.UtcNow, 6);

        // Act
        var act = () => project.Update(ClientId, "Portal", "Intranet", ".NET 8", DateTime.UtcNow, invalidDuration);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("durationMonths");
        project.DurationMonths.Should().Be(6);
    }

    [Fact]
    public void Update_ShouldThrowException_WhenTitleIsInvalid()
    {
        // Arrange
        var project = Project.Create(ClientId, "Portal", "Intranet", ".NET 8", DateTime.UtcNow);

        // Act
        var act = () => project.Update(ClientId, "  ", "Intranet", ".NET 8", DateTime.UtcNow);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("title");
    }
}
