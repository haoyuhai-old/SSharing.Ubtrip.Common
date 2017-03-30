using System;
using System.Collections.Generic;
using System.Text;

namespace ZhiBen.Framework.Service.Common
{
    public class UserInfo
    {
        // Fields
        private string _UserId;

        // Methods
        public UserInfo(string userId)
        {
            this._UserId = userId;
        }

        // Properties
        public string UseId
        {
            get
            {
                return this._UserId;
            }
        }
    }
}
