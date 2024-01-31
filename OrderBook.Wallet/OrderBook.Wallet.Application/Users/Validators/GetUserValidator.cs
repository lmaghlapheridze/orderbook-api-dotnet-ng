using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OrderBook.Wallets.Application.Common.Interfaces;
using OrderBook.Wallets.Application.Users.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.Wallets.Application.Users.Validators;
public class GetUserValidator : AbstractValidator<GetUserQuery>
{
    private readonly IApplicationDbContext _context;

    public GetUserValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(u => u.UserName)
            .MustAsync(ExistUser)
            .WithMessage("User Does Not Exist");
    }

    public async Task<bool> ExistUser(string userName, CancellationToken cancellationToken)
    {
        return await _context.Users.AnyAsync(x => x.UserName == userName, cancellationToken);

    }

}
