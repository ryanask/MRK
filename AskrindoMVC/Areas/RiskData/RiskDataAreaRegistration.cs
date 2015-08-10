using System.Web.Mvc;

namespace AskrindoMVC.Areas.RiskData
{
    public class RiskDataAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "RiskData";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "RiskData_default",
                "RiskData/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
