﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CountingKs.Models
{
    public class AuthTokenModel
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}