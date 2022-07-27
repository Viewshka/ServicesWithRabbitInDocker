namespace App.Db.Entities;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public bool IsFired { get; set; }

    public DateTime Created { get; set; }
    public DateTime LastModified { get; set; }
}