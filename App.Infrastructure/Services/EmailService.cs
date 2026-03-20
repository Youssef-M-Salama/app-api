using App.Core.DTO.ResultPattern;
using App.Core.ServiceContracts;
using App.Core.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace App.Infrastructure.Services
{
    /// <summary>
    /// Handles email sending via SMTP using MailKit.
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        // =========================================================
        // PUBLIC METHODS
        // =========================================================

        /// <inheritdoc/>
        public async Task<ServiceResult<object>> SendVerificationEmailAsync(
            string to,
            string username,
            string verificationLink)
        {
            var subject = "Verify Your Email — Charity Platform";
            var body = $"""
                <h2>Welcome, {username}!</h2>
                <p>Thank you for registering on Charity Platform.</p>
                <p>Please verify your email by clicking the button below:</p>
                <a href="{verificationLink}" 
                   style="background:#4CAF50;color:white;padding:10px 20px;
                          text-decoration:none;border-radius:5px;">
                   Verify Email
                </a>
                <p>If you did not register, please ignore this email.</p>
                <p>This link expires in 24 hours.</p>
                """;

            return await SendEmailAsync(to, subject, body);
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<object>> SendEmailVerifiedAsync(
            string to,
            string username)
        {
            var subject = "Email Verified — Charity Platform";
            var body = $"""
                <h2>Hello, {username}!</h2>
                <p>Your email has been successfully verified.</p>
                <p>Your account is now pending admin approval.</p>
                <p>You will receive another email once your account has been reviewed.</p>
                """;

            return await SendEmailAsync(to, subject, body);
        }

        // =========================================================
        // PRIVATE — CORE SMTP SENDER
        // =========================================================

        /// <summary>
        /// Core method that builds and sends the email via SMTP.
        /// All public methods delegate here.
        /// </summary>
        private async Task<ServiceResult<object>> SendEmailAsync(
            string to,
            string subject,
            string body)
        {
            // Build the email message
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(
                _emailSettings.SenderName,
                _emailSettings.SenderEmail));

            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            // Build HTML body
            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            message.Body = bodyBuilder.ToMessageBody();

            // Connect and send via SMTP
            using var client = new SmtpClient();

            try
            {
                await client.ConnectAsync(
                    _emailSettings.Host,
                    _emailSettings.Port,
                    SecureSocketOptions.StartTls);

                await client.AuthenticateAsync(
                    _emailSettings.SenderEmail,
                    _emailSettings.Password);

                await client.SendAsync(message);
                await client.DisconnectAsync(quit: true);

                return ServiceResult<object>.Success("Email sent successfully");
            }
            catch (SmtpCommandException ex)
            {
                // SMTP server rejected the command
                // e.g. wrong credentials, invalid recipient, spam policy
                return ServiceResult<object>.Internal(
                    "SMTP command failed",
                    new { message = ex.Message });
            }
            catch (SmtpProtocolException ex)
            {
                // Server sent an unexpected response
                return ServiceResult<object>.Internal(
                    "SMTP protocol error",
                    new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Network issue, timeout, DNS failure, etc.
                return ServiceResult<object>.Internal(
                    "Failed to send email",
                    new { message = ex.Message });
            }
        }
    }
}