using FinanceProject.Domain.Repositories;

namespace FinanceProject.Services;

public class CategoryApplicationService
{
    public const string EmptyNameErrorKey = "category_error_empty";
    public const string DuplicateErrorKey = "category_error_duplicate";
    public const string NotFoundErrorKey = "category_error_not_found";
    public const string LastCategoryErrorKey = "category_error_last";

    private readonly ICategoryRepository _categoryRepository;

    public CategoryApplicationService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public List<string> GetAllCategories()
    {
        return _categoryRepository.LoadCategories();
    }

    public bool AddCategory(string categoryName, out string? errorKey)
    {
        var normalizedName = categoryName.Trim();

        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            errorKey = EmptyNameErrorKey;
            return false;
        }

        if (_categoryRepository.CategoryExists(normalizedName))
        {
            errorKey = DuplicateErrorKey;
            return false;
        }

        _categoryRepository.AddCategory(normalizedName);
        errorKey = null;
        return true;
    }

    public bool RenameCategory(string currentName, string newName, out string? errorKey)
    {
        var normalizedCurrent = currentName.Trim();
        var normalizedNew = newName.Trim();

        if (string.IsNullOrWhiteSpace(normalizedCurrent) || string.IsNullOrWhiteSpace(normalizedNew))
        {
            errorKey = EmptyNameErrorKey;
            return false;
        }

        if (string.Equals(normalizedCurrent, normalizedNew, StringComparison.OrdinalIgnoreCase))
        {
            errorKey = null;
            return true;
        }

        if (_categoryRepository.CategoryExists(normalizedNew))
        {
            errorKey = DuplicateErrorKey;
            return false;
        }

        if (!_categoryRepository.RenameCategory(normalizedCurrent, normalizedNew))
        {
            errorKey = NotFoundErrorKey;
            return false;
        }

        errorKey = null;
        return true;
    }

    public bool DeleteCategory(string categoryName, out string? errorKey)
    {
        var normalizedName = categoryName.Trim();

        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            errorKey = EmptyNameErrorKey;
            return false;
        }

        if (!_categoryRepository.DeleteCategory(normalizedName))
        {
            errorKey = LastCategoryErrorKey;
            return false;
        }

        errorKey = null;
        return true;
    }
}
