namespace FinanceProject.Domain.Repositories;

public interface ICategoryRepository
{
    List<string> LoadCategories();
    bool CategoryExists(string categoryName);
    void AddCategory(string categoryName);
    bool RenameCategory(string currentName, string newName);
    bool DeleteCategory(string categoryName);
}
