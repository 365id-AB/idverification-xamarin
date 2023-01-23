using System;

namespace IdVerificationSampleApp.Utils
{
    public static class Constants
    {
        public const string AppTitle = "365id Id Verification";

        public const string AuthorizationAPIHost = "https://eu.customer.365id.com";

        public const string SecureStorageAccessTokenKey = "AccessToken";

        public const string SecureStorageRefreshTokenKey = "RefreshToken";

        public const string DeviceInformationLocationIdKey = "LocationId";

        public const string DeviceInformationLocationNameKey = "LocationName";

        public const string AuthorizationAccessTokenKey = "Token";

        // TODO: set the client id
        public const string ClientId = "[CLIENT_ID]";

        // TODO: set the client secret
        public const string ClientSecret = "[CLIENT_SECRET]";

        // TODO: set the location id
        public const int LocationId = 0;

        // TODO: set the location name
        public const string LocationName = "UNKNOWN";
    }
}