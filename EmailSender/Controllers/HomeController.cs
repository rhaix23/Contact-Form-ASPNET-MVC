using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EmailSender.Models;
using MailKit.Net.Smtp;
using MimeKit;

namespace EmailSender.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _configuration;

    public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public IActionResult Index(EmailViewModel model)
    {
        return View(model);
    }

    // Requires MimeMessage and MailKit packages
    [HttpPost]
    public async Task<IActionResult> SendEmail(EmailViewModel model)
    {
        using MimeMessage message = new();
        
        // Set the sender address
        message.From.Add(new MailboxAddress("", _configuration["EmailConfiguration:Email"]));
        
        // Set the recipient address
        message.To.Add(new MailboxAddress(_configuration["EmailConfiguration:Name"], _configuration["EmailConfiguration:Email"]));

        // Set the email subject
        message.Subject = $"INQUIRY - {model.Subject}";
        
        // Set the email body
        string formattedBody = $@"
        <p><strong>Sender:</strong> {model.Name}</p>
        <p><strong>Sender Email:</strong> {model.Email}</p>
        <p>{model.Body.Replace(Environment.NewLine, "<br>")}</p>";
        BodyBuilder emailBodyBuilder = new() { HtmlBody = formattedBody };
        message.Body = emailBodyBuilder.ToMessageBody();
        
        // Set reply-to address
        message.ReplyTo.Add(new MailboxAddress(model.Name, model.Email));
        
        // Send the email
        using SmtpClient client = new();
        try
        {
            // Connect to the SMTP server
            await client.ConnectAsync(_configuration["EmailConfiguration:SmtpServer"], Convert.ToInt32(_configuration["EmailConfiguration:SmtpPort"]), MailKit.Security.SecureSocketOptions.StartTls);
            
            // Authenticate with the SMTP server
            await client.AuthenticateAsync(_configuration["EmailConfiguration:SmtpUsername"], _configuration["EmailConfiguration:SmtpPassword"]);
            
            // Send the email
            await client.SendAsync(message);
            
            // Disconnect from the SMTP server
            await client.DisconnectAsync(true);
            
            return RedirectToAction("Index");
        } 
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return RedirectToAction("Index");
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}