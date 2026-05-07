using Interrapidisimo.Domain.Entities;

namespace Interrapidisimo.Application.Common.Interfaces;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(Student student);
}
