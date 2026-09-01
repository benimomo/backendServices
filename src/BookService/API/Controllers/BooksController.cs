using Microsoft.AspNetCore.Mvc;
using LibraryApi.Application.DTOs;
using LibraryApi.Application.Services;

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
    public async Task<ActionResult<IEnumerable<BookDto>>> GetAll()
    {
        var books = await _bookService.GetAllBooksAsync();
        return Ok(books);
    }

    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookDto>> GetById(int id)
    {
        var book = await _bookService.GetBookByIdAsync(id);
        if (book is null)
            return NotFound(new { message = $"کتابی با شناسه {id} یافت نشد." });

        return Ok(book);
    }

    
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<BookDto>>> Search([FromQuery] string query)
    {
        var books = await _bookService.SearchBooksAsync(query);
        return Ok(books);
    }

    
    [HttpPost]
    public async Task<ActionResult<BookDto>> Create([FromBody] CreateBookDto dto)
    {
        var createdBook = await _bookService.CreateBookAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = createdBook.Id }, createdBook);
    }

    
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBookDto dto)
    {
        var updated = await _bookService.UpdateBookAsync(id, dto);
        if (!updated)
            return NotFound(new { message = $"کتابی با شناسه {id} برای ویرایش پیدا نشد." });

        return NoContent();
    }

    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _bookService.DeleteBookAsync(id);
        if (!deleted)
            return NotFound(new { message = $"کتابی با شناسه {id} برای حذف پیدا نشد." });

        return NoContent();
    }
}