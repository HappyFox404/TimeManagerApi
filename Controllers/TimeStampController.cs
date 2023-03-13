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
[Route("api/timestamp")]
public class TimeStampController : ControllerBase
{
    private readonly TimeManagerContext _context;
    private readonly ILogger<TimeStampController> _logger;
    private readonly IUserService _userService;

    public TimeStampController(TimeManagerContext context, ILogger<TimeStampController> logger, IUserService userService)
    {
        _context = context;
        _logger = logger;
        _userService = userService;
    }

    [HttpGet]
    public async Task<StandartResponse<IEnumerable<TimeStamp>>> Get(Guid scheduleId)
    {
        var needUserId = await _userService.GetCurrentUserId();
        IEnumerable<TimeStamp> timestamps = await _context.TimeStamps
            .Include(x => x.Schedule)
            .Where(x => x.ScheduleId == scheduleId && x.Schedule.UserId == needUserId).ToListAsync();

        if (timestamps.Any() == false)
            return StandartResponseAnswer.Ok<IEnumerable<TimeStamp>>(default,"Не найдено вреаменных меток");
        
        return StandartResponseAnswer.Ok(timestamps);
    }
    
    [HttpGet("day")]
    public async Task<StandartResponse<IEnumerable<TimeStamp>>> GetForDay(DateTime day)
    {
        if (day == default)
            return StandartResponseAnswer.Error<IEnumerable<TimeStamp>>("Передана неверная дата дня");
        
        var needUserId = await _userService.GetCurrentUserId();
        var needSchedule = await _context.Schedules.FirstOrDefaultAsync(x => x.Day == day && x.UserId == needUserId);
        if (needSchedule == null)
            throw new ArgumentNullException("Не найдено распиание на этот день");
        
        IEnumerable<TimeStamp> timestamps = await _context.TimeStamps
            .Include(x => x.Schedule)
            .Where(x => x.ScheduleId == needSchedule.Id).ToListAsync();
        
        if (timestamps.Any() == false)
            return StandartResponseAnswer.Ok<IEnumerable<TimeStamp>>(default,"Не найдено вреаменных меток");
        
        return StandartResponseAnswer.Ok(timestamps);
    }

    [HttpGet("id")]
    public async Task<StandartResponse<TimeStamp>> GetForId(Guid id)
    {
        var needUserId = await _userService.GetCurrentUserId();
        var needSchedule = await _context.Schedules.FirstOrDefaultAsync(x => x.UserId == needUserId);
        if (needSchedule == null)
            throw new ArgumentNullException("Не найдено распиание на этот день");

        var needTimeStamp = await _context.TimeStamps.FirstOrDefaultAsync(x => x.Id == id);

        if (needTimeStamp == null)
            return StandartResponseAnswer.Ok<TimeStamp>(default,"Не найдена временная метка");
        return StandartResponseAnswer.Ok(needTimeStamp);
    }

    [HttpPost]
    public async Task<StandartResponse<TimeStamp>> Post(TimeStampCreateFromSchedule model)
    {
        var needUserId = await _userService.GetCurrentUserId();
        var needSchedule = await _context.Schedules
            .FirstOrDefaultAsync(x => x.Id == model.ScheduleId && x.UserId == needUserId);
        if (needSchedule == null)
            return StandartResponseAnswer.Error<TimeStamp>("Не найдено расписание для создания временной метки");

        if (String.IsNullOrWhiteSpace(model.Title))
            return StandartResponseAnswer.Error<TimeStamp>("Поле Заголовок обязательно для заполнения");
        if (model.Title.Length > 60)
            return StandartResponseAnswer.Error<TimeStamp>("Поле Заголовок не может быть более 60 символов");

        var exsistsTimeStamp = await _context.TimeStamps
            .FirstOrDefaultAsync(x => x.Time == model.Time && x.ScheduleId == needSchedule.Id);
        if (exsistsTimeStamp != null)
            return StandartResponseAnswer.Error<TimeStamp>("Уже есть временная метка на данный период времени");
        
        var newTimeStamp = new TimeStamp()
        {
            ScheduleId = needSchedule.Id,
            Title = model.Title,
            Description = (String.IsNullOrWhiteSpace(model.Description)) ? null : model.Description,
            Time = model.Time,
            IsNotify = model.IsNotify
        };

        try
        {
            await _context.TimeStamps.AddAsync(newTimeStamp);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("Не удалось создать временную метку у пользователя {user}. {ex}",needUserId, ex);
            return StandartResponseAnswer.Error<TimeStamp>(
                "Во время создания временной метки произошла ошибка, обратитесь в тех. поддержку.");
        }

        return StandartResponseAnswer.Ok(newTimeStamp);
    }
    
    [HttpPost("withschedule")]
    public async Task<StandartResponse<TimeStamp>> Post(TimeStampCreateWithSchedule model)
    {
        var needUserId = await _userService.GetCurrentUserId();

        if (model.Day == default)
            return StandartResponseAnswer.Error<TimeStamp>("Передана неверная дата для расписания");
        
        bool isCreateSchedule = true;
        var exsistsSchedule = await _context.Schedules.FirstOrDefaultAsync(x => x.Day == model.Day && x.UserId == needUserId);
        if (exsistsSchedule != null)
            isCreateSchedule = false;

        if (String.IsNullOrWhiteSpace(model.Title))
            return StandartResponseAnswer.Error<TimeStamp>("Поле Заголовок обязательно для заполнения");
        if (model.Title.Length > 60)
            return StandartResponseAnswer.Error<TimeStamp>("Поле Заголовок не может быть более 60 символов");

        if (isCreateSchedule == false)
        {
            var exsistsTimeStamp = await _context.TimeStamps
                .FirstOrDefaultAsync(x => x.Time == model.Time && x.ScheduleId == exsistsSchedule.Id);
            if (exsistsTimeStamp != null)
                return StandartResponseAnswer.Error<TimeStamp>("Уже есть временная метка на данный период времени");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var newSchedule = new Schedule()
            {
                Day = model.Day,
                UserId = needUserId
            };

            if (isCreateSchedule)
            {
                await _context.Schedules.AddAsync(newSchedule);
                await _context.SaveChangesAsync();
            }

            var newTimeStamp = new TimeStamp()
            {
                ScheduleId = (isCreateSchedule) ? newSchedule.Id : exsistsSchedule!.Id,
                Title = model.Title,
                Description = (String.IsNullOrWhiteSpace(model.Description)) ? null : model.Description,
                Time = model.Time,
                IsNotify = model.IsNotify
            };

            await _context.TimeStamps.AddAsync(newTimeStamp);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return StandartResponseAnswer.Ok(newTimeStamp);
        }
        catch (Exception ex)
        {
            _logger.LogError("Не удалось создать временную метку у пользователя {user}. {ex}",needUserId, ex);
            return StandartResponseAnswer.Error<TimeStamp>(
                "Во время создания временной метки произошла ошибка, обратитесь в тех. поддержку.");
        }
    }

    [HttpPatch]
    public async Task<StandartResponse<object>> Patch(TimeStampEdit model)
    {
        var needUserId = await _userService.GetCurrentUserId();
        
        if (model.TimeStampId == default)
            return StandartResponseAnswer.Error("Предан неверный идентификатор временной метки");
        
        var needTimeStamp = await _context.TimeStamps
            .Include(x => x.Schedule)
            .FirstOrDefaultAsync(x => x.Id == model.TimeStampId && x.Schedule.UserId == needUserId);
        if (needTimeStamp == null)
            return StandartResponseAnswer.Error("Не найдена временная метка для редактирования");

        try
        {
            if (model.Time != needTimeStamp.Time)
            {
                bool isExsistsTimeStamp = await _context.TimeStamps
                    .AnyAsync(x => x.Time == model.Time && x.ScheduleId == needTimeStamp.ScheduleId);
                if (isExsistsTimeStamp)
                    return StandartResponseAnswer.Error(
                        "Вы пытаетесь изменить время метки, которое уже занято другой меткой");

                needTimeStamp.Time = model.Time;
            }
            
            needTimeStamp.Description = model.Description;
            needTimeStamp.Title = model.Title;
            needTimeStamp.IsNotify = model.IsNotify;

            await _context.SaveChangesAsync();
            return StandartResponseAnswer.Ok("Временная метка успешно изменена");
        }
        catch (Exception ex)
        {
            _logger.LogError("Не удалось отредактировать временную метку у пользователя {user}. {ex}",needUserId, ex);
            return StandartResponseAnswer.Error(
                "Во время редактирования временной метки произошла ошибка, обратитесь в тех. поддержку.");
        }
    }

    [HttpDelete]
    public async Task<StandartResponse<object>> Delete(Guid timeStampId)
    {
        var needUserId = await _userService.GetCurrentUserId();
        var needTimeStamp = await _context.TimeStamps
            .Include(x => x.Schedule)
            .FirstOrDefaultAsync(x => x.Schedule.UserId == needUserId);
        if (needTimeStamp == null)
            return StandartResponseAnswer.Error("Не найдена временная метка для удаления");

        try
        {
            _context.TimeStamps.Remove(needTimeStamp);
            await _context.SaveChangesAsync();
            return StandartResponseAnswer.Ok("Временная метка успешно удалена");
        }
        catch (Exception ex)
        {
            _logger.LogError("Не удалось удалить временную метку у пользователя {user}. {ex}",needUserId, ex);
            return StandartResponseAnswer.Error(
                "Во время удаления временной метки произошла ошибка, обратитесь в тех. поддержку.");
        }
    }
}