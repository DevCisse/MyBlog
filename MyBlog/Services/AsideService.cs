using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MyBlog.Data;
using MyBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Services
{



    public class AsideService
    {
        private readonly ApplicationDbContext context;

        public AsideService(ApplicationDbContext context)
        {
            this.context = context;
        }



        public async Task<IEnumerable<Blog>> GetBlogsAsync()
        {
            return await context.Blogs.Include(b => b.Posts).ToListAsync();
        }



        public async Task<IEnumerable<Tag>> GetTagsAsync()
        {
            return await context.Tags.Distinct().ToListAsync(); ;
        }



        public async Task<IEnumerable<Post>> GetTopThreeLatestPosts()
        {
            return await context.Posts.Where(r=>r.ReadyStatus == Models.Enums.ReadyStatus.ProductionReady).OrderByDescending(o=>o.Created).Take(3).ToListAsync() ;
        }
    }
}
