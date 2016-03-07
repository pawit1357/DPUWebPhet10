using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Web.Routing;

namespace DPUWebPhet10.Helpers
{
    public static class MyLabelExtensions
    {
        public static MvcHtmlString Label(this HtmlHelper htmlHelper, string forName, string labelText)
        {
            return Label(htmlHelper, forName, labelText, (object)null);
        }

        public static MvcHtmlString Label(this HtmlHelper htmlHelper, string forName, string labelText,
                                          object htmlAttributes)
        {
            return Label(htmlHelper, forName, labelText, new RouteValueDictionary(htmlAttributes));
        }
        public static MvcHtmlString Label(this HtmlHelper htmlHelper, string forName, string labelText,
                                          IDictionary<string, object> htmlAttributes)
        {
            var tagBuilder = new TagBuilder("label");
            tagBuilder.MergeAttributes(htmlAttributes);
            tagBuilder.MergeAttribute("for", forName.Replace(".", tagBuilder.IdAttributeDotReplacement), true);
            tagBuilder.SetInnerText(labelText);
            return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString LabelFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
                                                                Expression<Func<TModel, TProperty>> expression,
                                                                string labelText)
        {
            return LabelFor(htmlHelper, expression, labelText, (object)null);
        }
        public static MvcHtmlString LabelFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
                                                                Expression<Func<TModel, TProperty>> expression,
                                                                string labelText, object htmlAttributes)
        {
            return LabelFor(htmlHelper, expression, labelText, new RouteValueDictionary(htmlAttributes));
        }
        public static MvcHtmlString LabelFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
                                                                Expression<Func<TModel, TProperty>> expression,
                                                                string labelText,
                                                                IDictionary<string, object> htmlAttributes)
        {
            string inputName = ExpressionHelper.GetExpressionText(expression);
            return htmlHelper.Label(inputName, labelText, htmlAttributes);
        }
    }
}