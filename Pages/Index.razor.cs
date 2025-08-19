using Microsoft.AspNetCore.Components;
using PersonalAccountant.Services;
using PersonalAccountant.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalAccountant.Pages
{
    public partial class Index : ComponentBase
    {
        [Inject]
        private IFinanceService FinanceService { get; set; }

        [Inject]
        private ICurrencyService CurrencyService { get; set; }

        // Data
        private List<Expense>? _expenses;
        private decimal _budget;
        private decimal _totalExpenses;
        private decimal _grossSpent;

        // UI State
        private bool _showAddExpenseModal = false;
        private bool _showAddIncomeModal = false;
        private bool _isEditingBudget = false;
        private bool _showClearAllConfirmation = false;
        private bool _showEditModal = false;
        private decimal _budgetInput;
        private Expense _newExpense = new();
        private Expense _newIncome = new();
        private Expense _transactionToEdit = new();
        private ExpenseCategory? _expandedCategory;

        // State for date filtering
        private DateTime _selectedDate = DateTime.Today;
        private bool _isDatePickerVisible = false;

        // State for currency conversion
        private decimal _remainingBudgetInEur;
        private bool _isLoadingCurrency = true;
        private decimal _cachedUahToEurRate;

        // Category display mapping
        private readonly Dictionary<ExpenseCategory, (string Name, string Icon)> _categoryDisplayInfo = new()
        {
            { ExpenseCategory.Income, ("Income", "💰") },
            { ExpenseCategory.Groceries, ("Groceries", "🛒") },
            { ExpenseCategory.Transportation, ("Transportation", "🚗") },
            { ExpenseCategory.MonthlyBills, ("Monthly Bills", "🧾") },
            { ExpenseCategory.Subscriptions, ("Subscriptions", "🔄") },
            { ExpenseCategory.RestaurantsAndCafes, ("Restaurants & Cafes", "☕") },
            { ExpenseCategory.Beauty, ("Beauty", "💅") },
            { ExpenseCategory.Unplanned, ("Unplanned", "💥") }
        };

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
            await LoadCurrencyConversion();
        }

        private async Task LoadData()
        {
            _budget = await FinanceService.GetBudgetAsync();
            _budgetInput = _budget;
            _expenses = await FinanceService.GetExpensesAsync();

            _totalExpenses = _expenses?.Sum(e => e.Amount) ?? 0;
            _grossSpent = _expenses?.Where(e => e.Amount > 0).Sum(e => e.Amount) ?? 0;
        }

        private async Task LoadCurrencyConversion()
        {
            _isLoadingCurrency = true;
            _remainingBudgetInEur = 0;

            if (_cachedUahToEurRate == 0)
            {
                _cachedUahToEurRate = await CurrencyService.GetExchangeRateAsync("UAH", "EUR");
            }

            CalculateRemainingEur();
            _isLoadingCurrency = false;
            StateHasChanged();
        }

        private void CalculateRemainingEur()
        {
            _remainingBudgetInEur = 0;
            var remainingUah = _budget - _totalExpenses;
            if (remainingUah > 0 && _cachedUahToEurRate > 0)
            {
                _remainingBudgetInEur = remainingUah * _cachedUahToEurRate;
            }
        }

        private void OpenAddExpenseModal(ExpenseCategory category)
        {
            _newExpense = new Expense { Category = category, Date = _selectedDate };
            _showAddExpenseModal = true;
        }

        private void CloseModal()
        {
            _showAddExpenseModal = false;
        }

        private async Task HandleAddExpense()
        {
            _newExpense.Amount = Math.Abs(_newExpense.Amount);
            await FinanceService.AddExpenseAsync(_newExpense);
            CloseModal();
            await LoadData();
            CalculateRemainingEur();
        }

        private void OpenAddIncomeModal()
        {
            _newIncome = new Expense { Date = _selectedDate };
            _showAddIncomeModal = true;
        }

        private async Task HandleAddIncome()
        {
            _newIncome.Amount = -Math.Abs(_newIncome.Amount);
            _newIncome.Category = ExpenseCategory.Income;

            await FinanceService.AddExpenseAsync(_newIncome);
            _showAddIncomeModal = false;
            await LoadData();
            CalculateRemainingEur();
        }

        private async Task HandleSetBudget()
        {
            await FinanceService.SetBudgetAsync(_budgetInput);
            _isEditingBudget = false;
            await LoadData();
            CalculateRemainingEur();
        }

        private void ToggleCategoryExpansion(ExpenseCategory category)
        {
            if (_expandedCategory == category)
            {
                _expandedCategory = null;
            }
            else
            {
                _expandedCategory = category;
            }
        }

        private async Task HandleClearAllDataAsync()
        {
            await FinanceService.ClearAllDataAsync();
            _showClearAllConfirmation = false;
            await LoadData();
            CalculateRemainingEur();
        }

        private void OpenEditModal(Expense transaction)
        {
            _transactionToEdit = new Expense
            {
                Id = transaction.Id,
                Amount = Math.Abs(transaction.Amount),
                Description = transaction.Description,
                Category = transaction.Category,
                Date = transaction.Date
            };
            _showEditModal = true;
        }

        private async Task HandleUpdateTransactionAsync()
        {
            var transaction = _transactionToEdit;

            if (transaction.Category == ExpenseCategory.Income)
            {
                transaction.Amount = -Math.Abs(transaction.Amount);
            }
            else
            {
                transaction.Amount = Math.Abs(transaction.Amount);
            }

            await FinanceService.UpdateTransactionAsync(transaction);
            _showEditModal = false;
            await LoadData();
            CalculateRemainingEur();
        }

        private async Task HandleDeleteTransactionAsync()
        {
            await FinanceService.DeleteTransactionAsync(_transactionToEdit.Id);
            _showEditModal = false;
            await LoadData();
            CalculateRemainingEur();
        }
    }
}