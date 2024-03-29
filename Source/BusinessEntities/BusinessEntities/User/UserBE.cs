﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack.Web.App.BusinessEntities.User
{
    public class UserBE
    {
        /// <summary>
        /// The ID number of the user.
        /// </summary>
        public int UserID;
        /// <summary>
        /// The unique username of the user.
        /// </summary>
        public string Username;
        /// <summary>
        /// The current balance of the user.
        /// </summary>
        public int Balance;
    }
}
