namespace IDM.Infrastructure.Repositories.Interfaces
{
    public interface IUserContext
    {
        Guid GetUserId();
        string GetUserName();
    }
}
