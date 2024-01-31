using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OrderBook.Wallets.Application.Common.Interfaces;
using OrderBook.Wallets.Application.Users.Commands;
using OrderBook.Wallets.Application.Users.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.Wallets.Application.Users.Validators;
public class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateUserValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(c => c.UserName)
            .MustAsync(NotExistUser)
            .WithMessage("User With This Username Already Exists");

        RuleFor(c => c.UserName.Length)
            .GreaterThan(4)
            .WithMessage("Username Must Contain More Than 4 Characters");
    }

    public async Task<bool> NotExistUser(string userName, CancellationToken cancellationToken)
    {
        return !await _context.Users.AnyAsync(x => x.UserName == userName, cancellationToken);

    }

}
