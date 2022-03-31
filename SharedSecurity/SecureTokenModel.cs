using System;

namespace SharedSecurity
{
    public class SecureTokenModel
    {
        public DateTime Expires { get; set; }
        public UserModel User { get; set; }
    }
}
