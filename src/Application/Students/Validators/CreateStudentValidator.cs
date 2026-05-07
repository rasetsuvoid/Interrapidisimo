using FluentValidation;
using Interrapidisimo.Application.Students.DTOs;

namespace Interrapidisimo.Application.Students.Validators;

public class CreateStudentValidator : AbstractValidator<CreateStudentDto>
{
    public CreateStudentValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.")
            .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s'-]+$").WithMessage("El nombre contiene caracteres no válidos.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("El apellido es obligatorio.")
            .MaximumLength(100).WithMessage("El apellido no puede superar los 100 caracteres.")
            .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s'-]+$").WithMessage("El apellido contiene caracteres no válidos.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
            .EmailAddress().WithMessage("Ingresa un correo electrónico válido.")
            .MaximumLength(255).WithMessage("El correo electrónico no puede superar los 255 caracteres.");

        RuleFor(x => x.StudentCode)
            .NotEmpty().WithMessage("El código de estudiante es obligatorio.")
            .MaximumLength(20).WithMessage("El código de estudiante no puede superar los 20 caracteres.")
            .Matches(@"^[A-Za-z0-9\-]+$").WithMessage("El código de estudiante solo puede contener letras, números y guiones.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("La fecha de nacimiento es obligatoria.")
            .LessThan(DateTime.UtcNow.AddYears(-15)).WithMessage("El estudiante debe tener al menos 15 años.")
            .GreaterThan(DateTime.UtcNow.AddYears(-100)).WithMessage("La fecha de nacimiento no es válida.");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("El teléfono no puede superar los 20 caracteres.")
            .Matches(@"^[\+\d\s\-\(\)]*$").WithMessage("El teléfono contiene caracteres no válidos.")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
    }
}

public class UpdateStudentValidator : AbstractValidator<UpdateStudentDto>
{
    public UpdateStudentValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("El apellido es obligatorio.")
            .MaximumLength(100).WithMessage("El apellido no puede superar los 100 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
            .EmailAddress().WithMessage("Ingresa un correo electrónico válido.")
            .MaximumLength(255).WithMessage("El correo electrónico no puede superar los 255 caracteres.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("La fecha de nacimiento es obligatoria.")
            .LessThan(DateTime.UtcNow.AddYears(-15)).WithMessage("El estudiante debe tener al menos 15 años.");
    }
}
