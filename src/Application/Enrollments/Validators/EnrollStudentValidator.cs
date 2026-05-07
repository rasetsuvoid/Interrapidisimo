using FluentValidation;
using Interrapidisimo.Application.Enrollments.DTOs;

namespace Interrapidisimo.Application.Enrollments.Validators;

public class EnrollStudentValidator : AbstractValidator<EnrollStudentDto>
{
    public EnrollStudentValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("El ID del estudiante es obligatorio.");

        RuleFor(x => x.SubjectId)
            .NotEmpty().WithMessage("El ID de la materia es obligatorio.");
    }
}
