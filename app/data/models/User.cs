using Microsoft.AspNetCore.Identity;

public class User : IdentityUser{
    public List<Note>? Notes {get; set;}
}