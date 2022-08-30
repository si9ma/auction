using System.ComponentModel.DataAnnotations;

public class User
{
    [Key] public string UserName { get; set; }

    public string Password { get; set; }
}