using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace AkbankBootcamp.Controllers;

public class Staff
{

    public string? Name { get; set; }


    public string? Email { get; set; }


    public string? Phone { get; set; }


    public decimal? HourlySalary { get; set; }
}

public class StaffValidator : AbstractValidator<Staff>
{
    public StaffValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .Length(10, 250)
            .WithMessage("Name must be between 10 and 250 characters.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email address is required.")
            .EmailAddress()
            .WithMessage("Email address is not valid.");

        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage("Phone is required.")
            .Matches(@"^[0-9]+$")
            .WithMessage("Phone is not valid.");

        RuleFor(x => x.HourlySalary)
            .NotNull()
            .WithMessage("Hourly salary is required.")
            .InclusiveBetween(30, 400)
            .WithMessage("Hourly salary must be between 30 and 400.");
    }
}

[Route("api/[controller]")]
[ApiController]
public class StaffController : ControllerBase
{
    public StaffController()
    {
    }

    [HttpPost]
    public IActionResult Post([FromBody] Staff value)
    {
        var validator = new StaffValidator();
        var validationResult = validator.Validate(value);

        if (!validationResult.IsValid)
        {
            var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(errorMessages);
        }


        return Ok(value);
    }
}