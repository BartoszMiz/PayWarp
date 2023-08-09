namespace PayWarp.Api.Services;

public interface IDateTimeProvider
{
	public DateTime UtcNow { get; }
	public DateOnly UtcToday { get; }
}
