namespace DVSRegister.BusinessLogic.Abstractions;

public interface IUtcClock
{
    DateTime UtcNow { get; }
}