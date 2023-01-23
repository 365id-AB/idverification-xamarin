using System;
using static Xamarin.Essentials.Permissions;
using System.Collections.Generic;

namespace IdVerificationSampleApp.Droid.Permissions
{
    public class NFCPermission : BasePlatformPermission
    {
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions => new List<(string androidPermission, bool isRuntime)>
        {
            (Android.Manifest.Permission.Nfc, true)
        }.ToArray();
    }
}