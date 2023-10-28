using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;

namespace VC.Res.WebInterface.Helpers
{
    public class General
    {
        public static MarkupString TextToHtml(string str)
        {
            return new MarkupString(str.Replace("\n", "<br />"));
        }
        public static MarkupString TextToHtml(string str,string color="red")
        {
            return new MarkupString(str.Replace("\n", "<br />"));
        }

        public static async Task OnFilter(SfMultiSelect<int[], Models.DropDownItem> ddl, List<Models.DropDownItem> lstDataSource, FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;
            var query = new Query().Where(new WhereFilter() { Field = "Text", Operator = "contains", value = args.Text, IgnoreCase = true });

            query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

            await ddl.FilterAsync(lstDataSource, query);
        }

        public static async Task OnFilter(SfDropDownList<int?, Models.DropDownItem> ddl, List<Models.DropDownItem> lstDataSource, FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;
            var query = new Query().Where(new WhereFilter() { Field = "Text", Operator = "contains", value = args.Text, IgnoreCase = true });

            query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

            await ddl.FilterAsync(lstDataSource, query);
        }
    }
}
