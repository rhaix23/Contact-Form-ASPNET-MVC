using System.ComponentModel.DataAnnotations;

namespace EmailSender.Models;

public class EmailViewModel
{
    public string Name { get; set; }
    
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}