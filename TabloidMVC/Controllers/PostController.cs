using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Security.Claims;
using TabloidMVC.Models;
using TabloidMVC.Models.ViewModels;
using TabloidMVC.Repositories;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace TabloidMVC.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITagRepository _tagRepository;

        public PostController(IPostRepository postRepository, ICategoryRepository categoryRepository, ITagRepository tagRepository)
        {
            _postRepository = postRepository;
            _categoryRepository = categoryRepository;
            _tagRepository = tagRepository;
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

                return RedirectToAction("Index");
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

        public ActionResult ManageTags(int id)
        {
            Post post = _postRepository.GetPublishedPostById(id);
            List<Tag> postTags = _tagRepository.GetTagsByPostId(id);
            List<Tag> tags = _tagRepository.GetAllTags();

            PostTagViewModel ptvm = new PostTagViewModel()
            {
                Post = post,
                PostTags = postTags,
                Tags = tags
            };

            return View(ptvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult ManageTags(int id, IFormCollection formCollection)
        {
            try
            {
                var selectedTags = formCollection.Select(tag => tag.Key.Substring())
                return View();
            }
            catch(Exception ex)
            {
                Post post = _postRepository.GetPublishedPostById(id);
                List<Tag> postTags = _tagRepository.GetTagsByPostId(id);
                List<Tag> tags = _tagRepository.GetAllTags();

                PostTagViewModel ptvm = new PostTagViewModel()
                {
                    Post = post,
                    PostTags = postTags,
                    Tags = tags
                };

                return View(ptvm);
            }
        }
    }
}
