using ContactService.Application.Features.Persons.Commands.CreatePerson;
using FluentValidation;

namespace ContactService.Application.Features.Persons.Validators
{
  public class CreatePersonCommandValidator : AbstractValidator<CreatePersonCommand>
  {
    public CreatePersonCommandValidator()
    {
      RuleFor(v => v.Ad)
        .NotEmpty().WithMessage("Ad boş olamaz.")
        .MaximumLength(100).WithMessage("Ad en fazla 100 karakter olabilir.");

      RuleFor(v => v.Soyad)
        .NotEmpty().WithMessage("Soyad boş olamaz.")
        .MaximumLength(100).WithMessage("Soyad en fazla 100 karakter olabilir.");

      RuleFor(v => v.Firma)
        .MaximumLength(150).WithMessage("Firma en fazla 150 karakter olabilir.");
    }
  }
}
