namespace DVSRegister.BusinessLogic.Abstractions;

public sealed class SystemUtcClock : IUtcClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}