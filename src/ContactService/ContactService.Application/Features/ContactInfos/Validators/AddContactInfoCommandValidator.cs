using ContactService.Application.Features.ContactInfos.Commands.AddContactInfo;
using ContactService.Domain.Enums;
using FluentValidation;

namespace ContactService.Application.Features.ContactInfos.Validators
{
    public class AddContactInfoCommandValidator : AbstractValidator<AddContactInfoCommand>
    {
        public AddContactInfoCommandValidator()
        {
            RuleFor(v => v.PersonId)
                .NotEmpty().WithMessage("Kişi ID boş olamaz.");

            RuleFor(v => v.BilgiTipi)
                .IsInEnum().WithMessage("Geçerli bir bilgi tipi seçilmelidir.");

            RuleFor(v => v.BilgiIcerigi)
                .NotEmpty().WithMessage("Bilgi içeriği boş olamaz.")
                .MaximumLength(200).WithMessage("Bilgi içeriği en fazla 200 karakter olabilir.");

     
            RuleFor(v => v.BilgiIcerigi)
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi girilmelidir.")
                .When(v => v.BilgiTipi == ContactType.EmailAdresi);

           
            RuleFor(v => v.BilgiIcerigi)
                .Matches(@"^[0-9\s\-\+\(\)]+$").WithMessage("Geçerli bir telefon numarası formatı girilmelidir.")
                .MinimumLength(7).WithMessage("Telefon numarası çok kısa.")
                .When(v => v.BilgiTipi == ContactType.TelefonNumarasi);
        }
    }
}