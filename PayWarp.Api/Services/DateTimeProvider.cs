namespace PayWarp.Api.Services;

public class DateTimeProvider : IDateTimeProvider
{
	public DateTime UtcNow => DateTime.UtcNow;
	public DateOnly UtcToday => DateOnly.FromDateTime(UtcNow);
}