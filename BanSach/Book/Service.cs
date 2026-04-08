using BanSach.Common;
using BanSach.Book;
using Microsoft.EntityFrameworkCore;
using BookEntity = BanSach.Book.Book;

namespace BanSach.Services
{
    public class BookService : IBookService
    {
        private readonly AppDbContext _context;

        public BookService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<BookEntity>> GetAllAsync()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task<BookEntity?> GetByIdAsync(int id)
        {
            return await _context.Books.FindAsync(id);
        }
    }
}