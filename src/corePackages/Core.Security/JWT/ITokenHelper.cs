using Core.Security.Entities;

namespace Core.Security.JWT;

public interface ITokenHelper
{
    AccessToken CreateToken(User user, string sessionId);

    RefreshToken CreateRefreshToken(User user, string ipAddress);
}