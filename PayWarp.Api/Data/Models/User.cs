namespace PayWarp.Api.Data.Models;

public class User
{
	public required Guid Id { get; set; }
	public required string Name { get; set; }
	public required DateTime CreationDate { get; set; }
}
