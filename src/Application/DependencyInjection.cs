using FluentValidation;
using Interrapidisimo.Application.Auth.Commands;
using Interrapidisimo.Application.Common.Interfaces;
using Interrapidisimo.Application.Enrollments.Commands;
using Interrapidisimo.Application.Professors.Queries;
using Interrapidisimo.Application.Students.Commands;
using Interrapidisimo.Application.Subjects.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Interrapidisimo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ISubjectService, SubjectService>();
        services.AddScoped<IProfessorService, ProfessorService>();
        services.AddScoped<IEnrollmentService, EnrollmentService>();

        return services;
    }
}
