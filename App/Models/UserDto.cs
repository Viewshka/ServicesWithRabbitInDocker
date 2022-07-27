namespace App.Models;

public class UserDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public bool IsFired { get; set; }
    public DateTime Created { get; set; }
}