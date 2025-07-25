using FluentValidation.Results;

namespace Soda.Application.ApplicationServices.NotificationService;

public class NotificationServiceContext 
{
    private readonly List<Notification> _notifications = new();
    public IReadOnlyCollection<Notification> Notifications => _notifications;
    public bool HasNotifications => _notifications.Any();

    public void AddNotification(string key, string message)
    {
        _notifications.Add(new Notification(key, message));
    }
    public void AddNotification(string message)
    {
        _notifications.Add(new Notification(Guid.NewGuid().ToString(), message));
    }
    public void AddNotifications(ValidationResult validationResult)
    {
        foreach (var error in validationResult.Errors)
        {
            AddNotification(error.ErrorCode, error.ErrorMessage);
        }
    }
}