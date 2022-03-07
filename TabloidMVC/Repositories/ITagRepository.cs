using System.Collections.Generic;
using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public interface ITagRepository
    {
        public List<Tag> GetAllTags();
        public List<Tag> GetTagsByPostId(int id);
        public Tag GetTagById(int id);
        public void AddTag(Tag tag);
        public void UpdateTag(Tag tag);
        public void DeleteTag(int id);
    }
}
