using App.Core.DTOs.ResultPattern;
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
        // AUTH
        // =========================================================

        /// <inheritdoc/>
        public async Task<ServiceResult<object>> SendVerificationEmailAsync(
            string to, string username, string verificationLink)
        {
            var subject = "Verify Your Email — Waffer Platform";
            var body = $"""
                <h2>Welcome, {username}!</h2>
                <p>Thank you for registering on Waffer Platform.</p>
                <p>Please verify your email by clicking the button below:</p>
                <a href="{verificationLink}"
                   style="background:#4CAF50;color:white;padding:10px 20px;
                          text-decoration:none;border-radius:5px;">
                   Verify Email
                </a>
                <p>This link expires in 24 hours.</p>
                """;
            return await SendEmailAsync(to, subject, body);
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<object>> SendEmailVerifiedAsync(
            string to, string username)
        {
            var subject = "Email Verified — Waffer Platform";
            var body = $"""
                <h2>Hello, {username}!</h2>
                <p>Your email has been successfully verified.</p>
                <p>Your account is now pending admin approval.</p>
                <p>You will receive another email once your account has been reviewed.</p>
                """;
            return await SendEmailAsync(to, subject, body);
        }

        // =========================================================
        // ADMIN — account verification
        // =========================================================

        /// <inheritdoc/>
        public async Task<ServiceResult<object>> SendAccountVerifiedAsync(
            string to, string username)
        {
            var subject = "Account Verified — Waffer Platform";
            var body = $"""
                <h2>Congratulations, {username}!</h2>
                <p>Your account has been verified by our admin team.</p>
                <p>You can now log in and start using the platform.</p>
                """;
            return await SendEmailAsync(to, subject, body);
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<object>> SendAccountRejectedAsync(
            string to, string username)
        {
            var subject = "Account Rejected — Waffer Platform";
            var body = $"""
                <h2>Hello, {username}</h2>
                <p>We regret to inform you that your account verification has been rejected.</p>
                <p>If you believe this was a mistake, please contact our support team.</p>
                """;
            return await SendEmailAsync(to, subject, body);
        }

        // =========================================================
        // ADMIN — charity need approval
        // =========================================================

        /// <inheritdoc/>
        public async Task<ServiceResult<object>> SendCharityNeedApprovedAsync(
            string to, string username, string productName)
        {
            var subject = "Charity Need Approved — Waffer Platform";
            var body = $"""
                <h2>Hello, {username}!</h2>
                <p>Your charity need for <strong>{productName}</strong> has been approved.</p>
                <p>It is now visible to donor organizations on the platform.</p>
                """;
            return await SendEmailAsync(to, subject, body);
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<object>> SendCharityNeedRejectedAsync(
            string to, string username, string productName)
        {
            var subject = "Charity Need Rejected — Waffer Platform";
            var body = $"""
                <h2>Hello, {username}</h2>
                <p>Your charity need for <strong>{productName}</strong> has been rejected.</p>
                <p>If you believe this was a mistake, please contact our support team.</p>
                """;
            return await SendEmailAsync(to, subject, body);
        }

        // =========================================================
        // ADMIN — offer approval
        // =========================================================

        /// <inheritdoc/>
        public async Task<ServiceResult<object>> SendOfferApprovedAsync(
            string to, string username, string productName)
        {
            var subject = "Offer Approved — Waffer Platform";
            var body = $"""
                <h2>Hello, {username}!</h2>
                <p>Your offer for <strong>{productName}</strong> has been approved.</p>
                <p>It is now visible to charity organizations on the platform.</p>
                """;
            return await SendEmailAsync(to, subject, body);
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<object>> SendOfferRejectedAsync(
            string to, string username, string productName)
        {
            var subject = "Offer Rejected — Waffer Platform";
            var body = $"""
                <h2>Hello, {username}</h2>
                <p>Your offer for <strong>{productName}</strong> has been rejected.</p>
                <p>If you believe this was a mistake, please contact our support team.</p>
                """;
            return await SendEmailAsync(to, subject, body);
        }

        // =========================================================
        // APPLICATIONS — notify on new application
        // =========================================================

        /// <inheritdoc/>
        public async Task<ServiceResult<object>> SendNeedApplicationReceivedAsync(
            string to, string charityUsername, string donorName, string productName)
        {
            var subject = "New Application for Your Charity Need — Waffer Platform";
            var body = $"""
                <h2>Hello, {charityUsername}!</h2>
                <p><strong>{donorName}</strong> has applied to fulfil your charity need
                   for <strong>{productName}</strong>.</p>
                <p>Log in to review and accept or reject the application.</p>
                """;
            return await SendEmailAsync(to, subject, body);
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<object>> SendOfferApplicationReceivedAsync(
            string to, string donorUsername, string charityName, string productName)
        {
            var subject = "New Application for Your Offer — Waffer Platform";
            var body = $"""
                <h2>Hello, {donorUsername}!</h2>
                <p><strong>{charityName}</strong> has applied to receive your offer
                   for <strong>{productName}</strong>.</p>
                <p>Log in to review and accept or reject the application.</p>
                """;
            return await SendEmailAsync(to, subject, body);
        }

        // =========================================================
        // APPLICATIONS — notify on accept / reject
        // =========================================================

        /// <inheritdoc/>
        public async Task<ServiceResult<object>> SendNeedApplicationAcceptedAsync(
            string to, string donorUsername, string productName)
        {
            var subject = "Your Application Has Been Accepted — Waffer Platform";
            var body = $"""
                <h2>Hello, {donorUsername}!</h2>
                <p>Great news! The charity has <strong>accepted</strong> your application
                   for <strong>{productName}</strong>.</p>
                <p>Please coordinate with the charity to complete the donation.</p>
                """;
            return await SendEmailAsync(to, subject, body);
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<object>> SendNeedApplicationRejectedAsync(
            string to, string donorUsername, string productName)
        {
            var subject = "Your Application Was Not Accepted — Waffer Platform";
            var body = $"""
                <h2>Hello, {donorUsername}</h2>
                <p>The charity has <strong>rejected</strong> your application
                   for <strong>{productName}</strong>.</p>
                <p>You can browse other charity needs on the platform.</p>
                """;
            return await SendEmailAsync(to, subject, body);
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<object>> SendOfferApplicationAcceptedAsync(
            string to, string charityUsername, string productName)
        {
            var subject = "Your Application Has Been Accepted — Waffer Platform";
            var body = $"""
                <h2>Hello, {charityUsername}!</h2>
                <p>Great news! The donor has <strong>accepted</strong> your application
                   for <strong>{productName}</strong>.</p>
                <p>Please coordinate with the donor organization to receive the donation.</p>
                """;
            return await SendEmailAsync(to, subject, body);
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<object>> SendOfferApplicationRejectedAsync(
            string to, string charityUsername, string productName)
        {
            var subject = "Your Application Was Not Accepted — Waffer Platform";
            var body = $"""
                <h2>Hello, {charityUsername}</h2>
                <p>The donor has <strong>rejected</strong> your application
                   for <strong>{productName}</strong>.</p>
                <p>You can browse other offers on the platform.</p>
                """;
            return await SendEmailAsync(to, subject, body);
        }

        // =========================================================
        // PRIVATE — core SMTP sender
        // =========================================================

        private async Task<ServiceResult<object>> SendEmailAsync(
            string to, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = body }.ToMessageBody();

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(quit: true);
                return ServiceResult<object>.Success("Email sent successfully");
            }
            catch (SmtpCommandException ex)
            {
                return ServiceResult<object>.Internal("SMTP command failed", new { message = ex.Message });
            }
            catch (SmtpProtocolException ex)
            {
                return ServiceResult<object>.Internal("SMTP protocol error", new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return ServiceResult<object>.Internal("Failed to send email", new { message = ex.Message });
            }
        }
    }
}