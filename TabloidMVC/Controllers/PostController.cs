using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using TabloidMVC.Models;
using TabloidMVC.Models.ViewModels;
using TabloidMVC.Repositories;
using System.Linq;

namespace TabloidMVC.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICommentRepository _commentRepository;


        public PostController(IPostRepository postRepository, ICategoryRepository categoryRepository, ICommentRepository commentRepository)
        {
            _postRepository = postRepository;
            _categoryRepository = categoryRepository;
            _commentRepository = commentRepository;

        }

        //Posts sorted by PublishDateTime newest first

        public IActionResult Index()
        {
            //Return posts
            var posts = _postRepository.GetAllPublishedPosts();
            //Sort by PublishDateTime
            posts.Sort((y, x) => DateTime.Compare((DateTime)x.PublishDateTime, (DateTime)y.PublishDateTime));
            return View(posts);
        }

        //Sorted by CreatedDateTime
        public IActionResult MyPosts()
        {
            //Return posts
            var posts = _postRepository.GetAllPublishedPosts();
            //Sort Only posts that match current user
            posts = posts.Where(p => GetCurrentUserProfileId() == p.UserProfileId).ToList();

            //Sort by CreatedDateTime newest created first

            posts.Sort((y, x) => DateTime.Compare(x.CreateDateTime, y.CreateDateTime));
            return View(posts);
        }

        public IActionResult Details(int id)
        {

            //Pass the Current userid to HTML file
            ViewData["currentUserId"] = GetCurrentUserProfileId();


            var post = _postRepository.GetPublishedPostById(id);
            if (post == null)
            {
                int userId = GetCurrentUserProfileId();
                post = _postRepository.GetUserPostById(id, userId);
                if (post == null)
                {
                    return NotFound();
                }
            }
            return View(post);
        }

        public IActionResult Comments(int id)
        {
            Post post = _postRepository.GetPublishedPostById(id);
            List<Comment> comment = _commentRepository.GetAllCommentsByPostId(id);
            

            //Sort by CreatedDateTime newest created first
            comment.Sort((y, x) => DateTime.Compare(x.CreateDateTime, y.CreateDateTime));
            var vm = new PostIndexViewModel()
            {
                Post = post,
                Comments = comment

            };
            if (comment == null)
            {

                return NotFound();

            }
            return View(vm);
        }

        public IActionResult MyPostDetails(int id)
        {
            var post = _postRepository.GetPublishedPostById(id);
            if (post == null)
            {
                int userId = GetCurrentUserProfileId();
                post = _postRepository.GetUserPostById(id, userId);
                if (post == null)
                {
                    return NotFound();
                }
            }
            return View(post);
        }

        //Get: PostCotrollers/Create
        public IActionResult Create()
        {
            var vm = new PostCreateViewModel();
            vm.CategoryOptions = _categoryRepository.GetAll();
            return View(vm);
        }

        //Post: PostControllers/Create
        [HttpPost]
        public IActionResult Create(PostCreateViewModel vm)
        {
            try
            {
                vm.Post.CreateDateTime = DateAndTime.Now;
                vm.Post.IsApproved = true;
                vm.Post.UserProfileId = GetCurrentUserProfileId();

                _postRepository.Add(vm.Post);

                return RedirectToAction("Details", new { id = vm.Post.Id });
            } 
            catch
            {
                vm.CategoryOptions = _categoryRepository.GetAll();
                return View(vm);
            }
        }

        private int GetCurrentUserProfileId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id);
        }

        
        // GET: PostsController/Edit
        [Authorize]
        public ActionResult Edit(int id)
        {

            List<Category> categories = _categoryRepository.GetAll();
            Post post = _postRepository.GetPublishedPostById(id);

            var vm = new PostCreateViewModel()
            {
                Post = post,
                CategoryOptions = categories

            };
            vm.CategoryOptions = _categoryRepository.GetAll();


            if (post == null || post.UserProfileId != GetCurrentUserProfileId())
            {
                return NotFound();
            }

            return View(vm);
        }

        // POST: PostsController/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(int id, Post post)
        {
            List<Category> categories = _categoryRepository.GetAll();
            post.UserProfileId = _postRepository.GetPublishedPostById(id).UserProfileId;
            var vm = new PostCreateViewModel()
            {
                Post = post,
                CategoryOptions = categories

            };
            vm.CategoryOptions = _categoryRepository.GetAll();

            if (post == null || post.UserProfileId != GetCurrentUserProfileId())
            {
                return NotFound();
            }
            try
            {
                vm.Post.IsApproved = true;
                post.UserProfileId = GetCurrentUserProfileId();
                _postRepository.UpdatePost(post);

                return RedirectToAction("MyPosts");
            }
            catch (Exception ex)
            {
                return View(post);
            }
        }

        // GET: PostController/Delete
        public ActionResult Delete(int id)
        {
            
            Post post = _postRepository.GetPublishedPostById(id);
            if (post == null || post.UserProfileId != GetCurrentUserProfileId())
            {
                return NotFound();
            }
            return View(post);
        }

        // POST: PostController/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Post post)
        {
            
            try
            {
                _postRepository.DeletePost(id);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(post);
            }
        }
    }
}
