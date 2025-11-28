using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Application.DTOs
{
    public class CategoryDto
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Picture { get; set; }
    }

    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(15, MinimumLength = 1, ErrorMessage = "Category name must be between 1 and 15 characters")]
        public string CategoryName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description must not exceed 500 characters")]
        public string? Description { get; set; }

        [Url(ErrorMessage = "Picture must be a valid URL")]
        [StringLength(500, ErrorMessage = "Picture URL must not exceed 500 characters")]
        public string? Picture { get; set; }
    }
}
