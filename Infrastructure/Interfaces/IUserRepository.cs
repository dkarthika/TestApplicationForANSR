using Infrastructure.Data;

public interface IUserRepository
{
    User GetByUsername(string username);
    User GetByRefreshToken(string refreshToken);
    void Update(User user);
}
