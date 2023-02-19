﻿using System.Text.Json.Serialization;

namespace TimeManagerApi.Core.Context.Entity;

public class Schedule : BaseEntity
{
    public DateTime Day { get; set; }
    public Guid UserId { get; set; }
    [JsonIgnore] public virtual User User { get; set; } = null!;
    [JsonIgnore] public virtual ICollection<TimeStamp> TimeStamps { get; set; } = new List<TimeStamp>();
}