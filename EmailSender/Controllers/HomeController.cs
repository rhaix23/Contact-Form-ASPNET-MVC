using System.Diagnostics;
using System.Text.Encodings.Web;
using EmailSender.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using SendGrid;
using SendGrid.Helpers.Mail;

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

    public IActionResult SendGrid(EmailViewModel model)
    {
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> SendEmail(EmailViewModel model)
    {
        // Sanitize inputs
        string sanitizedSenderName = HtmlEncoder.Default.Encode(model.Name);
        string sanitizedSenderEmail = HtmlEncoder.Default.Encode(model.Email);
        string sanitizedBody = HtmlEncoder.Default.Encode(model.Body);
    
        // Create the email message
        using MimeMessage message = new();
        
        // Set the sender address
        message.From.Add(new MailboxAddress(model.Email, _configuration["EmailConfiguration:Email"]));
        
        // Set the recipient address
        message.To.Add(new MailboxAddress(_configuration["EmailConfiguration:Name"], _configuration["EmailConfiguration:Email"]));
    
        // Set the email subject
        message.Subject = $"INQUIRY - {model.Subject}";
        
        // Set the email body
        string formattedBody = $@"
        <p><strong>Sender:</strong> {sanitizedSenderName}</p>
        <p><strong>Sender Email:</strong> {sanitizedSenderEmail}</p>
        <p>{sanitizedBody.Replace(Environment.NewLine, "<br>")}</p>";
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
            
            TempData["Message"] = "Your message has been sent successfully.";
            
            return RedirectToAction("Index");
        } 
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);

            TempData["Message"] = "An error occurred while sending your message. Please try again later.";
            
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public async Task<IActionResult> SendEmailUsingSendGrid(EmailViewModel model)
    {
        EmailAddress from = new(_configuration["SendGrid:FromEmail"], _configuration["SendGrid:FromName"]);
        
        EmailAddress to = new(_configuration["SendGrid:ToEmail"], _configuration["SendGrid:ToName"]);
        
        string subject = $"INQUIRY - {model.Subject}";
        
        string plainTextContent = model.Body;

        string htmlContent = model.Body;
        
        SendGridMessage message = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

        message.AddReplyTo(model.Email, model.Name);

        try
        {
            await new SendGridClient(_configuration["SendGrid:ApiKey"]).SendEmailAsync(message);
            
            TempData["Message"] = "Your message has been sent successfully.";
            
            return RedirectToAction("Index");
        } 
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            
            TempData["Message"] = $"{ex}";
            
            return RedirectToAction("Index");
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}