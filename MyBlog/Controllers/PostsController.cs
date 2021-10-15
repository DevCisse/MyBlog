using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyBlog.Data;
using MyBlog.Models;
using MyBlog.Models.Enums;
using MyBlog.Services;
using MyBlog.ViewModels;
using X.PagedList;

namespace MyBlog.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISlugService slugService;
        private readonly IImageService imageService;
        private readonly UserManager<BlogUser> userManager;
        private readonly SearchService searchService;

        public PostsController(ApplicationDbContext context,ISlugService slugService,IImageService imageService,UserManager<BlogUser> userManager,SearchService searchService)
        {
            _context = context;
            this.slugService = slugService;
            this.imageService = imageService;
            this.userManager = userManager;
            this.searchService = searchService;
        }

        // GET: Posts



        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index()
        {

           
            var applicationDbContext = _context.Posts.Include(p => p.Blog).Include(p => p.BlogUser);
            return View(await applicationDbContext.ToListAsync());
        }


      
        // GET: Posts/Details/5
        public async Task<IActionResult> Details(string slug)
        {
            if (slug == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Blog)
                .Include(p => p.BlogUser)
                
                .Include(p => p.Comments).ThenInclude(c => c.BlogUser)
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(m => m.Slug == slug);

          

            if (post == null)
            {
                return NotFound();
            }

            PostDetailsViewModel model = new()
            {
                Post = post,
                Comments = post.Comments,
                Tags = post.Tags
            };

            post.View++;
           await  _context.SaveChangesAsync();


            return View(model);
        }

        // GET: Posts/Create

        public async Task<IActionResult> SearchIndex(int? page, string searchTerm)
        {

            ViewData["SearchTerm"] = searchTerm;

            var pageNumber = page ?? 1;
            var pageSize = 4;
            var posts = searchService.Search(searchTerm);

           
            return View(await posts.ToPagedListAsync(pageNumber, pageSize));

        }

        public async Task<IActionResult> TagIndex(int? page, string tag)
        {
            ViewData["Tag"] = tag;

            var pageNumber = page ?? 1;
            var pageSize = 4;
            var posts = searchService.SearcTag(tag);


            return View(await posts.ToPagedListAsync(pageNumber, pageSize));
        }

        [Authorize(Roles ="Administrator")]
        public IActionResult Create()
        {
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description");
            ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BlogId,Title,Abstract,Content,Created,ReadyStatus,Image")] Post post,List<string> tagValues)
        {
            if (ModelState.IsValid)
            {

                post.Created = DateTime.Now;

                var slug = slugService.UrlFriendly(post.Title);
                post.Slug = slug;

                var authorId =  userManager.GetUserId(User);


                bool validationError = default ;

                if(string.IsNullOrEmpty(slug))
                {
                    validationError = true; ;

                    ModelState.AddModelError("", "The title you provided cannot be used as it empty");
                 
                }
                if(!slugService.IsUnique(slug))
                {
                    validationError = true;
                    ModelState.AddModelError("Title", "The title you provided cannot be used as it results in a duplicate slug.");
                  
                }

                if(validationError)
                {
                    ViewData["TagValues"] = string.Join(",", tagValues);
                    return View(post);
                }

                post.ImageData = await imageService.EncodeImageAsync(post.Image);
                post.ContentType = imageService.ContentType(post.Image);

                post.Slug = slug;

                
                
                _context.Add(post);
                await _context.SaveChangesAsync();



                foreach (var tag in tagValues)
                {
                    _context.Add(new Tag
                    {
                        PostId = post.Id,
                        Text = tag,
                        BlogUserId = authorId

                    });
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name", post.BlogId);
            return View(post);
        }

        // GET: Posts/Edit/5

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description", post.BlogId);
            ViewData["TagValues"] = string.Join(",", post.Tags.Select(t=>t.Text));
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(int id, [Bind("Id,BlogId,Title,Abstract,Content,ReadyStatus")] Post post,IFormFile newImage,List<string> tagValues)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    var newPost = await _context.Posts.Include(p =>p.Tags).FirstOrDefaultAsync(p => p.Id
                         == id);
                    newPost.Updated = DateTime.Now;
                    newPost.Title = post.Title;
                    newPost.Abstract = post.Abstract;
                    newPost.Content = post.Content;
                    newPost.ReadyStatus = post.ReadyStatus;
                    newPost.BlogUserId = userManager.GetUserId(User);

                    var authorId = userManager.GetUserId(User);


                    var newSlug = slugService.UrlFriendly(post.Title);
                    if(newSlug !=  newPost.Slug)
                    {
                        if(slugService.IsUnique(newSlug))
                        {
                            newPost.Title = post.Title;
                            newPost.Slug = newSlug;
                        }
                        else
                        {
                            ModelState.AddModelError("Title", "The title you provided cannot be used as it results in a duplicate slug.");
                            ViewData["TagValues"] = string.Join(",", tagValues);
                            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name", post.BlogId);
                            return View(post);
                        }
                    }





                    if (newImage is not null)
                    {
                        newPost.ImageData = await imageService.EncodeImageAsync(newImage);
                        newPost.ContentType = imageService.ContentType(newImage);

                    }

                    //remove old tags
                    _context.Tags.RemoveRange(newPost.Tags);

                    //add new tags

                    foreach (var tag in tagValues)
                    {
                        _context.Tags.Add(new Tag
                        {
                            PostId = post.Id,
                            BlogUserId = authorId,
                            Text = tag

                        }); 
                    }


                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name", post.BlogId);
           
            return View(post);
        }

        // GET: Posts/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Blog)
                .Include(p => p.BlogUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
