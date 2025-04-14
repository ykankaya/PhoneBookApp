using ContactService.Application.Features.Persons.Commands.CreatePerson;
using ContactService.Application.Interfaces.Persistence;
using ContactService.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;



namespace ContactService.Application.Tests.Features.Persons.Commands
{
    public class CreatePersonCommandHandlerTests
    {
        private readonly Mock<IContactDbContext> _mockContext;
        private readonly Mock<DbSet<Person>> _mockPersonDbSet; 
        private readonly CreatePersonCommandHandler _handler;

        public CreatePersonCommandHandlerTests()
        {
            _mockContext = new Mock<IContactDbContext>();
            _mockPersonDbSet = new Mock<DbSet<Person>>();

          
            _mockContext.Setup(c => c.Persons).Returns(_mockPersonDbSet.Object);

           
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(1);

            _handler = new CreatePersonCommandHandler(_mockContext.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_ShouldAddPersonAndReturnId()
        {
            // Arrange
            var command = new CreatePersonCommand
            {
                Ad = "Test",
                Soyad = "User",
                Firma = "Test Corp"
            };
            var cancellationToken = new CancellationToken();
            Person? addedPerson = null;

        
             _mockPersonDbSet.Setup(d => d.AddAsync(It.IsAny<Person>(), It.IsAny<CancellationToken>()))
                             .Callback<Person, CancellationToken>((p, ct) => addedPerson = p)
                             .ReturnsAsync((Person p, CancellationToken ct) => null!); 

            // Act
            var result = await _handler.Handle(command, cancellationToken);

            // Assert
            result.Should().NotBeEmpty(); 
            addedPerson.Should().NotBeNull();
            addedPerson?.Ad.Should().Be(command.Ad);
            addedPerson?.Soyad.Should().Be(command.Soyad);
            addedPerson?.Firma.Should().Be(command.Firma);
            addedPerson?.Id.Should().Be(result); 

         
            _mockPersonDbSet.Verify(d => d.AddAsync(It.IsAny<Person>(), cancellationToken), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(cancellationToken), Times.Once);
        }
    }
}