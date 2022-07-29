using AutoMapper;
using Blackjack.Web.App.BusinessEntities.User;
using Blackjack.Web.App.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack.Web.App.Facades.Translators.User
{
    public class UserEntity_UserBE : ITypeConverter<UserEntity, UserBE>, ITypeConverter<UserBE, UserEntity>
    {
        public UserEntity Convert(UserBE source, UserEntity destination, ResolutionContext context)
        {
            UserEntity result = destination ?? new();
            result.UserID = source.UserID;
            result.Balance = source.Balance;
            result.Username = source.Username;
            return result;
        }

        public UserBE Convert(UserEntity source, UserBE destination, ResolutionContext context)
        {
            UserBE result = destination ?? new();
            result.UserID = source.UserID;
            result.Balance = source.Balance;
            result.Username = source.Username;
            return result;
        }
    }
}
