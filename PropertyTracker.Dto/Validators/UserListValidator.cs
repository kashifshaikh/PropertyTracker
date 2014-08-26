﻿using FluentValidation;
using PropertyTracker.Dto.Models;

namespace PropertyTracker.Dto.Validators
{
    public class UserListValidator : AbstractValidator<UserList> 
    {
        public UserListValidator()
        {
            RuleFor(list => list.Users).SetCollectionValidator(new UserValidator());
        }
    }
}
