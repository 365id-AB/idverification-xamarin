using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace IdVerificationSampleApp.Permissions
{
    public interface IPermissionService
    {
        Task<PermissionStatus> CheckNFCPermissionStatusAsync();

        Task<PermissionStatus> RequestNFCPermissionAsync();
    }
}