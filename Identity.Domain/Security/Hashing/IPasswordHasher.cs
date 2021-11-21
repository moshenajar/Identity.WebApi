using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Security.Hashing
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool PasswordMatches(string providedPassword, string passwordHash);
    }
}
