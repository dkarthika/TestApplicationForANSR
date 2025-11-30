using Infrastructure.Data;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public User? GetByUsername(string username)
    {
        var user = _context.Users.SingleOrDefault(u => u.Username == username);
        return user == null ? null : new User
        {
            Id = user.Id,
            Username = user.Username,
            PasswordHash = user.PasswordHash,
            RefreshToken = user.RefreshToken,
            RefreshTokenExpiryTime = user.RefreshTokenExpiryTime
        };          
    }

    public User? GetByRefreshToken(string refreshToken)
    {
        var user = _context.Users.SingleOrDefault(u => u.RefreshToken == refreshToken && u.RefreshTokenExpiryTime > DateTime.UtcNow);
        return user == null ? null : new User
        {
            Id = user.Id,
            Username = user.Username,
            PasswordHash = user.PasswordHash,
            RefreshToken = user.RefreshToken,
            RefreshTokenExpiryTime = user.RefreshTokenExpiryTime
        };
    }

    public void Update(User user)
    {
        var existingUser = _context.Users.SingleOrDefault(u => u.Id == user.Id);
        if (existingUser != null)
        {
            existingUser.Username = user.Username;
            existingUser.PasswordHash = user.PasswordHash;
            existingUser.RefreshToken = user.RefreshToken;
            existingUser.RefreshTokenExpiryTime = user.RefreshTokenExpiryTime;

            _context.Users.Update(existingUser);
            _context.SaveChanges();
        }
    }
}
