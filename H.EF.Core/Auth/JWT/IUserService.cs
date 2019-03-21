namespace H.EF.Core
{
    public interface IUserService
    {
        bool Auth(string username, string password);
    }

    public class UserService: IUserService
    {

        public bool Auth(string username, string password)
        {
            return true;
        }
    }
}