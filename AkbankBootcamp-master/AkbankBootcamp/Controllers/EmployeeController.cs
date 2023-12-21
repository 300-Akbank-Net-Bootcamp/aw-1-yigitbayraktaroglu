using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace AkbankBootcamp.Controllers;

public class Employee
{
    public string Name { get; set; }

    public DateTime DateOfBirth { get; set; }

    public string Email { get; set; }

    public string Phone { get; set; }

    public double HourlySalary { get; set; }
}

public class EmployeeValidator : AbstractValidator<Employee>
{
    public EmployeeValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Length(10, 250).WithMessage("Name must be between 10 and 250 characters.");

        RuleFor(x => x.DateOfBirth)
            .Must(BeValidBirthDate).WithMessage("Birthdate is not valid.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("Email address is not valid.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone is required.")
            .Matches(@"^[0-9]+$").WithMessage("Phone is not valid.");

        RuleFor(x => x.HourlySalary)
           .Must((employee, hourlySalary) => BeValidHourlySalary(hourlySalary, employee)).WithMessage("Minimum hourly salary is not valid.");
    }

    private bool BeValidBirthDate(DateTime dateOfBirth)
    {
        var minAllowedBirthDate = DateTime.Today.AddYears(-65);
        return minAllowedBirthDate <= dateOfBirth;
    }

    private bool BeValidHourlySalary(double hourlySalary, Employee employee)
    {
        var dateBeforeThirtyYears = DateTime.Today.AddYears(-30);
        var isOlderThanThirdyYears = employee.DateOfBirth <= dateBeforeThirtyYears;

        return isOlderThanThirdyYears ? hourlySalary >= 200 : hourlySalary >= 50;
    }
}


[Route("api/[controller]")]
[ApiController]
public class EmployeeController : ControllerBase
{
    public EmployeeController()
    {
    }

    [HttpPost]
    public IActionResult Post([FromBody] Employee value)
    {
        var validator = new EmployeeValidator();
        var validationResult = validator.Validate(value);

        if (!validationResult.IsValid)
        {
            var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(errorMessages);
        }

        return Ok(value);
    }
}