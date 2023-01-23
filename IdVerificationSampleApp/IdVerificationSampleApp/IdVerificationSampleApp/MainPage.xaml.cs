using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdVerificationSampleApp.Models;
using IdVerificationSampleApp.Permissions;
using IdVerificationSampleApp.Services;
using IdVerificationSampleApp.Utils;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace IdVerificationSampleApp
{
    public partial class MainPage : ContentPage
    {
        private IIdVerificationService _idVerificationService;

        public MainPage()
        {
            InitializeComponent();

            _idVerificationService = DependencyService.Get<IIdVerificationService>();

            RestHelper.Init();

            if (VersionTracking.IsFirstLaunchEver)
                AuthorizationHelper.RemoveTokens();
        }

        private void GetTokenAndStartSDK_Clicked(System.Object sender, System.EventArgs e)
            => Task.Run(async () => await StartSDK());

        private async Task StartSDK()
        {
            SetIsRunning(true);

            SetTransactionResult(null);

            var permissionsGaranted = await CheckPermissions();
            if (!permissionsGaranted)
            {
                await DisplayAlert("Error", "Camera and NFC permissions are required");
                return;
            }

            try
            {
                var accessToken = await AuthorizationHelper.GetAccessToken();
                if (string.IsNullOrEmpty(accessToken))
                    await DisplayAlert("Error", $"There was an error getting the access token");
                else
                {
                    var result = _idVerificationService.StartSDK(accessToken, Constants.LocationId, Constants.LocationName, StartWithDeviceInfoCallback);
                    ConsoleHelper.Print($"StartWithDeviceInfo result: {result}");

                    if (result)
                        _idVerificationService.PushSDKView(true);
                    else
                        await DisplayAlert("Error", $"StartWithDeviceInfo result: {result}");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message);
                ConsoleHelper.Print($"Error: {ex.Message}");
            }

            SetIsRunning(false);
        }

        private void SetIsRunning(bool isRunning)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                button.IsEnabled = !isRunning;
                activityIndicator.IsRunning = isRunning;
            });
        }

        private void SetTransactionResult(TransactionResultModel result)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                lblResultTransactionId.Text = result != null ? result.TransactionId : "-";
                lblResultStatus.Text = result != null ? result.Status : "-";
                lblResultAssessment.Text = result != null ? result.Assessment : "-";
                lblResultUserMessage.Text = result != null ? result.UserMessage : "-";
            });
        }

        private async Task<bool> CheckPermissions()
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                return await MainThread.InvokeOnMainThreadAsync<bool>(async () =>
                {
                    var status = await Xamarin.Essentials.Permissions.CheckStatusAsync<Xamarin.Essentials.Permissions.Camera>();
                    if (status != PermissionStatus.Granted)
                    {
                        status = await Xamarin.Essentials.Permissions.RequestAsync<Xamarin.Essentials.Permissions.Camera>();
                        if (status != PermissionStatus.Granted)
                            return false;
                    }

                    var permissionService = DependencyService.Resolve<IPermissionService>();
                    status = await permissionService.CheckNFCPermissionStatusAsync();
                    if (status != PermissionStatus.Granted)
                    {
                        status = await permissionService.RequestNFCPermissionAsync();
                        if (status != PermissionStatus.Granted)
                            return false;
                    }

                    return true;
                });
            }
            else
                return true;
        }

        private async Task StopSDK()
        {
            try
            {
                _idVerificationService.StopSDK();
                ConsoleHelper.Print("SDK stop was invoked");

                await Task.Delay(200);

                _idVerificationService.PopSDKView();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message);
                ConsoleHelper.Print($"Error: {ex.Message}");
            }
        }

        private void StartWithDeviceInfoCallback(TransactionResultModel result)
        {
            SetTransactionResult(result);

            var status = !string.IsNullOrEmpty(result.Status) ? result.Status.ToLower() : result.Status;

            switch (status)
            {
                case "ok":
                    // This is returned when a transaction completes successfully
                    // Note: This does not mean the user identity or supplied document is verified,
                    // only that the transaction process itself did not end prematurely.
                    // The assessment shows a summary
                    // Have a look at `result.Assessment` for more information
                    _ = DisplayAlert(Constants.AppTitle, "Ok - Transaction completes successfully");
                    break;
                case "dismissed":
                    // This is returned if the user dismisses the SDK view prematurely.
                    _ = DisplayAlert("Error", "User dismissed SDK");
                    break;
                case "clientexception":
                    // This is returned if the SDK encountered an internal error.
                    // Please Report such issues to 365id as bugs!
                    _ = DisplayAlert("Error", "Client has thrown an exception");
                    break;
                case "serverexception":
                    // This is returned if there was an issue communicating with the 365id Backend.
                    // Could be a connectivity issue.
                    _ = DisplayAlert("Error", "Server has thrown an exception");
                    break;
                default:
                    // This should not occur
                    _ = DisplayAlert("Error", "Unsupported status type was returned");
                    break;
            }

            ConsoleHelper.Print($"Result: {result.TransactionId}, {result.Status}, {result.UserMessage}, {result.Assessment}");

            Task.Run(async () =>
            {
                // Stops the SDK and releases the resources.
                await StopSDK();
            });
        }

        private async Task DisplayAlert(string title, string message)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await DisplayAlert(title, message, "OK");
            });
        }
    }
}