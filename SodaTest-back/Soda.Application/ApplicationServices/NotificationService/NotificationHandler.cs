
using Microsoft.Extensions.Logging;

namespace Soda.Application.ApplicationServices.NotificationService;

public static class NotificationHandler
{
    public static void ThrowIfAny(NotificationServiceContext context)
    {
        if (context.HasNotifications)
        {
            var message = string.Join(" | ", context.Notifications.Select(n => $"{n.Key}: {n.Message}"));
            throw new ApplicationException($"Notifications found: {message}");
        }
    }

    public static void LogIfAny(NotificationServiceContext context, ILogger logger)
    {
        if (context.HasNotifications)
        {
            foreach (var notification in context.Notifications)
            {
                logger.LogInformation("Notification: {Key} - {Message}", notification.Key, notification.Message);
            }
        }
    }

    public static bool ShouldStopWorker(NotificationServiceContext context, ILogger logger)
    {
        if (context.HasNotifications)
        {
            logger.LogError("Worker stopped due to notifications.");
            LogIfAny(context, logger);
            return true;
        }

        return false;
    }
}