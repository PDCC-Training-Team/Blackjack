using AutoMapper;
using Blackjack.Web.App.BusinessEntities.User;
using Blackjack.Web.App.Data;
using Blackjack.Web.App.Data.Entities;
using System.Linq;

namespace Blackjack.Web.App.Facades;

public interface IUserFacade
{
    /// <summary>
    /// Adds a user to the Users table if it does not 
    /// already exist.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="soundex"></param>
    /// <param name="balance"></param>
    /// <returns></returns>
    Task CreateUser(string username, string soundex, int balance);
    /// <summary>
    /// Retrieves a list of users from the Users table based on Soundex.
    /// Returns null if the user does not exist.
    /// </summary>
    /// <param name="soundex"></param>
    /// <returns></returns>
    Task<List<UserBE>> GetUserBySoundex(string soundex);

    /// <summary>
    /// Retrieves a user from the Users table based on the UserID.
    /// Returns null if the user does not exist.
    /// </summary>
    /// <param name="userID"></param>
    /// <returns></returns>
    Task<UserBE> GetUserByID(int userID);

    /// <summary>
    /// Updates a user in the Users table.
    /// </summary>
    /// <param name="userBE"></param>
    /// <returns></returns>
    Task UpdateUser(UserBE userBE);
}

/// <inheritdoc cref="IUserFacade"/>
public class UserFacade : IUserFacade
{
    private readonly IDataService _dataService;
    private readonly IMapper _mapper;

    public UserFacade(IDataService dataService, IMapper mapper)
    {
        _dataService = dataService;
        _mapper = mapper;
    }

    public async Task CreateUser(string username, string soundex, int balance)
    {
        UserEntity user = new UserEntity()
        {
            Username = username,
            UserSoundex = soundex,
            Balance = balance
        };
        await _dataService.UserRepo.CreateUser(user);
    }

    public async Task<List<UserBE>> GetUserBySoundex(string soundex)
    {
        List<UserEntity> userEntities = await _dataService.UserRepo.GetUsersBySoundex(soundex);
        List<UserBE> UserBEs = userEntities?.Select(userEntity => _mapper.Map<UserBE>(userEntity)).ToList();
        return UserBEs;
    }

    public async Task<UserBE> GetUserByID(int userID)
    {
        UserEntity userEntity = await _dataService.UserRepo.GetUserByID(userID);
        return _mapper.Map<UserBE>(userEntity);
    }

    public async Task UpdateUser(UserBE userBE)
    {
        UserEntity userEntity = _mapper.Map<UserEntity>(userBE);
        await _dataService.UserRepo.UpdateUser(userEntity);
    }
}
