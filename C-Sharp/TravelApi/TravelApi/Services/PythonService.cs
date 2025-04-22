using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TravelApi.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

public class PythonService
{
    private readonly HttpClient _httpClient;
    private readonly string _pythonServerUrl;

    public PythonService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _pythonServerUrl = configuration["PythonServerUrl"];  // 读取 Python 服务器地址
    }

    public async Task<User> RegisterUserAsync(User user)
    {
        var content = new StringContent(
            JsonSerializer.Serialize(user),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync($"{_pythonServerUrl}/user/register", content);
        
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<User>(responseContent);
        }
        
        throw new Exception($"注册失败: {response.StatusCode}");
    }

    public async Task<User> LoginAsync(long userId, long password)
    {
        var loginData = new { UserId = userId, password = password };
        var content = new StringContent(
            JsonSerializer.Serialize(loginData),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync($"{_pythonServerUrl}/user/login", content);
        
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<User>(responseContent);
        }
        
        throw new Exception($"登录失败: {response.StatusCode}");
    }

    public async Task<User> GetUserAsync(long userId)
    {
        var response = await _httpClient.GetAsync($"{_pythonServerUrl}/user/{userId}");
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<User>(content);
        }
        
        throw new Exception($"获取用户信息失败: {response.StatusCode}");
    }

    public async Task<User> UpdateUserAsync(long userId, User user)
    {
        var content = new StringContent(
            JsonSerializer.Serialize(user),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PutAsync($"{_pythonServerUrl}/user/{userId}", content);
        
        if (response.IsSuccessStatusCode)
        {
            return user;
        }
        
        throw new Exception($"更新用户信息失败: {response.StatusCode}");
    }

    public async Task DeleteUserAsync(long userId)
    {
        var response = await _httpClient.DeleteAsync($"{_pythonServerUrl}/user/{userId}");
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"删除用户失败: {response.StatusCode}");
        }
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        var response = await _httpClient.GetAsync($"{_pythonServerUrl}/users");
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<User>>(content);
        }
        
        throw new Exception($"获取所有用户失败: {response.StatusCode}");
    }

    public async Task<Journal> CreateJournalAsync(Journal journal)
    {
        var content = new StringContent(
            JsonSerializer.Serialize(journal),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync($"{_pythonServerUrl}/journal/create", content);
        
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Journal>(responseContent);
        }
        
        throw new Exception($"创建日志失败: {response.StatusCode}");
    }

    public async Task<string> UploadJournalImageAsync(long journalId, IFormFile image)
    {
        using var content = new MultipartFormDataContent();
        using var imageContent = new StreamContent(image.OpenReadStream());
        content.Add(imageContent, "picture_file", image.FileName);

        var response = await _httpClient.PostAsync($"{_pythonServerUrl}/journal/upload_image/{journalId}", content);
        
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Dictionary<string, string>>(result)["picture_path"];
        }
        
        throw new Exception($"上传图片失败: {response.StatusCode}");
    }

    public async Task<Journal> GetJournalAsync(long journalId)
    {
        var response = await _httpClient.GetAsync($"{_pythonServerUrl}/journal/{journalId}");
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Journal>(content);
        }
        
        throw new Exception($"获取日志失败: {response.StatusCode}");
    }

    public async Task<List<Journal>> GetUserJournalsAsync(long userId)
    {
        var response = await _httpClient.GetAsync($"{_pythonServerUrl}/journals/user/{userId}");
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Journal>>(content);
        }
        
        throw new Exception($"获取用户日志失败: {response.StatusCode}");
    }

    public async Task<Journal> UpdateJournalAsync(long journalId, Journal journal)
    {
        var content = new StringContent(
            JsonSerializer.Serialize(journal),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PutAsync($"{_pythonServerUrl}/journal/{journalId}", content);
        
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
            if (result.ContainsKey("journal"))
            {
                // 从响应中获取更新后的日志对象
                return JsonSerializer.Deserialize<Journal>(result["journal"].ToString());
            }
            throw new Exception("更新日志响应格式错误");
        }
        
        throw new Exception($"更新日志失败: {response.StatusCode}");
    }

    public async Task DeleteJournalAsync(long journalId)
    {
        var response = await _httpClient.DeleteAsync($"{_pythonServerUrl}/journal/{journalId}");
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"删除日志失败: {response.StatusCode}");
        }
    }

    public async Task<List<Journal>> GetAllJournalsAsync()
    {
        var response = await _httpClient.GetAsync($"{_pythonServerUrl}/journals");
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Journal>>(content);
        }
        
        throw new Exception($"获取所有日志失败: {response.StatusCode}");
    }

    public async Task<Feedback> CreateFeedbackAsync(Feedback feedback)
    {
        var content = new StringContent(
            JsonSerializer.Serialize(feedback),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync($"{_pythonServerUrl}/feedback/create", content);
        
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Feedback>(responseContent);
        }
        
        throw new Exception($"创建反馈失败: {response.StatusCode}");
    }

    public async Task<List<Feedback>> GetUserFeedbacksAsync(long userId)
    {
        var response = await _httpClient.GetAsync($"{_pythonServerUrl}/feedback/user/{userId}");
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Feedback>>(content);
        }
        
        throw new Exception($"获取用户反馈失败: {response.StatusCode}");
    }

    public async Task<List<Feedback>> GetAllFeedbacksAsync()
    {
        var response = await _httpClient.GetAsync($"{_pythonServerUrl}/feedbacks");
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Feedback>>(content);
        }
        
        throw new Exception($"获取所有反馈失败: {response.StatusCode}");
    }

    public async Task UpdateFeedbackStatusAsync(long feedbackId, string status)
    {
        var statusUpdate = new { status = status };
        var content = new StringContent(
            JsonSerializer.Serialize(statusUpdate),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PutAsync($"{_pythonServerUrl}/feedback/{feedbackId}", content);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"更新反馈状态失败: {response.StatusCode}");
        }
    }

    public async Task<(byte[] ImageData, string ContentType)> GetJournalImageAsync(long journalId)
    {
        var response = await _httpClient.GetAsync($"{_pythonServerUrl}/journal/image/{journalId}");
        
        if (response.IsSuccessStatusCode)
        {
            var contentType = response.Content.Headers.ContentType?.MediaType ?? "image/jpeg";
            var imageData = await response.Content.ReadAsByteArrayAsync();
            return (imageData, contentType);
        }
        
        throw new Exception($"获取图片失败: {response.StatusCode}");
    }

    public async Task ClearJournalImagesAsync(long journalId)
    {
        var response = await _httpClient.DeleteAsync($"{_pythonServerUrl}/journal/image/{journalId}");
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"清除日志图片失败: {response.StatusCode}");
        }
    }
}
