using LibraryApi.Application.DTOs;
using LibraryApi.Domain.Models;
using LibraryApi.Infrastructure.Repositories;

namespace LibraryApi.Application.Services;

public class BookManager : IBookService
{
    private readonly IBookRepository _repository;

    public BookManager(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<BookDto>> GetAllBooksAsync()
    {
        var books = await _repository.GetAllAsync();
        return books.Select(MapToDto);
    }

    public async Task<BookDto?> GetBookByIdAsync(int id)
    {
        var book = await _repository.GetByIdAsync(id);
        return book is null ? null : MapToDto(book);
    }

    public async Task<IEnumerable<BookDto>> SearchBooksAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return await GetAllBooksAsync();

        var books = await _repository.SearchAsync(query);
        return books.Select(MapToDto);
    }

    public async Task<BookDto> CreateBookAsync(CreateBookDto dto)
    {
        var book = new Book
        {
            Title = dto.Title.Trim(),
            Author = dto.Author.Trim(),
            PublishedYear = dto.PublishedYear,
            Price = dto.Price
        };

        var created = await _repository.AddAsync(book);
        return MapToDto(created);
    }

    public async Task<bool> UpdateBookAsync(int id, UpdateBookDto dto)
    {
        var book = await _repository.GetByIdAsync(id);
        if (book is null) return false;

        book.Title = dto.Title.Trim();
        book.Author = dto.Author.Trim();
        book.PublishedYear = dto.PublishedYear;
        book.Price = dto.Price;

        await _repository.UpdateAsync(book);
        return true;
    }

    public async Task<bool> DeleteBookAsync(int id)
    {
        var book = await _repository.GetByIdAsync(id);
        if (book is null) return false;

        await _repository.DeleteAsync(book);
        return true;
    }

    private static BookDto MapToDto(Book book) => new()
    {
        Id = book.Id,
        Title = book.Title,
        Author = book.Author,
        PublishedYear = book.PublishedYear,
        Price = book.Price
    };
}