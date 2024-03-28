using Microsoft.AspNetCore.Identity;
using Olx.Data;
using Olx.Models;

namespace Olx.Extensions;

public static class SignInExtentions
{
    public static IdentityBuilder AddIdentity(this IServiceCollection services)
    {
        return services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ShopDbContext>();
    }
    
    public static IServiceCollection ConfigureIdentity(this IServiceCollection services)
    {
        return services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Password.RequiredUniqueChars = 3;
    
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;
        });
    }
}