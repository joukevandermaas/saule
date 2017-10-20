namespace Saule.Common.Tests.Models
{
    public class WidgetResource : ApiResource
    {
        public WidgetResource()
        {
            OfType("Widget");
            Attribute(nameof(Widget.Title));
        }
    }
}
