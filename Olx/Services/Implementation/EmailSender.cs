using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using Olx.Models;

namespace Olx.Services.Implementation;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<User> _userManager;

    public EmailSender(IConfiguration configuration, IUserStore<User> userStore, UserManager<User> userManager)
    {
        _configuration = configuration;
        _userManager = userManager;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var mime = new MimeMessage();
        mime.From.Add(new MailboxAddress("Mekafi noreply", _configuration["EmailSender:Email"]));
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            throw new InvalidOperationException("User not found");
        }
        
        mime.To.Add(new MailboxAddress(user.Name ?? user.UserName, email));
        mime.Subject = subject;
        mime.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = htmlMessage
        };
        
        using var client = new SmtpClient();
        await client.ConnectAsync(_configuration["EmailSender:Host"], int.Parse(_configuration["EmailSender:Port"]), true);
        await client.AuthenticateAsync(_configuration["EmailSender:Email"], _configuration["EmailSender:Password"]);
        await client.SendAsync(mime);
    }
}