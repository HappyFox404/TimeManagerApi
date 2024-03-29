﻿using System.Text.Json.Serialization;

namespace TimeManagerApi.Core.Context.Entity;

public class User : BaseEntity
{
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Email { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}