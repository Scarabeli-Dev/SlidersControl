﻿namespace SlidersControl.Entities;

public class S3ResponseDto
{
    public int StatusCode { get; set; } = 200;
    public string Message { get; set; } = "";
    public string Key { get; set; }
}
