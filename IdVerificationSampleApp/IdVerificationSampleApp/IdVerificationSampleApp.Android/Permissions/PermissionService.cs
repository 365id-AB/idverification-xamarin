using System;
using System.Threading.Tasks;
using IdVerificationSampleApp.Droid.Permissions;
using IdVerificationSampleApp.Permissions;
using IdVerificationSampleApp.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(PermissionService))]
namespace IdVerificationSampleApp.Droid.Permissions
{
    public class PermissionService : IPermissionService
    {
        public async Task<PermissionStatus> CheckNFCPermissionStatusAsync()
            => await Xamarin.Essentials.Permissions.CheckStatusAsync<NFCPermission>();

        public async Task<PermissionStatus> RequestNFCPermissionAsync()
            => await Xamarin.Essentials.Permissions.RequestAsync<NFCPermission>();
    }
}