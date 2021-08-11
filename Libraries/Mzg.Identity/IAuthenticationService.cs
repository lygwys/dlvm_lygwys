using Mzg.Organization.Domain;

namespace Mzg.Identity
{
    public interface IAuthenticationService
    {
        ICurrentUser GetAuthenticatedUser();

        void SignIn(SystemUser user, bool persistent = true);

        void SignOut();
    }
}