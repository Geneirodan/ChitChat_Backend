using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Identity.Persistence;

public sealed class User : IdentityUser<Guid>;