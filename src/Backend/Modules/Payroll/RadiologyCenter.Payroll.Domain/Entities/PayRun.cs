using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Exceptions;
using RadiologyCenter.BuildingBlocks.Domain.SoftDeletable;
using RadiologyCenter.Payroll.Domain.Enumerations;
using RadiologyCenter.Payroll.Domain.Errors;
using RadiologyCenter.Payroll.Domain.Events;

namespace RadiologyCenter.Payroll.Domain.Entities;

public sealed class PayRun : SoftDeletableAggregateRoot<Guid>
{
    private readonly List<Payslip> _payslips = [];
    private readonly List<ReferralFeeStatement> _referralFeeStatements = [];

    public DateTime RunFrom { get; private set; }
    public DateTime RunTo { get; private set; }
    public PayRunStatus Status { get; private set; }
    public string? ProcessedBy { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public string? Notes { get; private set; }
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    public IReadOnlyCollection<Payslip> Payslips => _payslips.AsReadOnly();
    public IReadOnlyCollection<ReferralFeeStatement> ReferralFeeStatements => _referralFeeStatements.AsReadOnly();

    private PayRun()
    {
        Status = null!;
    }

    public static PayRun Create(DateTime runFrom, DateTime runTo, string? notes = null)
    {
        Guard.AgainstDefault(runFrom, nameof(runFrom));
        Guard.AgainstDefault(runTo, nameof(runTo));
        Guard.Against(runTo, d => d < runFrom, DomainErrors.RunToBeforeRunFrom, "RunTo cannot be before RunFrom.");

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
            throw new BusinessRuleViolationException(
                nameof(AddPayslip),
                DomainErrors.DuplicatePayslip,
                "This staff member already has a payslip in this pay run.");

        var payslip = Payslip.Create(Id, staffId, grossSalary, unpaidLeaveDays, unpaidLeaveDeduction, notes);
        _payslips.Add(payslip);
        return payslip;
    }

    public void RemovePayslip(Guid staffId)
    {
        EnsureEditable();
        var payslip = _payslips.FirstOrDefault(p => p.StaffId == staffId)
            ?? throw new DomainException(DomainErrors.PayslipNotFound, "This staff member has no payslip in this pay run.");
        _payslips.Remove(payslip);
    }

    public Payslip SetPayslipDraft(
        Guid staffId,
        decimal baseSalary,
        int unpaidLeaveDays,
        decimal unpaidLeaveDeduction,
        IReadOnlyList<(string Name, decimal Amount, bool IsDeduction)> components)
    {
        if (TryGetPayslip(staffId, out var existing))
            RemovePayslip(staffId);

        var payslip = AddPayslip(staffId, baseSalary, unpaidLeaveDays, unpaidLeaveDeduction);
        foreach (var (name, amount, isDeduction) in components)
            payslip.AddComponent(name, amount, isDeduction);

        return payslip;
    }

    public ReferralFeeStatement AddReferralFeeStatement(
        Guid referralDoctorId,
        decimal totalFee,
        int examCount)
    {
        EnsureEditable();
        if (_referralFeeStatements.Any(s => s.ReferralDoctorId == referralDoctorId))
            throw new BusinessRuleViolationException(
                nameof(AddReferralFeeStatement),
                DomainErrors.DuplicateReferralFeeStatement,
                "This referral doctor already has a statement in this pay run.");

        var statement = ReferralFeeStatement.Create(Id, referralDoctorId, totalFee, examCount);
        _referralFeeStatements.Add(statement);
        return statement;
    }

    private bool TryGetPayslip(Guid staffId, out Payslip payslip)
    {
        payslip = _payslips.FirstOrDefault(p => p.StaffId == staffId)!;
        return payslip is not null;
    }

    public void Compute(string? by = null)
    {
        EnsureStatus(PayRunStatus.Draft, PayRunStatus.Computed);
        Status = PayRunStatus.Computed;
        ProcessedBy = by;
        ProcessedAt = DateTime.UtcNow;
        RaiseDomainEvent(new PayRunComputedEvent(Id, by));
    }

    public void Approve(string? by = null)
    {
        EnsureStatus(PayRunStatus.Computed, PayRunStatus.Approved);
        Status = PayRunStatus.Approved;
        ProcessedBy = by;
        ProcessedAt = DateTime.UtcNow;
        RaiseDomainEvent(new PayRunApprovedEvent(Id, by));
    }

    public void Reject(string? by = null)
    {
        EnsureStatus(PayRunStatus.Computed, PayRunStatus.Approved, PayRunStatus.Rejected);
        Status = PayRunStatus.Rejected;
        ProcessedBy = by;
        ProcessedAt = DateTime.UtcNow;
        RaiseDomainEvent(new PayRunRejectedEvent(Id, by));
    }

    public void Restart(string? by = null)
    {
        EnsureStatus(PayRunStatus.Rejected);
        Status = PayRunStatus.Computed;
        ProcessedBy = by;
        ProcessedAt = DateTime.UtcNow;
        RaiseDomainEvent(new PayRunComputedEvent(Id, by));
    }

    public void Pay(string? by = null)
    {
        EnsureStatus(PayRunStatus.Approved);
        Status = PayRunStatus.Paid;
        ProcessedBy = by;
        ProcessedAt = DateTime.UtcNow;
        RaiseDomainEvent(new PayRunPaidEvent(Id, by));
    }

    private void EnsureEditable()
    {
        if (Status != PayRunStatus.Draft)
            throw new BusinessRuleViolationException(
                nameof(EnsureEditable),
                DomainErrors.PayRunNotEditable,
                $"Pay run is {Status.Name} and cannot be modified.");
    }

    private void EnsureStatus(params PayRunStatus[] allowed)
    {
        if (!allowed.Contains(Status))
            throw new BusinessRuleViolationException(
                nameof(EnsureStatus),
                DomainErrors.InvalidPayRunTransition,
                $"Pay run is {Status.Name} and cannot transition to this state.");
    }
}
