﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public class Account : Entity, IAwake
    {
        public string account;
        public string password;
        public long createTime;
        public AccountType accountType;
    }

    public enum AccountType
    {
        //一般用户
        General,

        //黑名单
        BlackList
    }
}
