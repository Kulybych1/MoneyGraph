namespace PersonalAccountant.Services;

using PersonalAccountant.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IFinanceService
{
    Task AddExpenseAsync(Expense expense);
    Task<List<Expense>> GetExpensesAsync();
    Task SetBudgetAsync(decimal amount);
    Task<decimal> GetBudgetAsync();

    // New methods to add
    Task UpdateTransactionAsync(Expense transaction);

    Task ClearAllDataAsync();

    Task DeleteTransactionAsync(Guid transactionId);
}