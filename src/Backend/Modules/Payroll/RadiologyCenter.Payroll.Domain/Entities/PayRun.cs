using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Exceptions;
using RadiologyCenter.BuildingBlocks.Domain.SoftDeletable;
using RadiologyCenter.Payroll.Domain.Enumerations;

namespace RadiologyCenter.Payroll.Domain.Entities;

public sealed class PayRun : SoftDeletableAggregateRoot<Guid>
{
    private readonly List<Payslip> _payslips = [];

    public DateTime RunFrom { get; private set; }
    public DateTime RunTo { get; private set; }
    public PayRunStatus Status { get; private set; }
    public string? ProcessedBy { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public string? Notes { get; private set; }

    public IReadOnlyCollection<Payslip> Payslips => _payslips.AsReadOnly();

    private PayRun()
    {
        Status = null!;
    }

    public static PayRun Create(DateTime runFrom, DateTime runTo, string? notes = null)
    {
        Guard.AgainstDefault(runFrom, nameof(runFrom));
        Guard.AgainstDefault(runTo, nameof(runTo));
        Guard.Against(runTo, d => d < runFrom, "RunTo cannot be before RunFrom.");

        return new PayRun
        {
            Id = Guid.NewGuid(),
            RunFrom = runFrom,
            RunTo = runTo,
            Status = PayRunStatus.Draft,
            Notes = notes?.Trim()
        };
    }

    public Payslip AddPayslip(
        Guid staffId,
        decimal grossSalary,
        int unpaidLeaveDays = 0,
        decimal unpaidLeaveDeduction = 0,
        string? notes = null)
    {
        EnsureEditable();
        if (_payslips.Any(p => p.StaffId == staffId))
            throw new DomainException($"Staff '{staffId}' already has a payslip in pay run '{Id}'.");

        var payslip = Payslip.Create(Id, staffId, grossSalary, unpaidLeaveDays, unpaidLeaveDeduction, notes);
        _payslips.Add(payslip);
        return payslip;
    }

    public void RemovePayslip(Guid staffId)
    {
        EnsureEditable();
        var payslip = _payslips.FirstOrDefault(p => p.StaffId == staffId)
            ?? throw new DomainException($"Staff '{staffId}' has no payslip in pay run '{Id}'.");
        _payslips.Remove(payslip);
    }

    public void Compute(string? by = null)
    {
        EnsureStatus(PayRunStatus.Draft, PayRunStatus.Computed);
        Status = PayRunStatus.Computed;
        ProcessedBy = by;
        ProcessedAt = DateTime.UtcNow;
    }

    public void Approve(string? by = null)
    {
        EnsureStatus(PayRunStatus.Computed, PayRunStatus.Approved);
        Status = PayRunStatus.Approved;
        ProcessedBy = by;
        ProcessedAt = DateTime.UtcNow;
    }

    public void Reject(string? by = null)
    {
        EnsureStatus(PayRunStatus.Computed, PayRunStatus.Approved, PayRunStatus.Rejected);
        Status = PayRunStatus.Rejected;
        ProcessedBy = by;
        ProcessedAt = DateTime.UtcNow;
    }

    public void Pay(string? by = null)
    {
        EnsureStatus(PayRunStatus.Approved, PayRunStatus.Paid);
        Status = PayRunStatus.Paid;
        ProcessedBy = by;
        ProcessedAt = DateTime.UtcNow;
    }

    private void EnsureEditable()
    {
        if (Status != PayRunStatus.Draft)
            throw new DomainException($"Pay run '{Id}' is {Status.Name} and cannot be modified.");
    }

    private void EnsureStatus(params PayRunStatus[] allowed)
    {
        if (!allowed.Contains(Status))
            throw new DomainException($"Pay run '{Id}' is {Status.Name} and cannot transition to this state.");
    }
}
