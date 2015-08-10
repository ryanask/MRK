using System.Web.Mvc;

namespace AskrindoMVC.Areas.RefData
{
    public class RefDataAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "RefData";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "RefData_default",
                "RefData/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
