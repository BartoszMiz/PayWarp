using Microsoft.EntityFrameworkCore;
using PayWarp.Api.Data.Models;

namespace PayWarp.Api.Data;

public class AppDbContext : DbContext
{
	public DbSet<User> Users { get; set; } = null!;

	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
	}
}
