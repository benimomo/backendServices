using LibraryApi.Application.DTOs;

namespace LibraryApi.Application.Services;

public interface IBookService
{
    Task<IEnumerable<BookDto>> GetAllBooksAsync();
    Task<BookDto?> GetBookByIdAsync(int id);
    Task<IEnumerable<BookDto>> SearchBooksAsync(string query);
    Task<BookDto> CreateBookAsync(CreateBookDto dto);
    Task<bool> UpdateBookAsync(int id, UpdateBookDto dto);
    Task<bool> DeleteBookAsync(int id);
}