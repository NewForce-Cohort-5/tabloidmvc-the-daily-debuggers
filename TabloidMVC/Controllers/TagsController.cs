using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TabloidMVC.Repositories;
using TabloidMVC.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace TabloidMVC.Controllers
{
    [Authorize]
    
    // Put in a quick fix for only allowing admins access to tags
    public class TagsController : Controller
    {
        private readonly ITagRepository _tagRepo;
        private readonly IUserProfileRepository _userRepo;
        public TagsController(ITagRepository tagRepo, IUserProfileRepository userRepo)
        {
            _tagRepo = tagRepo;
            _userRepo = userRepo;
        }

        // GET: TagsController
        public ActionResult Index()
        {
            string userRole = GetUserRole();
            if(userRole != "Admin")
            {
                return Redirect(Request.Headers["referer"].ToString());
            }
            List<Tag> tags = _tagRepo.GetAllTags();
            return View(tags);
        }

        // GET: TagsController/Details/5
        public ActionResult Details(int id)
        {
            string userRole = GetUserRole();
            if (userRole != "Admin")
            {
                return Redirect(Request.Headers["referer"].ToString());
            }
            return View();
        }

        // GET: TagsController/Create
        public ActionResult Create()
        {
            string userRole = GetUserRole();
            if (userRole != "Admin")
            {
                return Redirect(Request.Headers["referer"].ToString());
            }
            return View(new Tag());
        }

        // POST: TagsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tag tag)
        {
            string userRole = GetUserRole();
            if (userRole != "Admin")
            {
                return Redirect(Request.Headers["referer"].ToString());
            }
            try
            {
                _tagRepo.AddTag(tag);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(tag);
            }
        }

        // GET: TagsController/Edit/5
        public ActionResult Edit(int id)
        {
            string userRole = GetUserRole();
            if (userRole != "Admin")
            {
                return Redirect(Request.Headers["referer"].ToString());
            }
            Tag tag = _tagRepo.GetTagById(id);
            return View(tag);
        }

        // POST: TagsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Tag tag)
        {
            string userRole = GetUserRole();
            if (userRole != "Admin")
            {
                return Redirect(Request.Headers["referer"].ToString());
            }
            try
            {
                _tagRepo.UpdateTag(tag);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(tag);
            }
        }

        // GET: TagsController/Delete/5
        public ActionResult Delete(int id)
        {
            string userRole = GetUserRole();
            if (userRole != "Admin")
            {
                return Redirect(Request.Headers["referer"].ToString());
            }
            Tag tag = _tagRepo.GetTagById(id);
            return View(tag);
        }

        // POST: TagsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Tag tag)
        {
            string userRole = GetUserRole();
            if (userRole != "Admin")
            {
                return Redirect(Request.Headers["referer"].ToString());
            }
            try
            {
                _tagRepo.DeleteTag(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(tag);
            }
        }

        private string GetUserRole()
        {
            return _userRepo.GetByEmail(User.FindFirstValue(ClaimTypes.Email)).UserType.Name;
        }
    }
}
