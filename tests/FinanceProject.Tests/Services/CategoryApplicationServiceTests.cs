using FinanceProject.Domain.Repositories;
using FinanceProject.Services;

namespace FinanceProject.Tests.Services;

public class CategoryApplicationServiceTests
{
    [Fact]
    public void AddCategory_ReturnsFalse_WhenNameIsEmpty()
    {
        var repository = new FakeCategoryRepository();
        var service = new CategoryApplicationService(repository);

        var result = service.AddCategory("   ", out var errorKey);

        Assert.False(result);
        Assert.Equal(CategoryApplicationService.EmptyNameErrorKey, errorKey);
    }

    [Fact]
    public void AddCategory_ReturnsFalse_WhenCategoryAlreadyExists()
    {
        var repository = new FakeCategoryRepository();
        repository.Categories.Add("Compras");
        var service = new CategoryApplicationService(repository);

        var result = service.AddCategory("Compras", out var errorKey);

        Assert.False(result);
        Assert.Equal(CategoryApplicationService.DuplicateErrorKey, errorKey);
    }

    [Fact]
    public void AddCategory_AddsCategory_WhenValid()
    {
        var repository = new FakeCategoryRepository();
        var service = new CategoryApplicationService(repository);

        var result = service.AddCategory("Mascotas", out var errorKey);

        Assert.True(result);
        Assert.Null(errorKey);
        Assert.Contains("Mascotas", repository.Categories);
    }

    [Fact]
    public void RenameCategory_ReturnsFalse_WhenNewNameAlreadyExists()
    {
        var repository = new FakeCategoryRepository();
        repository.Categories.AddRange(["Hogar", "Viajes"]);
        var service = new CategoryApplicationService(repository);

        var result = service.RenameCategory("Hogar", "Viajes", out var errorKey);

        Assert.False(result);
        Assert.Equal(CategoryApplicationService.DuplicateErrorKey, errorKey);
    }

    [Fact]
    public void RenameCategory_RenamesCategory_WhenValid()
    {
        var repository = new FakeCategoryRepository();
        repository.Categories.Add("Hogar");
        var service = new CategoryApplicationService(repository);

        var result = service.RenameCategory("Hogar", "Casa", out var errorKey);

        Assert.True(result);
        Assert.Null(errorKey);
        Assert.Contains("Casa", repository.Categories);
        Assert.DoesNotContain("Hogar", repository.Categories);
    }

    [Fact]
    public void DeleteCategory_ReturnsFalse_WhenRepositoryBlocksDeletion()
    {
        var repository = new FakeCategoryRepository { DeleteCategoryResult = false };
        repository.Categories.Add("Unica");
        var service = new CategoryApplicationService(repository);

        var result = service.DeleteCategory("Unica", out var errorKey);

        Assert.False(result);
        Assert.Equal(CategoryApplicationService.LastCategoryErrorKey, errorKey);
    }

    [Fact]
    public void DeleteCategory_Deletes_WhenAllowed()
    {
        var repository = new FakeCategoryRepository();
        repository.Categories.AddRange(["A", "B"]);
        var service = new CategoryApplicationService(repository);

        var result = service.DeleteCategory("A", out var errorKey);

        Assert.True(result);
        Assert.Null(errorKey);
        Assert.DoesNotContain("A", repository.Categories);
    }

    private sealed class FakeCategoryRepository : ICategoryRepository
    {
        public List<string> Categories { get; } = [];
        public bool DeleteCategoryResult { get; set; } = true;

        public List<string> LoadCategories() => Categories.ToList();

        public bool CategoryExists(string categoryName)
        {
            return Categories.Any(category =>
                string.Equals(category, categoryName, StringComparison.OrdinalIgnoreCase));
        }

        public void AddCategory(string categoryName)
        {
            Categories.Add(categoryName.Trim());
        }

        public bool RenameCategory(string currentName, string newName)
        {
            var index = Categories.FindIndex(category =>
                string.Equals(category, currentName, StringComparison.OrdinalIgnoreCase));
            if (index < 0)
            {
                return false;
            }

            Categories[index] = newName.Trim();
            return true;
        }

        public bool DeleteCategory(string categoryName)
        {
            if (!DeleteCategoryResult)
            {
                return false;
            }

            var removed = Categories.RemoveAll(category =>
                string.Equals(category, categoryName, StringComparison.OrdinalIgnoreCase));
            return removed > 0;
        }
    }
}
