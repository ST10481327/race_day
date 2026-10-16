using System.ComponentModel.DataAnnotations;

namespace RaceDay_poe_part2.Models;

public class AuthModel
{
    [Required]
    public int Userid { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int age { get; set; }
    public string Email { get; set; }
    public string passwordHashed { get; set; }

    public string createdAt { get; set; }
    public string Role { get; set; } = "";
    public string UserRole { get; set; } = "";

}
