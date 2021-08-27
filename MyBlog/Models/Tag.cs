using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string BlogUserId { get; set; }

        [Required]
        [StringLength(25, ErrorMessage = "The {0} must be at least {2} and at most {1}", MinimumLength = 2)]
        [Display(Name = "Comment")]
        public string Text { get; set; }


        public virtual Post Post { get; set; }
        public virtual BlogUser  BlogUser { get; set; }
    }
}
