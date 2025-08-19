namespace PersonalAccountant.Services;

using Microsoft.JSInterop;
using System.Text.Json;
using PersonalAccountant.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// This uses a primary constructor, which is a cleaner, modern C# feature.
// The typo "IJSRruntime" has been corrected to "IJSRuntime".
public class LocalStorageFinanceService(IJSRuntime jsRuntime) : IFinanceService
{
    private const string ExpenseKey = "expenses";
    private const string BudgetKey = "budget";

    public async Task AddExpenseAsync(Expense expense)
    {
        var expenses = await GetExpensesAsync();
        expenses.Add(expense);
        await SaveToStorage(ExpenseKey, expenses);
    }

    public async Task<List<Expense>> GetExpensesAsync()
    {
        return await ReadFromStorage<List<Expense>>(ExpenseKey) ?? new List<Expense>();
    }

    public async Task SetBudgetAsync(decimal amount)
    {
        await SaveToStorage(BudgetKey, amount);
    }

    public async Task<decimal> GetBudgetAsync()
    {
        return await ReadFromStorage<decimal>(BudgetKey);
    }

    public async Task UpdateTransactionAsync(Expense transaction)
    {
        var expenses = await GetExpensesAsync();
        var transactionToUpdate = expenses.FirstOrDefault(e => e.Id == transaction.Id);
        if (transactionToUpdate != null)
        {
            transactionToUpdate.Amount = transaction.Amount;
            transactionToUpdate.Description = transaction.Description;
            transactionToUpdate.Category = transaction.Category;
            transactionToUpdate.Date = transaction.Date;
        }
        await SaveToStorage(ExpenseKey, expenses);
    }

    public async Task ClearAllDataAsync()
    {
        await jsRuntime.InvokeVoidAsync("localStorage.removeItem", ExpenseKey);
        await jsRuntime.InvokeVoidAsync("localStorage.removeItem", BudgetKey);
    }

    public async Task DeleteTransactionAsync(Guid transactionId)
    {
        var expenses = await GetExpensesAsync();
        expenses.RemoveAll(e => e.Id == transactionId);
        await SaveToStorage(ExpenseKey, expenses);
    }

    private async Task SaveToStorage<T>(string key, T value)
    {
        var json = JsonSerializer.Serialize(value);
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", key, json);
    }

    private async Task<T?> ReadFromStorage<T>(string key)
    {
        var json = await jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
        if (string.IsNullOrEmpty(json))
        {
            return default;
        }
        return JsonSerializer.Deserialize<T>(json);
    }
}