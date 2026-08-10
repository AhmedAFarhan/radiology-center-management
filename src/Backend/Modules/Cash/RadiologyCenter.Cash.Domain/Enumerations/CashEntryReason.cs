using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.Cash.Domain.Enumerations;

public sealed class CashEntryReason : Enumeration
{
    public static readonly CashEntryReason Opening = new(1, "Opening");
    public static readonly CashEntryReason Payment = new(2, "Payment");
    public static readonly CashEntryReason Refund = new(3, "Refund");
    public static readonly CashEntryReason Deposit = new(4, "Deposit");
    public static readonly CashEntryReason Payout = new(5, "Payout");
    public static readonly CashEntryReason Adjustment = new(6, "Adjustment");

    private CashEntryReason(int value, string name) : base(value, name) { }
}