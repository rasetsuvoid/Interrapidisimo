using Interrapidisimo.Application.Auth.DTOs;
using Interrapidisimo.Application.Common.Models;

namespace Interrapidisimo.Application.Common.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponseDto>> RegisterStudentAsync(RegisterStudentDto dto, CancellationToken cancellationToken = default);
    Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default);
}
