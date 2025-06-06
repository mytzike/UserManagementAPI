using System.ComponentModel.DataAnnotations; // Import validation attributes
namespace UserManagementAPI.Models;
public class User
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [MinLength(2, ErrorMessage = "Name must be at least 2 characters")]
    public required string Name { get; set; } // 'required' ensures it must be set

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public required string Email { get; set; } // 'required' ensures it must be set

    public User() {} // Default constructor

    public User(string name, string email) // Ensure user is always initialized with valid values
    {
        Name = name;
        Email = email;
    }
}
