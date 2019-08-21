using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TheWall.Models
{
    public class LoginViewModel
    {
        public User newUser{get;set;}
        public Login newLogin {get;set;}
    }
}