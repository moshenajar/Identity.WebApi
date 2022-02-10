using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Identity.Domain.AggregatesModel.IdentityAggregate
{
    public class AppIdentityCustomPasswordPolicy : PasswordValidator<AppIdentityUser>
    {
        public override async Task<IdentityResult> ValidateAsync(UserManager<AppIdentityUser> manager, AppIdentityUser user, string password)
        {
            IdentityResult result = await base.ValidateAsync(manager, user, password);
            List<IdentityError> errors = result.Succeeded ? new List<IdentityError>() : result.Errors.ToList();
            //5
            if (password.ToLower().Contains(user.UserName.ToLower()))
            {
                errors.Add(new IdentityError
                {
                    Code = "RequireUsername",
                    Description = "Password cannot contain username"
                });
            }
            //2 
            if (!Regex.IsMatch(password[0].ToString(), "^[a-zA-Z]*$"))
            {
                errors.Add(new IdentityError
                {
                    Code = "RequireStartWith",
                    Description = "password must start with character in english"
                });
            }
            //6
            if (password.ToLower().Contains(user.FirstName) || password.ToLower().Contains(user.LastName))
            {
                errors.Add(new IdentityError
                {
                    Code = "RequireFirstNameOrLastName",
                    Description = "Password cannot contain FirstName or LastName"
                });
            }
            return errors.Count == 0 ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray());
        }
    }
}
