using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.Insurance.Domain.Enumerations;

public sealed class SettlementMethod : Enumeration
{
    public static readonly SettlementMethod Cash = new(1, "Cash");
    public static readonly SettlementMethod BankTransfer = new(2, "BankTransfer");
    public static readonly SettlementMethod Cheque = new(3, "Cheque");
    public static readonly SettlementMethod Electronic = new(4, "Electronic");

    private SettlementMethod(int value, string name) : base(value, name) { }
}