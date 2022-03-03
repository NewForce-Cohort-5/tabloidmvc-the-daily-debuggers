using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TabloidMVC.Repositories;
using TabloidMVC.Models;
using System.Collections.Generic;

namespace TabloidMVC.Controllers
{
    [Authorize]
    public class TagsController : Controller
    {
        private readonly ITagRepository _tagRepo;
        public TagsController(ITagRepository tagRepo)
        {
            _tagRepo = tagRepo;
        }

        // GET: TagsController
        public ActionResult Index()
        {
            List<Tag> tags = _tagRepo.GetAllTags();
            return View(tags);
        }

        // GET: TagsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TagsController/Create
        public ActionResult Create()
        {
            return View(new Tag());
        }

        // POST: TagsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tag tag)
        {
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
            Tag tag = _tagRepo.GetTagById(id);
            return View();
        }

        // POST: TagsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Tag tag)
        {
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
            return View();
        }

        // POST: TagsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
