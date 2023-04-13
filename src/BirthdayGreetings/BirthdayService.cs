﻿using System.Net.Mail;

namespace BirthdayGreetings;

/// <summary>
/// Class <c>BirthdayService</c> contains the whole
/// business logic mixing several level of abstractions.
/// It opens and reads a file, it parses its lines, it selects
/// employees with a birthday today and finally
/// it sends them an email.
/// </summary>
internal class BirthdayService
{
    private readonly IEmployeesRepo _employeesRepo;

    internal BirthdayService(IEmployeesRepo employeesRepo)
    {
        _employeesRepo = employeesRepo;
    }

    internal void SendGreetings(string fileName, XDate date, string smtpHost, int smtpPort)
    {
        var employees = _employeesRepo.FindAllEmployees();
        foreach (var employee in employees)
        {
            if (employee.IsBirthday(date))
            {
                SendMessage(
                    smtpHost: smtpHost,
                    smtpPort: smtpPort,
                    from: "sender@here.com",
                    subject: "Happy Birthday!",
                    body: $"Happy Birthday, dear {employee.FirstName}!",
                    recipient: employee.Email);
            }
        }
    }

    /// <summary>
    /// Sends a message to a certain user using a
    /// specific smtp server.
    /// </summary>
    /// <param name="smtpHost"></param>
    /// <param name="smtpPort"></param>
    /// <param name="from"></param>
    /// <param name="subject"></param>
    /// <param name="body"></param>
    /// <param name="recipient"></param>
    private static void SendMessage(string smtpHost, int smtpPort, string from, string subject, string body,
        string recipient)
    {
        using var client = new SmtpClient(smtpHost, smtpPort);
        var message = new MailMessage(from, recipient, subject, body);
        client.Send(message);
    }
}