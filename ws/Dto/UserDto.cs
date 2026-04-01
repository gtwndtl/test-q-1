using System;
using System.Text.Json.Serialization;

namespace ws.Dto;

public class RequestPaginatedUsersDto
{
    [JsonPropertyName("keyword")]
    public string? Keyword { get; set; }

    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }
}

public class RequestCreateUserDto
{
    [JsonPropertyName("firstname")]
    public string Firstname { get; set; } = default!;

    [JsonPropertyName("lastname")]
    public string Lastname { get; set; } = default!;

    [JsonPropertyName("address")]
    public string Address { get; set; } = default!;

    [JsonPropertyName("dateOfBirth")]
    public DateTime DateOfBirth { get; set; }

    [JsonPropertyName("age")]
    public int Age { get; set; }
}

public class RequestUpdateUserDto
{
    [JsonPropertyName("firstname")]
    public string? Firstname { get; set; }

    [JsonPropertyName("lastname")]
    public string? Lastname { get; set; }

    [JsonPropertyName("address")]
    public string? Address { get; set; }

    [JsonPropertyName("dateOfBirth")]
    public DateTime? DateOfBirth { get; set; }

    [JsonPropertyName("age")]
    public int? Age { get; set; }
}

public class UserResponseDto
{
    [JsonPropertyName("userId")]
    public string UserId { get; set; } = default!;
    
    [JsonPropertyName("firstname")]
    public string Firstname { get; set; } = default!;
    
    [JsonPropertyName("lastname")]
    public string Lastname { get; set; } = default!;
    
    [JsonPropertyName("address")]
    public string Address { get; set; } = default!;
    
    [JsonPropertyName("dateOfBirth")]
    public DateTime DateOfBirth { get; set; }
    
    [JsonPropertyName("age")]
    public int Age { get; set; }
}
