using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AlfahimSupplierRegistration.Helper
{
    public class Helper
    {
    }


    public static class ImageHelper
    {
        public static MvcHtmlString Image(this HtmlHelper helper, string src, string altText, string Widht, String Height)
        {
            var builder = new TagBuilder("img");
            builder.MergeAttribute("src", src);
            builder.MergeAttribute("alt", altText);
            builder.MergeAttribute("width", Widht);
            builder.MergeAttribute("height", Height);
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.SelfClosing));
        }

    }
}