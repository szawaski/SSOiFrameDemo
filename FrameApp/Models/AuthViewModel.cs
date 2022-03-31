using SharedSecurity;

namespace FrameApp.Models
{
    public class HomeViewModel
    {
        public string UserName { get; set; }
        public string[] Roles { get; set; }

        public TestDataModel DataFromMainApp { get; set; }
    }
}
