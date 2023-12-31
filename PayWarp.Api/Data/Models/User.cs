﻿namespace PayWarp.Api.Data.Models;

public class User
{
	public required Guid Id { get; set; }
	public required string Username { get; set; }
	public required byte[] PasswordHash { get; set; }
	public required byte[] Salt { get; set; }
	public required DateOnly CreationDate { get; set; }
}
