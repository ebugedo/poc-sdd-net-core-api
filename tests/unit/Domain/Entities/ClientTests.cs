using FluentAssertions;
using Xunit;
using poc_sdd_net_core_api.Domain.Entities;

namespace poc_sdd_net_core_api.Tests.Unit.Domain.Entities;

public class ClientTests
{
    [Fact]
    public void Create_ShouldCreateClient_WhenValidData()
    {
        // Arrange
        var name = "John Doe";
        var email = "john@example.com";
        var phone = "+1234567890";

        // Act
        var client = Client.Create(name, email, phone);

        // Assert
        client.Should().NotBeNull();
        client.Id.Should().NotBeEmpty();
        client.Name.Should().Be(name);
        client.Email.Should().Be(email);
        client.Phone.Should().Be(phone);
        client.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_ShouldCreateClient_WhenPhoneIsNull()
    {
        // Arrange
        var name = "John Doe";
        var email = "john@example.com";

        // Act
        var client = Client.Create(name, email);

        // Assert
        client.Phone.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_ShouldThrowException_WhenNameIsInvalid(string? invalidName)
    {
        // Arrange & Act
        var act = () => Client.Create(invalidName!, "john@example.com");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("name");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_ShouldThrowException_WhenEmailIsInvalid(string? invalidEmail)
    {
        // Arrange & Act
        var act = () => Client.Create("John Doe", invalidEmail!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("email");
    }

    [Fact]
    public void Update_ShouldUpdateClient_WhenValidData()
    {
        // Arrange
        var client = Client.Create("John Doe", "john@example.com");
        var newName = "Jane Doe";
        var newEmail = "jane@example.com";
        var newPhone = "+0987654321";

        // Act
        client.Update(newName, newEmail, newPhone);

        // Assert
        client.Name.Should().Be(newName);
        client.Email.Should().Be(newEmail);
        client.Phone.Should().Be(newPhone);
    }

    [Fact]
    public void Update_ShouldThrowException_WhenNameIsInvalid()
    {
        // Arrange
        var client = Client.Create("John Doe", "john@example.com");

        // Act
        var act = () => client.Update("", "jane@example.com");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("name");
    }

    [Fact]
    public void Update_ShouldThrowException_WhenEmailIsInvalid()
    {
        // Arrange
        var client = Client.Create("John Doe", "john@example.com");

        // Act
        var act = () => client.Update("Jane Doe", "");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("email");
    }
}
