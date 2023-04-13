using System.Net.Mail;

namespace BirthdayGreetings;

class SendBirthdayGreetingsVisitor : IVisitor<Employee>
{
    internal XDate Today { get; set; }
    private readonly IGreetingsFactory _greetingsFactory;
    private readonly SmtpClient _smtpClient;

    internal SendBirthdayGreetingsVisitor(IGreetingsFactory greetingsFactory, SmtpClient smtpClient, XDate today)
    {
        _greetingsFactory = greetingsFactory;
        _smtpClient = smtpClient;
        Today = today;
    }

    void IVisitor<Employee>.Visit(Employee employee)
    {
        IsBirthday(employee).Match(
            () => 0,
            employee => SendMessage(
                from: "sender@here.com",
                recipient: employee.Email,
                greetings: _greetingsFactory.MakeFor(employee))
        );
    }

    private Option IsBirthday(Employee employee) => 
        employee.IsBirthday(Today) ? new Some(employee) : new None();

    private int SendMessage(string from, string recipient, Greetings greetings)
    {
        var message = new MailMessage(from, recipient, greetings.Salutation, greetings.Message);

        _smtpClient.Send(message);
        
        return 1;
    }
}

interface Option
{
    internal T Match<T>(Func<T> whenNone, Func<Employee, T> whenSome);
}

class Some : Option
{
    private readonly Employee _employee;

    internal Some(Employee employee)
    {
        _employee = employee;
    }

    public T Match<T>(Func<T> whenNone, Func<Employee, T> whenSome) => whenSome(_employee);
}
class None : Option
{
    T Option.Match<T>(Func<T> whenNone, Func<Employee, T> whenSome) => whenNone();
}