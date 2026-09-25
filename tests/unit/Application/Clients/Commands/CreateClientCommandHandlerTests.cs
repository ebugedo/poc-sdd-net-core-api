using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;
using poc_sdd_net_core_api.Application.Clients.Commands.CreateClient;
using poc_sdd_net_core_api.Application.Mappings;
using poc_sdd_net_core_api.Domain.Entities;
using poc_sdd_net_core_api.Domain.Interfaces;
using Xunit;

namespace UnitTests.Application.Clients.Commands.CreateClient;

public class CreateClientCommandHandlerTests
{
    private readonly Mock<IClientRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IMapper _mapper;
    private readonly CreateClientCommandHandler _sut;
    private readonly Faker _faker;

    public CreateClientCommandHandlerTests()
    {
        _repositoryMock = new Mock<IClientRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<ClientMappingProfile>()).CreateMapper();
        _sut = new CreateClientCommandHandler(_repositoryMock.Object, _unitOfWorkMock.Object, _mapper);

        _faker = new Faker();
    }

    private CreateClientCommand GenerateCommand() =>
        new(_faker.Name.FullName(), _faker.Internet.Email(), _faker.Phone.PhoneNumber());

    [Fact]
    public async Task Handle_ShouldCreateAndReturnClient()
    {
        // Arrange
        var command = GenerateCommand();
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Client>())).ReturnsAsync((Client c) => c);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(command.Name);
        result.Email.Should().Be(command.Email);
        result.Id.Should().NotBeEmpty();
        _repositoryMock.Verify(r => r.AddAsync(It.Is<Client>(c => c.Name == command.Name)), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowArgumentException_WhenNameIsEmpty()
    {
        // Arrange
        var command = GenerateCommand() with { Name = "  " };

        // Act
        var act = () => _sut.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Client>()), Times.Never);
    }
}
