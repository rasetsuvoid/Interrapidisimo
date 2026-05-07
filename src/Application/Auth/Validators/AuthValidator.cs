using FluentValidation;
using Interrapidisimo.Application.Auth.DTOs;

namespace Interrapidisimo.Application.Auth.Validators;

public class RegisterStudentValidator : AbstractValidator<RegisterStudentDto>
{
    public RegisterStudentValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar 100 caracteres.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("El apellido es obligatorio.")
            .MaximumLength(100).WithMessage("El apellido no puede superar 100 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
            .EmailAddress().WithMessage("El correo electrónico no es válido.")
            .MaximumLength(255).WithMessage("El correo electrónico no puede superar 255 caracteres.");

        RuleFor(x => x.StudentCode)
            .NotEmpty().WithMessage("El código del estudiante es obligatorio.")
            .MaximumLength(20).WithMessage("El código del estudiante no puede superar 20 caracteres.")
            .Matches(@"^[A-Za-z0-9\-]+$").WithMessage("El código solo puede contener letras, números y guiones.");

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.UtcNow.AddYears(-15)).WithMessage("El estudiante debe tener al menos 15 años.");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("El teléfono no puede superar 20 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
            .Matches(@"[A-Z]").WithMessage("La contraseña debe incluir al menos una mayúscula.")
            .Matches(@"[a-z]").WithMessage("La contraseña debe incluir al menos una minúscula.")
            .Matches(@"\d").WithMessage("La contraseña debe incluir al menos un número.");
    }
}

public class LoginValidator : AbstractValidator<LoginDto>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
            .EmailAddress().WithMessage("El correo electrónico no es válido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.");
    }
}
