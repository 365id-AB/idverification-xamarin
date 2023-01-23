using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdVerificationSampleApp.iOS.Services;
using CoreAudioKit;
using Foundation;
using IdVerificationSampleApp.Models;
using IdVerificationSampleApp.Services;
using IdVerificationSampleApp.Utils;
using UIKit;
using Xamarin.Essentials;
using Xamarin.Forms;
using _365id.IdVerification.Xamarin.iOS.SDK;

[assembly: Dependency(typeof(IdVerificationService))]
namespace IdVerificationSampleApp.iOS.Services
{
    public class IdVerificationService : IIdVerificationService
    {
        private UINavigationController _uiNavigationController;

        public bool StartSDK(string token, int locationId, string locationName, Action<TransactionResultModel> callBack)
        {
            try
            {
                var keys = new List<NSString>();
                var values = new List<NSString>();

                keys.Add((NSString)Constants.AuthorizationAccessTokenKey);
                values.Add((NSString)token);

                keys.Add((NSString)Constants.DeviceInformationLocationIdKey);
                values.Add((NSString)locationId.ToString());

                keys.Add((NSString)Constants.DeviceInformationLocationNameKey);
                values.Add((NSString)locationName);

                var nsDeviceInfo = new NSDictionary<NSString, NSString>(keys.ToArray(), values.ToArray());

                return IdVerification.StartWithDeviceInfo(nsDeviceInfo, (TransactionResult transactionResult) =>
                {
                    callBack?.Invoke(new TransactionResultModel
                    {
                        TransactionId = transactionResult.TransactionId,
                        UserMessage = transactionResult.UserMessage,
                        Status = transactionResult.Status.ToString(),
                        Assessment = transactionResult.Assessment.ToString()
                    });
                });
            }
            catch (Exception ex)
            {
                ConsoleHelper.Print(ex.Message);
                return false;
            }
        }

        public void StopSDK()
        {
            IdVerification.Stop();
        }

        public void PushSDKView(bool showAppBar)
        {
            Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
            {
                _uiNavigationController = GetNavigationController();
                if (_uiNavigationController != null)
                {
                    var uiViewController = IdVerification.CreateMainViewWithShowAppBar(showAppBar);
                    _uiNavigationController.PushViewController(uiViewController, true);
                }
                else
                    ConsoleHelper.Print("The Xamarin Forms MainPage must be a NavigationPage");
            });
        }

        public void PopSDKView()
        {
            Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
            {
                if (_uiNavigationController != null)
                    _uiNavigationController.PopViewController(true);
                else
                    ConsoleHelper.Print("The Xamarin Forms MainPage must be a NavigationPage");
            });
        }

        private UINavigationController GetNavigationController()
        {
            try
            {
                return (UINavigationController)UIApplication.SharedApplication.KeyWindow.RootViewController.ChildViewControllers.First();
            }
            catch (Exception ex)
            {
                ConsoleHelper.Print(ex.Message);
                return null;
            }
        }
    }
}