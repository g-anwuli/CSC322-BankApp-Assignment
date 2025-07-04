namespace BankApp.Services
{
    public interface IAuthService
    {
        public (bool, string) Login(string email, string password, string currentPassword);
        public bool isAuthenticated();
        public void Logout();
    }

    public class AuthService : IAuthService
    {
        private bool _isAuthenticated = false;
        private DateTime _lastLoginTime = default;
        public string _currentEmail = string.Empty;

        public static TimeSpan EXP_TIME = TimeSpan.FromMinutes(5);

        public (bool, string) Login(string email, string password, string currentPassword)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(currentPassword))
            {
                return (false, "Email, password, and current password cannot be empty.");
            }


            if (password != currentPassword)
            {
                return (false, "Incorrect password.");
            }

            _isAuthenticated = true;
            _lastLoginTime = DateTime.Now;
            _currentEmail = email;

            return (true, "Login successful.");
        }

        public bool isAuthenticated()
        {
            if (!_isAuthenticated || _lastLoginTime == default)
            {
                return false;
            }


            if ((DateTime.Now - _lastLoginTime) < EXP_TIME)
            {
                _lastLoginTime = DateTime.Now;
                return true;
            }

            return _isAuthenticated;
        }

        public void Logout()
        {
            _isAuthenticated = false;
            _lastLoginTime = default;
        }
    }
}