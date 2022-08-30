using System.ComponentModel.DataAnnotations;

public class Admin
{
    [Key] public string UserName { get; set; }

    public string Password { get; set; }
}