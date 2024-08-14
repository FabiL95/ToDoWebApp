
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace ToDoWebApp.Services
{
    //this class replaces the default sign in manager, so we can use additional functions
    // means we have all functions from the default one, plus the one we define here
    public class CustomSignInManager : SignInManager<IdentityUser>
    {
        public CustomSignInManager(UserManager<IdentityUser> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<IdentityUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<IdentityUser>> logger, IAuthenticationSchemeProvider schemes, IUserConfirmation<IdentityUser> confirmation)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
        }

        public async Task<SignInResult> PasswordSignInWithUserNameOrEmailAsync(string userNameOrEmail, string password, bool isPersistent, bool lockoutOnFailure)
        {
            IdentityUser user;

            if (userNameOrEmail.Contains("@"))
            {
                user = await UserManager.FindByEmailAsync(userNameOrEmail);
            }
            else
            {
                user = await UserManager.FindByNameAsync(userNameOrEmail);
            }

            if (user == null)
            {
                return SignInResult.Failed;
            }

            return await PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
        }
    }

}