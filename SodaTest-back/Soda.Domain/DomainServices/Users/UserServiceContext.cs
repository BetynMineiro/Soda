namespace Soda.Domain.DomainServices.Users;

public class UserServiceContext
{
    private readonly UserAuth _userAuth = new();
    public UserAuth UserAuth => _userAuth;

    public void AddUser(string id)
    {
        _userAuth.Id = id;
    }
}