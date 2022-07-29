using Blackjack.Web.App.BusinessEntities.User;
using Blackjack.Web.App.Facades;
using Blackjack.Web.App.Infrastructure.Exceptions;
using System.Net;
using System.Text.RegularExpressions;
using WebException = Blackjack.Web.App.Infrastructure.Exceptions.WebException;

namespace Blackjack.Web.App.Adapters;

public interface IUserAdapter
{
    /// <summary>
    /// Adds a user to the Users table if it does not 
    /// already exist.
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    Task CreateUser(string username);

    /// <summary>
    /// Retrieves a user from the Users table. Returns null if
    /// the user does not exist.
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    Task<UserBE> GetUserBySoundex(string username);

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
    /// <param name="user"></param>
    /// <returns></returns>
    Task<UserBE> UpdateUser(UserBE user);
}
/// <inheritdoc cref="IUserAdapter"/>
public class UserAdapter : IUserAdapter
{
    private readonly IUserFacade _userFacade;

    public UserAdapter(IUserFacade userFacade)
    {
        _userFacade = userFacade;
    }

    /// <inheritdoc/>
    public async Task CreateUser(string username)
    {
        string soundex = CreateSoundex(username);
        UserBE userBE = await GetUserBySoundex(username);
        if (userBE != null)
        {
            throw new WebException(HttpStatusCode.BadRequest, "Username already exists.");
        }
        await _userFacade.CreateUser(username, soundex, balance: 500);
    }

    public async Task<UserBE> GetUserBySoundex(string username)
    {
        string soundex = CreateSoundex(username);
        List<UserBE> usersBySoundex = await _userFacade.GetUserBySoundex(soundex);
        return usersBySoundex?.SingleOrDefault(user => user.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<UserBE> GetUserByID(int userID)
    {
        return await _userFacade.GetUserByID(userID);
    }

    public async Task<UserBE> UpdateUser(UserBE user)
    {
        UserBE dbUser = await GetUserByID(user.UserID);
        if (dbUser == null)
        {
            throw new WebException(HttpStatusCode.BadRequest, "User does not exist.");
        }
        dbUser.Username = user.Username;
        dbUser.Balance = user.Balance;
        await _userFacade.UpdateUser(dbUser);
        return dbUser;
    }

    private string CreateSoundex(string username)
    {
        //After first character, delete all instances of y, h, w
        string tempName = Regex.Replace(username, @"[yhw]", "", RegexOptions.IgnoreCase).ToLower();
        // List of all characters replaced by the same number
        List<string> characterGroups = new List<string>()
        {
            "bfpv",
            "cgjkqsxz",
            "dt",
            "l",
            "mn",
            "r"
        };
        List<string> tempList = new List<string>();
        tempList.Add(username[0].ToString());
        // Skip all adjacent letters that are in the same group not separated by a vowel
        // otherwise add the corresponding number value to the soundex until it is 4 characters
        for (int i = 0; i < tempName.Length; i++)
        {
            char currChar = tempName[i];
            if ("aeiou".Contains(currChar))
                continue;
            int currGroup = 0;
            foreach (string group in characterGroups)
            {
                if (group.Contains(currChar))
                {
                    break;
                }
                currGroup++;
            }
            if (i != 0)
                tempList.Add((currGroup + 1).ToString());
            if (tempList.Count == 4)
                break;
            if (i == tempName.Length - 1)
                break;
            while (characterGroups[currGroup].Contains(tempName[i < tempName.Length - 1 ? i + 1 : i]))
                i++;
        }

        return String.Join("", tempList).PadRight(4, '0');
    }
}
