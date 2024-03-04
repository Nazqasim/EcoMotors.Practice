using EcoMotorsPractice.Shared.Events;

namespace EcoMotorsPractice.Domain.Common.Contracts;

public abstract class DomainEvent : IEvent
{
    public DateTime TriggeredOn { get; protected set; } = DateTime.UtcNow;
}