using AutoMapper;
using Blackjack.Web.App.BusinessEntities.User;
using Blackjack.Web.App.BusinessModels.User;

namespace Blackjack.Web.App.WebService.Controllers.Translators.Users
{
    public class UserBE_UserBM : ITypeConverter<UserBE, UserBM>, ITypeConverter<UserBM, UserBE>
    {
        public UserBE Convert(UserBM source, UserBE destination, ResolutionContext context)
        {
            UserBE result = destination ?? new();
            result.UserID = source.UserID;
            result.Username = source.Username;
            result.Balance = source.Balance;
            return result;
        }
        public UserBM Convert(UserBE source, UserBM destination, ResolutionContext context)
        {
            UserBM result = destination ?? new();
            result.UserID = source.UserID;
            result.Username = source.Username;
            result.Balance = source.Balance;
            return result;
        }
    }
}
