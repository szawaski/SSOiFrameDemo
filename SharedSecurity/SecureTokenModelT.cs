using System;

namespace SharedSecurity
{
    public class SecureTokenModel<T>
    {
        public DateTime? Expires { get; set; }
        public UserModel User { get; set; }
        public T Data { get; set; }
    }
}
