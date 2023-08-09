using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PayWarp.Api.Data;
using PayWarp.Api.Data.Models;
using PayWarp.Api.Services;

namespace PayWarp.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class UsersController : ControllerBase
{
	private readonly AppDbContext _db;
	private readonly IDateTimeProvider _dateTime;
	private readonly JwtSettings _jwtSettings;

	public UsersController(AppDbContext db, IDateTimeProvider dateTime, JwtSettings jwtSettings)
	{
		_db = db;
		_dateTime = dateTime;
		_jwtSettings = jwtSettings;
	}

	[HttpPost]
	public IActionResult Register(UserRegistrationRequest request)
	{
		var searchedUsername = request.Username.ToLower();
		var searchedUser = _db.Users.FirstOrDefault(x => x.Username == searchedUsername);
		if (searchedUser is not null)
		{
			return Conflict("User with this username already exists!");
		}

		using var hmac = new HMACSHA512();
		var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password));
		var user = new User
		{
			Id = Guid.NewGuid(),
			Username = searchedUsername,
			PasswordHash = passwordHash,
			Salt = hmac.Key,
			CreationDate = _dateTime.UtcToday
		};

		_db.Users.Add(user);
		_db.SaveChanges();

		return Ok(user);
	}

	[HttpPost]
	public IActionResult Login(LoginRequest request)
	{
		var searchedUsername = request.Username.ToLower();
		var searchedUser = _db.Users.FirstOrDefault(x => x.Username == searchedUsername);
		if (searchedUser is null)
		{
			return Unauthorized();
		}

		using var hmac = new HMACSHA512(searchedUser.Salt);
		var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password));
		if (!searchedUser.PasswordHash.SequenceEqual(passwordHash))
		{
			return Unauthorized();
		}

		var jwt = CreateJwt(searchedUser);
		return Ok(jwt);
	}

	private string CreateJwt(User user)
	{
		var claims = new Claim[]
		{
			new("name", user.Username),
		};

		var expiration = _dateTime.UtcNow.AddMinutes(_jwtSettings.ValidMinutes);
		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
		var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
		var jwt = new JwtSecurityToken(
			issuer: _jwtSettings.Issuer,
			audience: _jwtSettings.Audience,
			claims: claims,
			notBefore: _dateTime.UtcNow,
			expires: expiration,
			signingCredentials: credentials);

		var jwtDigest = new JwtSecurityTokenHandler().WriteToken(jwt);
		return jwtDigest;
	}
}
