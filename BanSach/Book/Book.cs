using System.ComponentModel.DataAnnotations;

namespace BanSach.Book;

public class Book
{
    [Key]
    public int Id { get; set; }

    [Required]
    public required string Title { get; set; }
    
    public string? Author { get; set; }

    [Required]
    public required decimal Price { get; set; }

    public string? Description { get; set; }

    [Required]
    public required int StockQuantity { get; set; }
}