namespace blazor_arsip.Services;

public enum ToastType
{
    Success,
    Error,
    Info,
    Warning
}

public class ToastMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public ToastType Type { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public int Duration { get; set; } = 5000; // 5 seconds default
}

public interface IToastService
{
    event Action<ToastMessage>? OnToastAdded;
    event Action<string>? OnToastRemoved;
    
    void ShowSuccess(string message, string title = "Berhasil", int duration = 5000);
    void ShowError(string message, string title = "Error", int duration = 7000);
    void ShowInfo(string message, string title = "Informasi", int duration = 5000);
    void ShowWarning(string message, string title = "Peringatan", int duration = 6000);
    void RemoveToast(string id);
    void ClearAll();
}

public class ToastService : IToastService
{
    public event Action<ToastMessage>? OnToastAdded;
    public event Action<string>? OnToastRemoved;

    public void ShowSuccess(string message, string title = "Berhasil", int duration = 5000)
    {
        Console.WriteLine($"ToastService.ShowSuccess called: {title} - {message}");
        var toast = new ToastMessage
        {
            Title = title,
            Message = message,
            Type = ToastType.Success,
            Duration = duration
        };
        Console.WriteLine($"Invoking OnToastAdded event with {OnToastAdded?.GetInvocationList()?.Length ?? 0} subscribers");
        OnToastAdded?.Invoke(toast);
    }

    public void ShowError(string message, string title = "Error", int duration = 7000)
    {
        var toast = new ToastMessage
        {
            Title = title,
            Message = message,
            Type = ToastType.Error,
            Duration = duration
        };
        OnToastAdded?.Invoke(toast);
    }

    public void ShowInfo(string message, string title = "Informasi", int duration = 5000)
    {
        var toast = new ToastMessage
        {
            Title = title,
            Message = message,
            Type = ToastType.Info,
            Duration = duration
        };
        OnToastAdded?.Invoke(toast);
    }

    public void ShowWarning(string message, string title = "Peringatan", int duration = 6000)
    {
        var toast = new ToastMessage
        {
            Title = title,
            Message = message,
            Type = ToastType.Warning,
            Duration = duration
        };
        OnToastAdded?.Invoke(toast);
    }

    public void RemoveToast(string id)
    {
        OnToastRemoved?.Invoke(id);
    }

    public void ClearAll()
    {
        // This will be handled by the component
    }
}