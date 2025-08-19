using System;
using System.ComponentModel.DataAnnotations;

namespace PersonalAccountant.Shared;

// Enum for fixed categories makes our code safer and cleaner
public enum ExpenseCategory
{
    Income,
    Transportation,
    Groceries,
    MonthlyBills,
    Subscriptions,
    RestaurantsAndCafes,
    Beauty,
    Unplanned
}

public class Expense
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be positive.")]
    public decimal Amount { get; set; }

    public string? Description { get; set; }

    [Required]
    public ExpenseCategory Category { get; set; }

    public DateTime Date { get; set; } = DateTime.UtcNow;
}