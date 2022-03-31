using System;
using System.Threading;
using System.Threading.Tasks;
using Zerra;
using Zerra.Identity.Consumers;
using Zerra.Identity.OpenID;

namespace SharedSecurity
{
    public static class SSOProvider
    {
        private static readonly SemaphoreSlim identityConsumerCacheLock = new SemaphoreSlim(1, 1);
        private static IIdentityConsumer identityConsumerCache = null;
        public static async Task<IIdentityConsumer> GetIdentityConsumerAsync()
        {
            if (identityConsumerCache == null)
            {
                await identityConsumerCacheLock.WaitAsync();

                if (identityConsumerCache == null)
                {
                    identityConsumerCache = await CreateOpenIDIdentityConsumerAsync();
                }
                identityConsumerCacheLock.Release();
            }

            return identityConsumerCache;
        }

        private static Task<OpenIDIdentityConsumer> CreateOpenIDIdentityConsumerAsync()
        {
            var directoryName = Config.GetSetting("AzureDirectoryName");
            var applicationClientID = Config.GetSetting("AzureB2CApplicationID");
            var b2cPolicyName = Config.GetSetting("AzureB2CPolicyName");
            var metadataUrlParameterized = Config.GetSetting("OpenIDMetadataUrlParameterized");
            var redirectUrl = Config.GetSetting("OpenIDLoginCallbackUrl");
            var postLogoutUrl = Config.GetSetting("OpenIDLogoutCallbackUrl");
            var task = OpenIDIdentityConsumer.FromMetadata(
                applicationClientID,
                            null,
                metadataUrl: String.Format(metadataUrlParameterized, directoryName, b2cPolicyName),
                redirectUrl: redirectUrl,
                redirectUrlPostLogout: postLogoutUrl,
                            null,
                responseType: OpenIDResponseType.IdToken
            );
            return task;
        }
    }
}
