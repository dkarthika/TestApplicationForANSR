using Microsoft.AspNetCore.Mvc;
using Application.Dto;
using Application.Interfaces;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public ActionResult<TokenResponse> Login([FromBody] LoginRequest request)
    {
        var result = _authService.Authenticate(request.Username, request.Password);
        if (result == null) return Unauthorized();
        return Ok(result);
    }

    [HttpPost("refresh")]
    public ActionResult<TokenResponse> Refresh([FromBody] string refreshToken)
    {
        var result = _authService.Refresh(refreshToken);
        if (result == null) return Unauthorized();
        return Ok(result);
    }
}
