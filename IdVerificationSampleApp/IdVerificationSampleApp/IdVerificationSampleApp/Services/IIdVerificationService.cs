using System;
using System.Collections.Generic;
using IdVerificationSampleApp.Models;

namespace IdVerificationSampleApp.Services
{
    public interface IIdVerificationService
    {
        bool StartSDK(string token, int locationId, string locationName, Action<TransactionResultModel> callBack);

        void StopSDK();

        void PushSDKView(bool showAppBar);

        void PopSDKView();
    }
}