using LibraryApi.Application.DTOs;
using LibraryApi.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Constants;

namespace LibraryApi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var books = await _bookService.GetAllBooksAsync();

        if (!books.Any())
        {
            return Ok(new ApiResponse<IEnumerable<BookDto>>
            {
                Success = true,
                Message = BookMessages.NoBooksAvailable,
                Data = books
            });
        }

        return Ok(new ApiResponse<IEnumerable<BookDto>>
        {
            Success = true,
            Message = BookMessages.BooksRetrieved,
            Data = books
        });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var book = await _bookService.GetBookByIdAsync(id);
        if (book is null)
        {
            return NotFound(new ApiResponse<BookDto>
            {
                Success = false,
                Message = BookMessages.BookNotFound,
                ErrorCode = "BOOK_001"
            });
        }

        return Ok(new ApiResponse<BookDto>
        {
            Success = true,
            Message = BookMessages.BookRetrieved,
            Data = book
        });
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query)
    {
        var books = await _bookService.SearchBooksAsync(query);

        if (!books.Any())
        {
            return Ok(new ApiResponse<IEnumerable<BookDto>>
            {
                Success = true,
                Message = BookMessages.NoBooksAvailable,
                Data = books
            });
        }

        return Ok(new ApiResponse<IEnumerable<BookDto>>
        {
            Success = true,
            Message = BookMessages.BooksRetrieved,
            Data = books
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookDto dto)
    {
        var createdBook = await _bookService.CreateBookAsync(dto);

        return CreatedAtAction(nameof(GetById), new { id = createdBook.Id }, new ApiResponse<BookDto>
        {
            Success = true,
            Message = BookMessages.BookCreatedSuccessfully,
            Data = createdBook
        });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBookDto dto)
    {
        var updated = await _bookService.UpdateBookAsync(id, dto);
        if (!updated)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = BookMessages.BookNotFound,
                ErrorCode = "BOOK_001"
            });
        }

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = BookMessages.BookUpdatedSuccessfully
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _bookService.DeleteBookAsync(id);
        if (!deleted)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = BookMessages.BookNotFound,
                ErrorCode = "BOOK_001"
            });
        }

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = BookMessages.BookDeletedSuccessfully
        });
    }
}