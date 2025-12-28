using Microsoft.AspNetCore.Authorization;

namespace WebApp_UnderTheHood.Authorization
{
    public class HRManagerProbationRequirement : IAuthorizationRequirement // all custom requirement has to implement this
    {
        public HRManagerProbationRequirement(int probationsMonths)
        {
            ProbationsMonths = probationsMonths;
        }

        public int ProbationsMonths { get; }
    }

    public class HRManagerProbationRequirementHandler : AuthorizationHandler<HRManagerProbationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HRManagerProbationRequirement requirement)
        {
            if(!context.User.HasClaim(x => x.Type == "EmploymentDate"))
            {
                return Task.CompletedTask;
            }

            if(DateTime.TryParse(context.User.FindFirst(x => x.Type == "EmploymentDate")?.Value, out DateTime employmentDate))
            {
                var period = DateTime.Now - employmentDate;
                if(period.Days > 30 * requirement.ProbationsMonths)
                {
                    context.Succeed(requirement);
                }
            }

           return Task.CompletedTask;
        }
    }
}
