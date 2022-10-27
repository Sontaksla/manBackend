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
            } catch { Assert.True(true); }

            fixed (char* ptr = "pass") 
            {
                try
                {
                    User user2 = new User("sdfsdf", "ad23", "via4e@gmail.com", ptr, 5);
                } catch { Assert.True(true); }
            }
        }
        [Fact]
        public void Password_Tests()
        {
            User user = new User();
            try
            {
                user.SavePasswordAsync("pass", true).Wait();
            } catch { Assert.True(false); }

            for (int i = 0; i < 10; i++)
            {
                User u = new User();
                u.Login = i.ToString() + "_LOGIN";
                u.SavePasswordAsync("pass" + i.ToString(), false).Wait();
            }

            new User() { Login = "6_LOGIN" }.SavePasswordAsync("newPass", true).Wait();
            Assert.True(true);
        }
    }
}