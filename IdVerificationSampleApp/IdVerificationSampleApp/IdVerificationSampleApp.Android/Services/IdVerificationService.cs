using System;
using System.Collections.Generic;
using IdVerificationSampleApp.Droid.Services;
using Android.Media.TV;
using Com.Id365.Idverification;
using IdVerificationSampleApp.Models;
using IdVerificationSampleApp.Services;
using IdVerificationSampleApp.Utils;
using Kotlin.Jvm.Functions;
using Xamarin.Essentials;
using Xamarin.Forms;
using Android.App;
using Android.Content;

[assembly: Dependency(typeof(IdVerificationService))]
namespace IdVerificationSampleApp.Droid.Services
{
    public class IdVerificationService : IIdVerificationService
    {
        private Android.Views.View _initialView;

        public bool StartSDK(string token, int locationId, string locationName, Action<TransactionResultModel> callBack)
        {
            try
            {
                var request = new _365iDRequest(token, locationName, locationId);

                var result = SdkContextKt.StartSdk(Platform.CurrentActivity.ApplicationContext, request, new StartCallbackFunction<_365iDResult>((result) =>
                {
                    callBack?.Invoke(new TransactionResultModel
                    {
                        TransactionId = result.TransactionId,
                        UserMessage = result.UserMessage,
                        Status = result.Status.ToString(),
                        Assessment = result.Assessment.ToString()
                    });
                }));

                return result;
            }
            catch (Exception ex)
            {
                ConsoleHelper.Print(ex.Message);
                return false;
            }
        }

        public void StopSDK()
        {
            try
            {
                Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
                {
                    SdkContextKt.StopSdk();
                });
            }
            catch (Exception ex)
            {
                ConsoleHelper.Print(ex.Message);
            }
        }

        public void PushSDKView(bool showAppBar)
        {
            try
            {
                Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
                {
                    _initialView = ((Android.Views.ViewGroup)Platform.CurrentActivity.FindViewById(Android.Resource.Id.Content)).GetChildAt(0);
                    var nativeView = SdkContextKt.GetView(Platform.CurrentActivity, null, Resource.Attribute.materialThemeOverlay, showAppBar);
                    Platform.CurrentActivity.SetContentView(nativeView);
                });
            }
            catch (Exception ex)
            {
                ConsoleHelper.Print(ex.Message);
            }
        }

        public void PopSDKView()
        {
            try
            {
                Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
                {
                    Platform.CurrentActivity.SetContentView(_initialView);
                });
            }
            catch (Exception ex)
            {
                ConsoleHelper.Print(ex.Message);
            }
        }

        public void SendIntentToSDK(Intent intent)
        {
            try
            {
                Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
                {
                    SdkContextKt.SendIntentToSdk(intent);
                });
            }
            catch (Exception ex)
            {
                ConsoleHelper.Print(ex.Message);
            }
        }
    }

    public class StartCallbackFunction<T> : Java.Lang.Object, IFunction1 where T : Java.Lang.Object
    {
        private readonly Action<T> _onInvoked;

        public StartCallbackFunction(Action<T> onInvoked)
        {
            _onInvoked = onInvoked;
        }

        public Java.Lang.Object Invoke(Java.Lang.Object objParameter)
        {
            try
            {
                T parameter = (T)objParameter;
                _onInvoked?.Invoke(parameter);
            }
            catch (Exception ex)
            {
                ConsoleHelper.Print(ex.Message);
            }

            return null;
        }
    }
}