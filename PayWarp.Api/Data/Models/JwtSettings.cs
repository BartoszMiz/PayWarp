namespace PayWarp.Api.Data.Models;

public record JwtSettings(string Issuer, string Audience, string Key, int ValidMinutes)
{
	public const string SectionName = "JwtSettings";
}
