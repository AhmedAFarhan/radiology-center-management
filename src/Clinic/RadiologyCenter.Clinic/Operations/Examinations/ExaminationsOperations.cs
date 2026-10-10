using RadiologyCenter.Clinic.Dispatching;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Examinations.Application.Commands.AssignExaminationStaff;
using RadiologyCenter.Examinations.Application.Commands.AddExaminationItem;
using RadiologyCenter.Examinations.Application.Commands.AddExaminationTypeItem;
using RadiologyCenter.Examinations.Application.Commands.BookExamination;
using RadiologyCenter.Examinations.Application.Commands.CancelExamination;
using RadiologyCenter.Examinations.Application.Commands.CheckInExamination;
using RadiologyCenter.Examinations.Application.Commands.CompleteExamination;
using RadiologyCenter.Examinations.Application.Commands.CreateExamination;
using RadiologyCenter.Examinations.Application.Commands.RecordExaminationPayment;
using RadiologyCenter.Examinations.Application.Commands.RecordPacsImages;
using RadiologyCenter.Examinations.Application.Commands.RemoveExaminationItem;
using RadiologyCenter.Examinations.Application.Commands.RemoveExaminationTypeItem;
using RadiologyCenter.Examinations.Application.Commands.ScheduleExamination;
using RadiologyCenter.Examinations.Application.Commands.StartExamination;
using RadiologyCenter.Examinations.Application.Commands.UpdateExamination;
using RadiologyCenter.Examinations.Application.DTOs;
using RadiologyCenter.Examinations.Application.Queries.GetExaminationById;
using RadiologyCenter.Examinations.Application.Queries.GetExaminations;
using RadiologyCenter.Examinations.Application.Queries.GetExaminationsForCalendar;
using RadiologyCenter.Examinations.Application.Queries.GetAvailableSlots;
using RadiologyCenter.Examinations.Application.Queries.GetExaminationTypeItems;

using RadiologyCenter.Clinic.Extensions;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Clinic.Operations.Examinations;

public class ExaminationsOperations : ClinicOperations
{
    private readonly IMessageBus _bus;

    public ExaminationsOperations(IMessageBus bus) => _bus = bus;

    [ClinicPermission(ExaminationsReadCode)]
    [ClinicOperation("GET", "examinations/{id:guid}")]
    public async Task<ClinicResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ExaminationDto>>(new GetExaminationByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ExaminationsReadCode)]
    [ClinicOperation("POST", "examinations/all")]
    public async Task<ClinicResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<ExaminationListItemDto>>>(new GetExaminationsQuery(request), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ExaminationsCreateCode)]
    [ClinicOperation("POST", "examinations")]
    public async Task<ClinicResult> CreateAsync([FromBody] CreateExaminationCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ExaminationDto>>(command, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ExaminationsCreateCode)]
    [ClinicOperation("POST", "examinations/book")]
    public async Task<ClinicResult> BookAsync([FromBody] BookExaminationCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ExaminationDto>>(command, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ExaminationsUpdateCode)]
    [ClinicOperation("PUT", "examinations/{examinationId:guid}")]
    public async Task<ClinicResult> UpdateAsync(Guid examinationId, [FromBody] UpdateExaminationCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { ExaminationId = examinationId }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ExaminationsUpdateCode)]
    [ClinicOperation("PUT", "examinations/{examinationId:guid}/staff")]
    public async Task<ClinicResult> AssignStaffAsync(Guid examinationId, [FromBody] AssignExaminationStaffCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { ExaminationId = examinationId }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ExaminationsUpdateCode)]
    [ClinicOperation("POST", "examinations/{examinationId:guid}/schedule")]
    public async Task<ClinicResult> ScheduleAsync(Guid examinationId, [FromBody] ScheduleExaminationCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { ExaminationId = examinationId }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ExaminationsPerformCode)]
    [ClinicOperation("POST", "examinations/{examinationId:guid}/check-in")]
    public async Task<ClinicResult> CheckInAsync(Guid examinationId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new CheckInExaminationCommand(examinationId), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ExaminationsPerformCode)]
    [ClinicOperation("POST", "examinations/{examinationId:guid}/start")]
    public async Task<ClinicResult> StartAsync(Guid examinationId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new StartExaminationCommand(examinationId), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ExaminationsPerformCode)]
    [ClinicOperation("POST", "examinations/{examinationId:guid}/complete")]
    public async Task<ClinicResult> CompleteAsync(Guid examinationId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new CompleteExaminationCommand(examinationId), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ExaminationsPerformCode)]
    [ClinicOperation("POST", "examinations/{examinationId:guid}/pacs-images")]
    public async Task<ClinicResult> RecordPacsImagesAsync(Guid examinationId, [FromBody] RecordPacsImagesCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { ExaminationId = examinationId }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ExaminationsCancelCode)]
    [ClinicOperation("POST", "examinations/{examinationId:guid}/cancel")]
    public async Task<ClinicResult> CancelAsync(Guid examinationId, [FromBody] CancelExaminationCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { ExaminationId = examinationId }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ExaminationsUpdateCode)]
    [ClinicOperation("POST", "examinations/{examinationId:guid}/items")]
    public async Task<ClinicResult> AddItemAsync(Guid examinationId, [FromBody] AddExaminationItemCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ExaminationItemDto>>(command with { ExaminationId = examinationId }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ExaminationsUpdateCode)]
    [ClinicOperation("POST", "examinations/{examinationId:guid}/payments")]
    [ClinicPermission(CashEntriesAddCode)]
    public async Task<ClinicResult> RecordPaymentAsync(Guid examinationId, [FromBody] RecordExaminationPaymentCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { ExaminationId = examinationId }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ExaminationsUpdateCode)]
    [ClinicOperation("DELETE", "examinations/{examinationId:guid}/items/{examinationItemId:guid}")]
    public async Task<ClinicResult> RemoveItemAsync(Guid examinationId, Guid examinationItemId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new RemoveExaminationItemCommand(examinationId, examinationItemId), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ExaminationsReadCode)]
    [ClinicOperation("GET", "examinations/examination-types/{examinationTypeId:guid}/items")]
    public async Task<ClinicResult> GetExaminationTypeItemsAsync(Guid examinationTypeId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<IReadOnlyList<ExaminationTypeItemDto>>>(new GetExaminationTypeItemsQuery(examinationTypeId), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ExaminationsTypesManageCode)]
    [ClinicOperation("POST", "examinations/examination-types/{examinationTypeId:guid}/items")]
    public async Task<ClinicResult> AddExaminationTypeItemAsync(Guid examinationTypeId, [FromBody] AddExaminationTypeItemCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ExaminationTypeItemDto>>(command with { ExaminationTypeId = examinationTypeId }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ExaminationsTypesManageCode)]
    [ClinicOperation("DELETE", "examinations/examination-types/{examinationTypeId:guid}/items/{examinationTypeItemId:guid}")]
    public async Task<ClinicResult> RemoveExaminationTypeItemAsync(Guid examinationTypeId, Guid examinationTypeItemId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new RemoveExaminationTypeItemCommand(examinationTypeId, examinationTypeItemId), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ExaminationsReadCode)]
    [ClinicOperation("GET", "examinations/calendar")]
    public async Task<ClinicResult> GetCalendarSlotsAsync(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] Guid? equipmentId,
        [FromQuery] Guid? radiologistId,
        [FromQuery] string? modality,
        CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<IReadOnlyList<CalendarSlotDto>>>(
            new GetExaminationsForCalendarQuery(startDate, endDate, equipmentId, radiologistId, modality), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ExaminationsReadCode)]
    [ClinicOperation("GET", "examinations/available-slots")]
    public async Task<ClinicResult> GetAvailableSlotsAsync(
        [FromQuery] DateTime date,
        [FromQuery] Guid equipmentId,
        [FromQuery] int? intervalMinutes,
        CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<IReadOnlyList<AvailableSlotDto>>>(
            new GetAvailableSlotsQuery(date, equipmentId, intervalMinutes ?? 30), ct);
        return result.ToActionResult();
    }
}
