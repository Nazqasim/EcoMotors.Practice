using EcoMotorsPractice.Shared.Events;

namespace EcoMotorsPractice.Application.Common.Events;

public interface IEventPublisher : ITransientService
{
    Task PublishAsync(IEvent @event);
}