using TabloidMVC.Models;
using System.Collections.Generic;

namespace TabloidMVC.Models.ViewModels
{
    public class PostTagViewModel
    {
        public Post Post { get; set; }
        public List<Tag> PostTags { get; set; } 
        public List<Tag> Tags { get; set; }
    }
}
