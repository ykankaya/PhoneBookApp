using ContactService.Application.Features.Persons.Commands.CreatePerson;
using ContactService.Application.Features.Persons.Validators;
using FluentAssertions;
using Xunit;

namespace ContactService.Application.Tests.Features.Persons.Validators
{
    public class CreatePersonCommandValidatorTests
    {
        private readonly CreatePersonCommandValidator _validator = new CreatePersonCommandValidator();

        [Fact]
        public void Should_Have_Error_When_Ad_Is_Empty()
        {
            var command = new CreatePersonCommand { Ad = "", Soyad = "Test", Firma = "Test" };
            var result = _validator.Validate(command);
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Ad) && e.ErrorMessage.Contains("boş olamaz"));
        }

        [Fact]
        public void Should_Have_Error_When_Soyad_Is_Empty()
        {
            var command = new CreatePersonCommand { Ad = "Test", Soyad = "", Firma = "Test" };
            var result = _validator.Validate(command);
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Soyad));
        }

        [Fact]
        public void Should_Have_Error_When_Ad_Is_Too_Long()
        {
            var command = new CreatePersonCommand { Ad = new string('a', 101), Soyad = "Test", Firma = "Test" };
            var result = _validator.Validate(command);
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Ad) && e.ErrorMessage.Contains("100 karakter"));
        }

        [Fact]
        public void Should_Not_Have_Error_When_Command_Is_Valid()
        {
            var command = new CreatePersonCommand { Ad = "Valid Name", Soyad = "Valid Surname", Firma = "Valid Company" };
            var result = _validator.Validate(command);
            result.IsValid.Should().BeTrue();
        }
    }
}