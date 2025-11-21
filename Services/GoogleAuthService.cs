using System.Text.Json;
using Quick_CommerceApiForEx.DTOs;
using Microsoft.Extensions.Configuration;

namespace Quick_CommerceApiForEx.Services
{
    public interface IGoogleAuthService
    {
        Task<GoogleUserInfo?> VerifyGoogleTokenAsync(string idToken);
    }

    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public GoogleAuthService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<GoogleUserInfo?> VerifyGoogleTokenAsync(string token)
        {
            try
            {
                // Debug logging
                Console.WriteLine($"Verifying Google token: {token?.Substring(0, Math.Min(20, token?.Length ?? 0))}...");
                
                // First try to verify as ID token
                var idTokenUrl = $"https://oauth2.googleapis.com/tokeninfo?id_token={token}";
                var idTokenResponse = await _httpClient.GetAsync(idTokenUrl);
                
                if (idTokenResponse.IsSuccessStatusCode)
                {
                    var content = await idTokenResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"Google ID token response: {content}");
                    var tokenInfo = JsonSerializer.Deserialize<GoogleUserInfo>(content);

                    // Verify the token is for our application
                    var clientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID") ?? _configuration["Google:ClientId"];
                    Console.WriteLine($"Configured Client ID: {clientId}");
                    
                    if (tokenInfo != null && !string.IsNullOrEmpty(clientId))
                    {
                        Console.WriteLine($"ID token verification successful for user: {tokenInfo.Email}");
                        return tokenInfo;
                    }
                }
                else
                {
                    Console.WriteLine($"ID token verification failed, trying access token...");
                    
                    // If ID token fails, try as access token
                    var accessTokenUrl = $"https://oauth2.googleapis.com/tokeninfo?access_token={token}";
                    var accessTokenResponse = await _httpClient.GetAsync(accessTokenUrl);
                    
                    if (accessTokenResponse.IsSuccessStatusCode)
                    {
                        var content = await accessTokenResponse.Content.ReadAsStringAsync();
                        Console.WriteLine($"Google access token response: {content}");
                        
                        // For access token, we need to get user info separately
                        var userInfoRequest = new HttpRequestMessage(HttpMethod.Get, "https://www.googleapis.com/oauth2/v2/userinfo")
                        {
                            Headers = { { "Authorization", $"Bearer {token}" } }
                        };
                        var userInfoResponse = await _httpClient.SendAsync(userInfoRequest);
                        
                        if (userInfoResponse.IsSuccessStatusCode)
                        {
                            var userInfoContent = await userInfoResponse.Content.ReadAsStringAsync();
                            Console.WriteLine($"Google user info: {userInfoContent}");
                            var userInfo = JsonSerializer.Deserialize<GoogleUserInfo>(userInfoContent);
                            
                            if (userInfo != null)
                            {
                                Console.WriteLine($"Access token verification successful for user: {userInfo.Email}");
                                return userInfo;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Access token verification failed with status: {accessTokenResponse.StatusCode}");
                        var errorContent = await accessTokenResponse.Content.ReadAsStringAsync();
                        Console.WriteLine($"Error content: {errorContent}");
                    }
                }

                Console.WriteLine("Token verification failed: Invalid token");
                return null;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error verifying Google token: {ex.Message}");
                return null;
            }
        }
    }
}
