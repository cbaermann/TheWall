using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheWall.Models
{
    public class Login
    {
        [Required(ErrorMessage="Please enter your email")]
        [EmailAddress]
        public string loginEmail{get;set;}

        [Required(ErrorMessage="Enter your password")]
        [MinLength(8, ErrorMessage="Password must be at least 8 characters")]
        [DataType(DataType.Password)]
        public string loginPassword{get;set;}
    }
}