using FluentAssertions;
using poc_sdd_net_core_api.Domain.Entities;
using Xunit;

namespace poc_sdd_net_core_api.Tests.Unit.Domain.Entities;

public class SectorTests
{
    [Fact]
    public void DefaultCatalog_ShouldContainTheSevenDefinedSectors()
    {
        // Act
        var catalog = Sector.DefaultCatalog;

        // Assert
        catalog.Should().HaveCount(7);
        catalog.Select(s => s.Name).Should().BeEquivalentTo(new[]
        {
            "Administración pública",
            "Ingeniería",
            "Publicidad",
            "Servicios financieros",
            "Servicios tecnológicos",
            "Transporte",
            "Sector inmobiliario"
        });
    }

    [Fact]
    public void DefaultCatalog_ShouldUseStableAndUniqueIds()
    {
        // Act
        var catalog = Sector.DefaultCatalog;

        // Assert
        catalog.Select(s => s.Id).Should().OnlyHaveUniqueItems();
        catalog.Should().OnlyContain(s => s.Id != Guid.Empty);
        Sector.DefaultCatalog.Select(s => s.Id)
            .Should().BeEquivalentTo(Sector.DefaultCatalog.Select(s => s.Id));
    }

    [Fact]
    public void DefaultCatalog_ShouldNotBeMutable()
    {
        // Act
        var catalog = Sector.DefaultCatalog;

        // Assert
        catalog.Should().BeAssignableTo<IReadOnlyList<Sector>>();
    }

    [Fact]
    public void Create_ShouldCreateSector_WhenValidData()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var sector = Sector.Create(id, "Transporte");

        // Assert
        sector.Id.Should().Be(id);
        sector.Name.Should().Be("Transporte");
    }

    [Fact]
    public void Create_ShouldThrowException_WhenIdIsEmpty()
    {
        // Arrange & Act
        var act = () => Sector.Create(Guid.Empty, "Transporte");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("id");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_ShouldThrowException_WhenNameIsInvalid(string? invalidName)
    {
        // Arrange & Act
        var act = () => Sector.Create(Guid.NewGuid(), invalidName!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("name");
    }
}
