namespace Application.Interfaces
{
    public interface IAuthService
    {
        TokenResponse Authenticate(string username, string password);
        TokenResponse Refresh(string refreshToken);
    }
}
