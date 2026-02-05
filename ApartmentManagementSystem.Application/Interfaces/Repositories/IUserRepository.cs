namespace ApartmentManagementSystem.Application.Interfaces.Repositories
{
    using ApartmentManagementSystem.Domain.Entities;
    public interface IUserRepository
    {     
            Task<User?> GetByUsernameAsync(string username);
            Task<User?> GetByIdAsync(Guid id);
            Task<User?> GetByPhoneAsync(string phone);
            Task AddAsync(User user);
            Task SaveChangesAsync();
            Task<List<User>> GetPendingResidentsAsync();
            Task<User?> GetByEmailAsync(string email);
            Task<bool> PhoneExistsAsync(string phone);
            Task<bool> UsernameExistsAsync(string username);
            Task UpdateAsync(User user);
        Task<User?> GetByUsernameWithRolesAsync(string username);//added new method
        Task<List<User>> GetUsersByRoleAsync(string roleName);      //added this method for assingning manager..
        Task CreateExternalManagerUserAsync(User user, string roleName);
        Task AddRoleToUserAsync(Guid userId, string roleName);
        Task<List<User>> GetUsersByRoleWithFlatsAsync(string roleName);     //added this for role dropdown

    }

}
