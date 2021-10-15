using Microsoft.EntityFrameworkCore;
using MyBlog.Data;
using MyBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Services
{
    public class SearchService
    {
        private readonly ApplicationDbContext context;

        public SearchService(ApplicationDbContext context)
        {
            this.context = context;
        }


        public IQueryable<Post> SearcTag(string tag)
        {
            var posts = context.Posts.Where(p => p.ReadyStatus == Models.Enums.ReadyStatus.ProductionReady).AsQueryable();
            if (tag != null)
            {
                tag = tag.ToLower();
                posts = posts.Include(p => p.Tags).Where(

                    p => p.Tags.Any(t => t.Text.ToLower().Contains(tag)));
            }


            return posts;

        }

        public IQueryable<Post> Search(string searchTerm)
        {

            var posts = context.Posts.Where(p => p.ReadyStatus == Models.Enums.ReadyStatus.ProductionReady).AsQueryable();

            if (searchTerm != null)
            {

                searchTerm = searchTerm.ToLower();

                posts = posts.Include(p => p.Blog).Where(
                   p => p.Title.ToLower().Contains(searchTerm) ||

                   p.Abstract.ToLower().Contains(searchTerm) ||

                   p.Content.ToLower().Contains(searchTerm) ||

                   p.Comments.Any(c => c.Body.ToLower().Contains(searchTerm) ||

                                   c.ModeratedBody.ToLower().Contains(searchTerm) ||

                                   c.BlogUser.FirstName.ToLower().Contains(searchTerm) ||

                                   c.BlogUser.LastName.ToLower().Contains(searchTerm))



                    );

            }


            return posts.OrderByDescending(p=>p.Created);
        }
    }

}
