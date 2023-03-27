using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeManagerApi.Core.Context;
using TimeManagerApi.Core.Context.Entity;
using TimeManagerApi.Models;
using TimeManagerApi.Models.Requests;
using TimeManagerApi.Services;

namespace TimeManagerApi.Controllers;

[Authorize]
[ApiController]
[Route("api/schedule")]
public class ScheduleController : ControllerBase
{
    private readonly TimeManagerContext _context;
    private readonly IUserService _userService;
    private readonly ILogger<ScheduleController> _logger;
    
    public ScheduleController(TimeManagerContext context, IUserService userService, ILogger<ScheduleController> logger)
    {
        _context = context;
        _userService = userService;
        _logger = logger;
    }
    
    [HttpGet]
    public async Task<StandartResponse<IEnumerable<Schedule>>> Get(int count = 100)
    {
        var currentUserId = await _userService.GetCurrentUserId();
        IEnumerable<Schedule> needSchedules = await _context.Schedules.Where(x => x.UserId == currentUserId)
            .OrderBy(x => x.Day).Take(count).ToListAsync();
        return StandartResponseAnswer.Ok(needSchedules);
    }
    
    [HttpGet("id")]
    public async Task<StandartResponse<Schedule>> GetNeed(Guid id)
    {
        if(id == default)
            return StandartResponseAnswer.Error<Schedule>(message:"Передан неверный идентификатор");
        var currentUserId = await _userService.GetCurrentUserId();
        Schedule? needSchedule = await _context.Schedules.FirstOrDefaultAsync(x => x.UserId == currentUserId && x.Id == id);
        if(needSchedule != null)
            return StandartResponseAnswer.Ok(needSchedule);
        return StandartResponseAnswer.Error<Schedule>("Расписание не найдено");
    }
    
    [HttpGet("day")]
    public async Task<StandartResponse<Schedule>> GetNeed(DateTime day)
    {
        if(day == default)
            return StandartResponseAnswer.Error<Schedule>("Передана неверная дата");
        var currentUserId = await _userService.GetCurrentUserId();
        Schedule? needSchedule = await _context.Schedules.FirstOrDefaultAsync(x => x.UserId == currentUserId && x.Day == day);
        if(needSchedule != null)
            return StandartResponseAnswer.Ok(needSchedule);
        return StandartResponseAnswer.Error<Schedule>("Расписание не найдено");
    }

    [HttpPost]
    public async Task<StandartResponse<Schedule>> Post(ScheduleCreate model)
    {
        if(model.Day == default)
            return StandartResponseAnswer.Error<Schedule>("Передана неверная дата");
        var currentUserId = await _userService.GetCurrentUserId();
        var isExsistSchedule = await _context.Schedules.FirstOrDefaultAsync(x => x.UserId == currentUserId && x.Day == model.Day);
        if (isExsistSchedule != null)
            return StandartResponseAnswer.Error("Расписание уже существует", data:isExsistSchedule);

        var newSchedule = new Schedule()
        {
            UserId = currentUserId,
            Day = model.Day
        };

        try
        {
            await _context.Schedules.AddAsync(newSchedule);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("При добавлении расписания у пользователя {id}, произошла ошбика: {ex}", currentUserId, ex);
            return StandartResponseAnswer.Error<Schedule>("Произошла ошибка. Расписание не создано");
        }
        return StandartResponseAnswer.Ok(newSchedule);
    }

    [HttpPatch]
    public async Task<StandartResponse<object>> Patch(ScheduleEdit model)
    {
        if(model.Id == default)
            return StandartResponseAnswer.Error("Передан неверный идентификатор");
        if(model.Day == default)
            return StandartResponseAnswer.Error("Передана неверная дата");
        var currentUserId = await _userService.GetCurrentUserId();
        var needSchedule = await _context.Schedules.FirstOrDefaultAsync(x => x.UserId == currentUserId && x.Id == model.Id);
        if (needSchedule == null)
            return StandartResponseAnswer.Error("Требуемое расписание не найдено");
        var isExsistSchedule = await _context.Schedules.FirstOrDefaultAsync(x => x.UserId == currentUserId && x.Day == model.Day);
        if (isExsistSchedule != null)
            return StandartResponseAnswer.Error("Вы не можете перенести дату уже на существующее расписание");

        try
        {
            needSchedule.Day = model.Day;
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("При изменении расписания у пользователя {id}, произошла ошбика: {ex}", currentUserId, ex);
            return StandartResponseAnswer.Error("Произошла ошибка. Расписание не создано");
        }

        return StandartResponseAnswer.Ok("Расписание успешно изменено");
    }

    [HttpDelete]
    public async Task<StandartResponse<object>> Delete(Guid id)
    {
        if(id == default)
            return StandartResponseAnswer.Error("Передан неверный идентификатор");
        
        var currentUserId = await _userService.GetCurrentUserId();
        var needSchedule = await _context.Schedules.FirstOrDefaultAsync(x => x.UserId == currentUserId && x.Id == id);
        
        if(needSchedule == null)
            return StandartResponseAnswer.Error("Требуемое расписание не найдено");
        
        try
        {
            _context.Schedules.Remove(needSchedule);
            await _context.SaveChangesAsync();

            return StandartResponseAnswer.Ok("Расписание успешно удалено");
        }
        catch (Exception ex)
        {
            _logger.LogError("При изменении расписания у пользователя {id}, произошла ошбика: {ex}", currentUserId, ex);
            return StandartResponseAnswer.Error("Произошла ошибка. Расписание не создано");
        }
    }
}