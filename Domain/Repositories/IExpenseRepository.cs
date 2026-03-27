using FinanceProject.Models;

namespace FinanceProject.Domain.Repositories;

public interface IExpenseRepository
{
    List<Expense> Load();
    void Save(List<Expense> expenses);
}
