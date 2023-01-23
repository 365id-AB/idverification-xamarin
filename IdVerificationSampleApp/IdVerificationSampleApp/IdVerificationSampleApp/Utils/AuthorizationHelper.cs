using System;
using System.Threading.Tasks;
using IdVerificationSampleApp.Models;
using IdVerificationSampleApp.Services;

namespace IdVerificationSampleApp.Utils
{
    public static class AuthorizationHelper
    {
        public static async Task<string> GetAccessToken()
        {
            var refreshToken = await Xamarin.Essentials.SecureStorage.GetAsync(Constants.SecureStorageRefreshTokenKey);

            // TODO: Remove it to refresh the saved token
            refreshToken = null;

            if (string.IsNullOrEmpty(refreshToken))
                return await GetNewAccessToken();
            else
                return await RefreshToken(refreshToken);
        }

        private static async Task<string> GetNewAccessToken()
        {
            string accessToken = null;

            try
            {
                var response = await RestHelper.PostAsync<AccessTokenRequestDTO, AccessTokenResponseDTO>($"{Constants.AuthorizationAPIHost}/api/v1/access_token", new AccessTokenRequestDTO
                {
                    ClientId = Constants.ClientId,
                    ClientSecret = Constants.ClientSecret
                });

                if (!string.IsNullOrEmpty(response.Error) || !string.IsNullOrEmpty(response.ErrorDescription))
                    ConsoleHelper.Print($"Error: {response.Error} - {response.ErrorDescription}");
                else if (string.IsNullOrEmpty(response.AccessToken))
                    ConsoleHelper.Print($"Error: Access token is null");
                else if (string.IsNullOrEmpty(response.RefreshToken))
                    ConsoleHelper.Print($"Error: Refresh token is null");
                else
                {
                    await Xamarin.Essentials.SecureStorage.SetAsync(Constants.SecureStorageAccessTokenKey, response.AccessToken);
                    await Xamarin.Essentials.SecureStorage.SetAsync(Constants.SecureStorageRefreshTokenKey, response.RefreshToken);

                    accessToken = response.AccessToken;
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.Print($"Error: {ex.Message}");
            }

            return accessToken;
        }

        private static async Task<string> RefreshToken(string refreshToken)
        {
            string accessToken = null;

            try
            {
                var response = await RestHelper.PostAsync<RefreshTokenRequestDTO, RefreshTokenResponseDTO>($"{Constants.AuthorizationAPIHost}/api/v1/refresh_token", new RefreshTokenRequestDTO
                {
                    RefreshToken = refreshToken
                });

                if (!string.IsNullOrEmpty(response.Error) || !string.IsNullOrEmpty(response.ErrorDescription))
                    ConsoleHelper.Print($"Error: {response.Error} - {response.ErrorDescription}");
                else if (string.IsNullOrEmpty(response.AccessToken))
                    ConsoleHelper.Print($"Error: Access token is null");
                else if (string.IsNullOrEmpty(response.RefreshToken))
                    ConsoleHelper.Print($"Error: Refresh token is null");
                else
                {
                    await Xamarin.Essentials.SecureStorage.SetAsync(Constants.SecureStorageAccessTokenKey, response.AccessToken);
                    await Xamarin.Essentials.SecureStorage.SetAsync(Constants.SecureStorageRefreshTokenKey, response.RefreshToken);

                    accessToken = response.AccessToken;
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.Print($"Error: {ex.Message}");
            }

            return accessToken;
        }

        public static void RemoveTokens()
            => Xamarin.Essentials.SecureStorage.RemoveAll();
    }
}