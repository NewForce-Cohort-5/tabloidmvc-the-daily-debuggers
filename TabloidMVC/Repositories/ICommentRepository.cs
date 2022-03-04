using System.Collections.Generic;
using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public interface ICommentRepository
    {
        public void Add(Comment comment);
        public List<Comment> GetAllCommentsByPostId(int id);


    }
}
