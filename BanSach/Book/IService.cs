using BanSach.Book;
using BookEntity = BanSach.Book.Book;

namespace BanSach.Services
{
    public interface IBookService
    {
        Task<List<BookEntity>> GetAllAsync();
        Task<BookEntity?> GetByIdAsync(int id);
    }
}