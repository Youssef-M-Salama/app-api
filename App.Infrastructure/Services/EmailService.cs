using App.Core.DTOs.ResultPattern;
using App.Core.ServiceContracts;
using App.Core.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        // =========================================================
        // AUTH
        // =========================================================

        /// <inheritdoc/>
        public async Task<ServiceResult<object>> SendVerificationEmailAsync(
            string to, string username, string verificationLink)
        {
            var subject = "تفعيل البريد الإلكتروني — منصة وفّر";
            var body = $"""
                <div dir="rtl" style="font-family: Arial, sans-serif;">
                    <h2>أهلاً بك يا {username}!</h2>
                    <p>شكراً لتسجيلك في منصة وفّر.</p>
                    <p>يرجى تفعيل بريدك الإلكتروني من خلال الضغط على الزر أدناه:</p>
                    <a href="{verificationLink}"
                       style="background:#4CAF50;color:white;padding:10px 20px;
                              text-decoration:none;border-radius:5px;display:inline-block;">
                       تفعيل البريد الإلكتروني
                    </a>
                    <p>هذا الرابط صالح لمدة 24 ساعة.</p>
                </div>
                """;
            return await SendEmailAsync(to, subject, body);
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<object>> SendEmailVerifiedAsync(
            string to, string username)
        {
            var subject = "تم تفعيل البريد الإلكتروني — منصة وفّر";
            var body = $"""
                <div dir="rtl" style="font-family: Arial, sans-serif;">
                    <h2>مرحباً يا {username}!</h2>
                    <p>تم تفعيل بريدك الإلكتروني بنجاح.</p>
                    <p>حسابك الآن في انتظار مراجعة الإدارة.</p>
                    <p>ستتلقى رسالة أخرى بمجرد مراجعة حسابك وتفعيله بالكامل.</p>
                </div>
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
            var subject = "تم تفعيل الحساب — منصة وفّر";
            var body = $"""
                <div dir="rtl" style="font-family: Arial, sans-serif;">
                    <h2>تهانينا يا {username}!</h2>
                    <p>تم تفعيل حسابك من قبل فريق الإدارة.</p>
                    <p>يمكنك الآن تسجيل الدخول والبدء في استخدام المنصة.</p>
                </div>
                """;
            return await SendEmailAsync(to, subject, body);
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<object>> SendAccountRejectedAsync(
            string to, string username)
        {
            var subject = "تم رفض طلب تفعيل الحساب — منصة وفّر";
            var body = $"""
                <div dir="rtl" style="font-family: Arial, sans-serif;">
                    <h2>مرحباً يا {username}</h2>
                    <p>نأسف لإبلاغكم بأنه قد تم رفض طلب تفعيل حسابكم.</p>
                    <p>إذا كنت تعتقد أن هذا حدث عن طريق الخطأ، يرجى التواصل مع فريق الدعم.</p>
                </div>
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
            var subject = "تمت الموافقة على احتياج جمعية — منصة وفّر";
            var body = $"""
                <div dir="rtl" style="font-family: Arial, sans-serif;">
                    <h2>مرحباً يا {username}!</h2>
                    <p>تمت الموافقة على احتياجكم لـ <strong>{productName}</strong>.</p>
                    <p>الآن يمكن للمؤسسات المانحة رؤيته على المنصة.</p>
                </div>
                """;
            return await SendEmailAsync(to, subject, body);
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<object>> SendCharityNeedRejectedAsync(
            string to, string username, string productName)
        {
            var subject = "تم رفض احتياج جمعية — منصة وفّر";
            var body = $"""
                <div dir="rtl" style="font-family: Arial, sans-serif;">
                    <h2>مرحباً يا {username}</h2>
                    <p>نأسف لإبلاغكم بأنه قد تم رفض احتياجكم لـ <strong>{productName}</strong>.</p>
                    <p>إذا كنت تعتقد أن هذا حدث عن طريق الخطأ، يرجى التواصل مع فريق الدعم.</p>
                </div>
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
            var subject = "تمت الموافقة على العرض — منصة وفّر";
            var body = $"""
                <div dir="rtl" style="font-family: Arial, sans-serif;">
                    <h2>مرحباً يا {username}!</h2>
                    <p>تمت الموافقة على عرضكم لـ <strong>{productName}</strong>.</p>
                    <p>الآن يمكن للجمعيات الخيرية رؤيته على المنصة.</p>
                </div>
                """;
            return await SendEmailAsync(to, subject, body);
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<object>> SendOfferRejectedAsync(
            string to, string username, string productName)
        {
            var subject = "تم رفض العرض — منصة وفّر";
            var body = $"""
                <div dir="rtl" style="font-family: Arial, sans-serif;">
                    <h2>مرحباً يا {username}</h2>
                    <p>نأسف لإبلاغكم بأنه قد تم رفض عرضكم لـ <strong>{productName}</strong>.</p>
                    <p>إذا كنت تعتقد أن هذا حدث عن طريق الخطأ، يرجى التواصل مع فريق الدعم.</p>
                </div>
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
            var subject = "طلب جديد على احتياجكم — منصة وفّر";
            var body = $"""
                <div dir="rtl" style="font-family: Arial, sans-serif;">
                    <h2>مرحباً يا {charityUsername}!</h2>
                    <p>قدمت <strong>{donorName}</strong> طلباً لتلبية احتياجكم لـ <strong>{productName}</strong>.</p>
                    <p>سجل دخولك لمراجعة الطلب والموافقة عليه أو رفضه.</p>
                </div>
                """;
            return await SendEmailAsync(to, subject, body);
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<object>> SendOfferApplicationReceivedAsync(
            string to, string donorUsername, string charityName, string productName)
        {
            var subject = "طلب جديد على عرضكم — منصة وفّر";
            var body = $"""
                <div dir="rtl" style="font-family: Arial, sans-serif;">
                    <h2>مرحباً يا {donorUsername}!</h2>
                    <p>قدمت <strong>{charityName}</strong> طلباً للحصول على عرضكم لـ <strong>{productName}</strong>.</p>
                    <p>سجل دخولك لمراجعة الطلب والموافقة عليه أو رفضه.</p>
                </div>
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
            var subject = "تم قبول طلبكم — منصة وفّر";
            var body = $"""
                <div dir="rtl" style="font-family: Arial, sans-serif;">
                    <h2>مرحباً يا {donorUsername}!</h2>
                    <p>أخبار رائعة! لقد وافقت الجمعية على <strong>قبول</strong> طلبكم الخاص بـ <strong>{productName}</strong>.</p>
                    <p>يرجى التنسيق مع الجمعية لإتمام التبرع.</p>
                </div>
                """;
            return await SendEmailAsync(to, subject, body);
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<object>> SendNeedApplicationRejectedAsync(
            string to, string donorUsername, string productName)
        {
            var subject = "لم يتم قبول طلبكم — منصة وفّر";
            var body = $"""
                <div dir="rtl" style="font-family: Arial, sans-serif;">
                    <h2>مرحباً يا {donorUsername}</h2>
                    <p>لقد قررت الجمعية <strong>عدم قبول</strong> طلبكم الخاص بـ <strong>{productName}</strong>.</p>
                    <p>يمكنكم تصفح احتياجات الجمعيات الأخرى على المنصة.</p>
                </div>
                """;
            return await SendEmailAsync(to, subject, body);
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<object>> SendOfferApplicationAcceptedAsync(
            string to, string charityUsername, string productName)
        {
            var subject = "تم قبول طلبكم — منصة وفّر";
            var body = $"""
                <div dir="rtl" style="font-family: Arial, sans-serif;">
                    <h2>مرحباً يا {charityUsername}!</h2>
                    <p>أخبار رائعة! لقد وافق المتبرع على <strong>قبول</strong> طلبكم الخاص بـ <strong>{productName}</strong>.</p>
                    <p>يرجى التنسيق مع المؤسسة المانحة لاستلام التبرع.</p>
                </div>
                """;
            return await SendEmailAsync(to, subject, body);
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<object>> SendOfferApplicationRejectedAsync(
            string to, string charityUsername, string productName)
        {
            var subject = "لم يتم قبول طلبكم — منصة وفّر";
            var body = $"""
                <div dir="rtl" style="font-family: Arial, sans-serif;">
                    <h2>مرحباً يا {charityUsername}</h2>
                    <p>لقد قرر المتبرع <strong>عدم قبول</strong> طلبكم الخاص بـ <strong>{productName}</strong>.</p>
                    <p>يمكنكم تصفح العروض الأخرى على المنصة.</p>
                </div>
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
                return ServiceResult<object>.Success("تم إرسال البريد الإلكتروني بنجاح");
            }
            catch (SmtpCommandException ex)
            {
                _logger.LogError(ex, "SMTP command failed while sending email to {To}", to);
                return ServiceResult<object>.Internal("فشل أمر SMTP", new { message = ex.Message });
            }
            catch (SmtpProtocolException ex)
            {
                _logger.LogError(ex, "SMTP protocol error while sending email to {To}", to);
                return ServiceResult<object>.Internal("خطأ في بروتوكول SMTP", new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}", to);
                return ServiceResult<object>.Internal("فشل في إرسال البريد الإلكتروني", new { message = ex.Message });
            }
        }
    }
}