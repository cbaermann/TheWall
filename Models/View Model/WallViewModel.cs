using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TheWall.Models
{
    public class WallViewModel
    {
        public User User{get;set;}
        public Message Message{get;set;}
        public Comment Comment{get;set;}
        public List<Message> Messages{get;set;}
        public List<Comment> Comments{get;set;}
    }
}