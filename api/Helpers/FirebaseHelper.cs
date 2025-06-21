using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public static class FirebaseHelper
{
    private static bool _initialized = false;
    private static readonly object _lock = new object();
    private static readonly HttpClient _httpClient = new HttpClient();


    private static void InitializeFirebase()
    {
        if (!_initialized)
        {
            lock (_lock)
            {
                if (!_initialized)
                {
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile("firebase-adminsdk.json")
                            .CreateScoped("https://www.googleapis.com/auth/firebase.messaging")
                    });
                    _initialized = true;
                }
            }
        }
    }

    
    public static async Task SendTaskNotificationAsync(string deviceToken, int pendingCount, int inProgressCount)
    {
        InitializeFirebase();

        var (title, body) = BuildTaskNotification(pendingCount, inProgressCount);

        var message = new Message()
        {
            Token = deviceToken,
            Notification = new Notification
            {
                Title = title,
                Body = body
            },
            Data = new Dictionary<string, string>
            {
                { "type", "task_reminder" },
                { "pending", pendingCount.ToString() },
                { "in_progress", inProgressCount.ToString() },
                { "timestamp", DateTime.UtcNow.ToString("o") }
            }
        };

        try
        {
            await FirebaseMessaging.DefaultInstance.SendAsync(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error enviando notificación: {ex.Message}");
            throw;
        }
    }


    public static async Task SendToTopicAsync(string topic, string title, string body)
    {
        InitializeFirebase();

        var message = new Message()
        {
            Topic = topic,
            Notification = new Notification
            {
                Title = title,
                Body = body
            }
        };

        try
        {
            await FirebaseMessaging.DefaultInstance.SendAsync(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error enviando notificación a tema: {ex.Message}");
            throw;
        }
    }

 
    public static async Task SendPushNotificationAsync(string deviceToken, string title, string body)
    {
        var accessToken = await GetAccessTokenAsync();

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

        var message = new
        {
            message = new
            {
                token = deviceToken,
                notification = new { title, body },
                data = new { type = "custom_notification" }
            }
        };

        var json = JsonSerializer.Serialize(message);
        var response = await _httpClient.PostAsync(
            "https://fcm.googleapis.com/v1/projects/yetiy-7ccc0/messages:send",
            new StringContent(json, Encoding.UTF8, "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error FCM: {error}");
        }
    }

    private static (string title, string body) BuildTaskNotification(int pending, int inProgress)
    {
        string title = "Recordatorio de Tareas";
        string body = "";

        if (pending > 0 && inProgress > 0)
            body = $"Tienes {pending} tareas pendientes y {inProgress} en progreso";
        else if (pending > 0)
            body = $"Tienes {pending} tareas pendientes";
        else if (inProgress > 0)
            body = $"Tienes {inProgress} tareas en progreso";
        else
            body = "No tienes tareas pendientes";

        return (title, body);
    }

    private static async Task<string> GetAccessTokenAsync()
    {
        var credential = GoogleCredential
            .FromFile("firebase-adminsdk.json")
            .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");

        return await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
    }
}