namespace RadiologyCenter.Examinations.Application.Abstractions;

public record ExaminationFeeResolution(
    decimal? RadiologistFee,
    decimal? TechnicianFee,
    decimal? ReferralFee);
