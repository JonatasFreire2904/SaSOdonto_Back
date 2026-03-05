using Api.Services;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.Auth;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly TokenService _tokenService;

        public AuthController(AppDbContext db, TokenService tokenService)
        {
            _db = db;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Registrar um novo usuário
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var emailExists = await _db.Users.AnyAsync(u => u.Email == request.Email);
            if (emailExists)
                return Conflict("Já existe um usuário com este email.");

            var user = new User
            {
                UserName = request.UserName,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = UserRole.User
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new { Message = "Usuário registrado com sucesso!", user.Id, user.UserName, user.Email });
        }

        /// <summary>
        /// Login com email e senha
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null)
                return BadRequest("Dados de login não informados.");

            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Email e senha são obrigatórios.");

            var user = await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized("Email ou senha inválidos.");

            var token = _tokenService.GenerateToken(user);

            return Ok(new LoginResponse
            {
                Token = token,
                UserName = user.UserName,
                Role = user.Role.ToString()
            });
        }

        /// <summary>
        /// Endpoint protegido para testar o token JWT
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            return Ok(new
            {
                Message = "Você está autenticado!",
                User = User.Identity?.Name,
                Claims = User.Claims.Select(c => new { c.Type, c.Value })
            });
        }
    }
}