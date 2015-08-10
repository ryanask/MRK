using System.Web.Mvc;

namespace AskrindoMVC.Areas.Doc
{
    public class DocAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Doc";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Doc_default",
                "Doc/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
