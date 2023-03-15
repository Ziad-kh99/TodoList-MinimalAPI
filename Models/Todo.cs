﻿namespace TodoAPI.Models;

public class Todo
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsCompleted { get; set; }
    public string? Secret { get; set; }
}

