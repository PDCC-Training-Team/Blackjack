using Blackjack.Web.App.Data.Entities;
using System.Data;
using System.Data.SqlClient;

namespace Blackjack.Web.App.Data.Repos;

public interface IUserRepo
{
    /// <summary>
    /// Adds the user to the Users table if that user does not
    /// already exist.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task CreateUser(UserEntity user);

    /// <summary>
    /// Retrieves a list of users from the Users table based on Soundex.
    /// Returns null if the user does not exist.
    /// </summary>
    /// <param name="soundex"></param>
    /// <returns></returns>
    Task<List<UserEntity>> GetUsersBySoundex(string soundex);

    /// <summary>
    /// Updates a user in the Users table. Returns null if that
    /// user does not exist.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <returns></returns>
    Task UpdateUser(UserEntity userEntity);

    /// <summary>
    /// Retrieves a user from the Users table based on the UserID.
    /// Returns null if the user does not exist.
    /// </summary>
    /// <param name="userID"></param>
    /// <returns></returns>
    Task<UserEntity> GetUserByID(int userID);
}

/// <inheritdoc cref="IUserRepo"/>
public class UserRepo : BaseRepo, IUserRepo
{
    public UserRepo(BaseDataService dataService) : base(dataService)
    {
    }

    public async Task CreateUser(UserEntity user)
    {
        List<SqlParameter> parameters = new List<SqlParameter>()
        {
            new SqlParameter()
            {
                ParameterName = $"@{nameof(user.Username)}",
                SqlDbType = SqlDbType.VarChar,
                Value = user.Username
            },
            new SqlParameter()
            {
                ParameterName = $"@{nameof(user.UserSoundex)}",
                SqlDbType = SqlDbType.VarChar,
                Value = user.UserSoundex
            },
            new SqlParameter()
            {
                ParameterName = $"@{nameof(user.Balance)}",
                SqlDbType = SqlDbType.Int,
                Value = user.Balance
            }
        };
        await base.Create(StoredProcedures.CreateUser, parameters);
    }

    public async Task<List<UserEntity>> GetUsersBySoundex(string soundex)
    {
        List<SqlParameter> parameters = new List<SqlParameter>()
        {
            new SqlParameter()
            {
                ParameterName = $"@{nameof(UserEntity.UserSoundex)}",
                SqlDbType = SqlDbType.VarChar,
                Value = soundex
            }
        };

        DataTable queryData = await base.Get(StoredProcedures.GetUserBySoundex, parameters);

        List<UserEntity> userEntities = CreateUserEntity(queryData);

        return userEntities;
    }

    public async Task<UserEntity> GetUserByID(int userID)
    {
        List<SqlParameter> parameters = new List<SqlParameter>()
        {
            new SqlParameter()
            {
                ParameterName = $"@{nameof(UserEntity.UserID)}",
                SqlDbType = SqlDbType.VarChar,
                Value = userID
            }
        };

        DataTable queryData = await base.Get(StoredProcedures.GetUserByID, parameters);

        List<UserEntity> userEntities = CreateUserEntity(queryData);

        return userEntities.SingleOrDefault();
    }

    public async Task UpdateUser(UserEntity user)
    {
        List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter()
                {
                    ParameterName = $"@{nameof(user.UserID)}",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = user.UserID
                },
                new SqlParameter()
                {
                    ParameterName = $"@{nameof(user.Username)}",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = user.Username
                },
                new SqlParameter()
                {
                    ParameterName = $"@{nameof(user.Balance)}",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = user.Balance
                }
            };

        DataTable queryData =
            await base.Update(StoredProcedures.UpdateUser, parameters);
    }
    private List<UserEntity> CreateUserEntity(DataTable userDataTable)
    {
        List<UserEntity> userEntities = new List<UserEntity>();

        if (userDataTable.Rows.Count == 0)
            return null;

        foreach (DataRow row in userDataTable.Rows)
        {
            UserEntity entity = new UserEntity()
            {
                UserID = row.Field<int>(nameof(UserEntity.UserID)),
                Username = row.Field<string>(nameof(UserEntity.Username)),
                UserSoundex = row.Field<string>(nameof(UserEntity.UserSoundex)),
                Balance = row.Field<int>(nameof(UserEntity.Balance))
            };
            userEntities.Add(entity);
        }
        return userEntities;
    }
    private struct StoredProcedures
    {
        public const string CreateUser = "user.usp_INSERT_User";
        public const string GetUserBySoundex = "user.usp_SELECT_Users_BySoundex";
        public const string UpdateUser = "user.usp_UPDATE_User_ByID";
        public const string GetUserByID = "user.usp_SELECT_Users_ByID";
    }
}


