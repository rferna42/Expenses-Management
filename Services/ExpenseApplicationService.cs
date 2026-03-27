using FinanceProject.Domain.Repositories;
using FinanceProject.Models;

namespace FinanceProject.Services;

public class ExpenseApplicationService
{
    private readonly IExpenseRepository _expenseRepository;

    public ExpenseApplicationService(IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
    }

    public List<Expense> GetAllExpenses()
    {
        return _expenseRepository.Load();
    }

    public void AddExpense(Expense expense)
    {
        _expenseRepository.Upsert(expense);
    }

    public bool UpdateExpense(Expense expense)
    {
        if (!_expenseRepository.ExistsById(expense.Id))
        {
            return false;
        }

        _expenseRepository.Upsert(expense);
        return true;
    }

    public bool DeleteExpense(string expenseId)
    {
        if (!_expenseRepository.ExistsById(expenseId))
        {
            return false;
        }

        _expenseRepository.DeleteById(expenseId);
        return true;
    }
}
