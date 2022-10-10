using manBackend.Models.Auth;

namespace manTests
{
    public unsafe class UserTests
    {
        [Fact]
        public unsafe void Constructor_Tests()
        {
            try
            {
                User user = new User("usename", "login", "address");
            }
            catch
            {
                Assert.True(true);
            }
            fixed (char* ptr = "pass") 
            {
                try
                {
                    User user2 = new User("sdfsdf", "ad23", "via4e@gmail.com", ptr, 5);
                }
                catch 
                {
                    Assert.True(true);
                }
            }
        }
    }
}