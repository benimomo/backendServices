using System.ComponentModel.DataAnnotations;

namespace LibraryApi.Application.DTOs;
public class CreateBookDto
{
    [Required(ErrorMessage = "عنوان کتاب الزامی است.")]
    [StringLength(200, ErrorMessage = "عنوان کتاب نمی‌تواند بیشتر از 200 کاراکتر باشد.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "نام نویسنده الزامی است.")]
    [StringLength(100, ErrorMessage = "نام نویسنده نمی‌تواند بیشتر از 100 کاراکتر باشد.")]
    public string Author { get; set; } = string.Empty;

    [Range(1000, 2100, ErrorMessage = "سال انتشار باید یک مقدار منطقی باشد.")]
    public int PublishedYear { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "قیمت باید بزرگ‌تر از صفر باشد.")]
    public decimal Price { get; set; }
}