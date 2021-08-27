using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Models
{
    public class BlogUser : IdentityUser
    {

        [Required]
        [StringLength(50,ErrorMessage ="The {0} must be at least {2} and no more than {1}",MinimumLength =2)]
        [Display(Name ="First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name= "Last name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and no more than {1}",MinimumLength =2)]
        public string LastName { get; set; }
        public byte[] Image{ get; set; }
        public string ContentType { get; set; }
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and no more than {1}", MinimumLength = 2)]
        public string   FacebookUrl{ get; set; }
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and no more than {1}", MinimumLength = 2)]
        public string TwitterUrl { get; set; }


        [NotMapped]
        public string FullName { get => $"{FirstName} {LastName}"; }

        public virtual ICollection<Blog> Blogs { get; set; } = new HashSet<Blog>();

        public virtual ICollection<Post> Posts { get; set; } = new HashSet<Post>();


    }
}
