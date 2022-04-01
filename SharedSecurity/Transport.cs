using System;
using System.Security;
using System.Security.Claims;
using Zerra;
using Zerra.Encryption;
using Zerra.Serialization;

namespace SharedSecurity
{
    public static class Transport
    {
        private static readonly SymmetricKey key = SymmetricEncryptor.GetKey(Config.GetSetting("SharedEncryptionKey"));
        private static readonly ByteSerializer serializer = new ByteSerializer();

        public static string CreateSecureToken(int? expiresInSeconds = 90)
        {
            var principal = System.Threading.Thread.CurrentPrincipal as ClaimsPrincipal;
            if (principal == null)
                throw new Exception("User not logged in");

            var user = Auth.GetUserFromClaims();

            var model = new SecureTokenModel()
            {
                Expires = expiresInSeconds.HasValue ? DateTime.UtcNow.AddSeconds(expiresInSeconds.Value) : (DateTime?)null,
                User = user,
            };

            var serialized = serializer.Serialize(model);
            var encrypted = SymmetricEncryptor.Encrypt(SymmetricAlgorithmType.AESwithShift, key, serialized);
            var encoded = Convert.ToBase64String(encrypted);
            return encoded;
        }
        public static string CreateSecureToken<T>(T data, int? expiresInSeconds = 90)
        {
            var principal = System.Threading.Thread.CurrentPrincipal as ClaimsPrincipal;
            if (principal == null)
                throw new Exception("User not logged in");

            var user = Auth.GetUserFromClaims();

            var model = new SecureTokenModel<T>()
            {
                Expires = expiresInSeconds.HasValue ? DateTime.UtcNow.AddSeconds(expiresInSeconds.Value) : (DateTime?)null,
                User = user,
                Data = data
            };

            var serialized = serializer.Serialize(model);
            var encrypted = SymmetricEncryptor.Encrypt(SymmetricAlgorithmType.AESwithShift, key, serialized);
            var encoded = Convert.ToBase64String(encrypted);
            return encoded;
        }

        public static SecureTokenModel ReadSecureToken(string data)
        {
            SecureTokenModel model;
            try
            {
                var encrypted = Convert.FromBase64String(data);
                var serialized = SymmetricEncryptor.Decrypt(SymmetricAlgorithmType.AESwithShift, key, encrypted);
                model = serializer.Deserialize<SecureTokenModel>(serialized);
            }
            catch
            {
                throw new SecurityException("Invalid Secure Token");
            }

            if (model.Expires.HasValue && model.Expires < DateTime.UtcNow)
                throw new SecurityException("Secure Token Has Expired");

            return model;
        }
        public static SecureTokenModel<T> ReadSecureToken<T>(string data)
        {
            SecureTokenModel<T> model;
            try
            {
                var encrypted = Convert.FromBase64String(data);
                var serialized = SymmetricEncryptor.Decrypt(SymmetricAlgorithmType.AESwithShift, key, encrypted);
                model = serializer.Deserialize<SecureTokenModel<T>>(serialized);
            }
            catch
            {
                throw new SecurityException("Invalid Secure Token");
            }

            if (model.Expires.HasValue && model.Expires < DateTime.UtcNow)
                throw new SecurityException("Secure Token Has Expired");

            return model;
        }
    }
}
