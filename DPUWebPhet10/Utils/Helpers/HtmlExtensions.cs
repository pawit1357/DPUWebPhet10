using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Linq.Expressions;
using System.Text;
using System.Web.Routing;
using System.ComponentModel.DataAnnotations;

namespace DPUWebPhet10.Helpers
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString RadioButtonForSelectList<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            IEnumerable<SelectListItem> listOfValues)
        {
            var metaData = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var sb = new StringBuilder();

            if (listOfValues != null)
            {
                // Create a radio button for each item in the list 
                foreach (SelectListItem item in listOfValues)
                {
                    // Generate an id to be given to the radio button field 
                    var id = string.Format("{0}_{1}", metaData.PropertyName, item.Value);

                    // Create and populate a radio button using the existing html helpers 
                    var label = htmlHelper.Label(id, HttpUtility.HtmlEncode(item.Text), new { style = "text-align: left;" });
                    var radio = htmlHelper.RadioButtonFor(expression, item.Value, new { id = id }).ToHtmlString();

                    // Create the html string that will be returned to the client 
                    // e.g. <input data-val="true" data-val-required="You must select an option" id="TestRadio_1" name="TestRadio" type="radio" value="1" /><label for="TestRadio_1">Line1</label> 
                    sb.AppendFormat("<div class=\"RadioButton\">{0}{1}</div>", radio, label);
                }
            }

            return MvcHtmlString.Create(sb.ToString());
        }

        // Extension method
        public static MvcHtmlString ActionImage(this HtmlHelper html, string action, object routeValues, string imagePath, string alt)
        {
            var url = new UrlHelper(html.ViewContext.RequestContext);

            // build the <img> tag
            var imgBuilder = new TagBuilder("img");
            imgBuilder.MergeAttribute("src", url.Content(imagePath));
            imgBuilder.MergeAttribute("alt", alt);
            string imgHtml = imgBuilder.ToString(TagRenderMode.SelfClosing);

            // build the <a> tag
            var anchorBuilder = new TagBuilder("a");
            anchorBuilder.MergeAttribute("href", url.Action(action, routeValues));
            anchorBuilder.InnerHtml = imgHtml; // include the <img> tag inside
            string anchorHtml = anchorBuilder.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(anchorHtml);
        }


        public static MvcHtmlString SpanFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            var valueGetter = expression.Compile();
            var value = valueGetter(helper.ViewData.Model);

            var span = new TagBuilder("span");
            span.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            span.SetInnerText(value.ToString());

            return MvcHtmlString.Create(span.ToString());
        }


        public static MvcHtmlString ValidatedEditorFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            if (htmlHelper.ViewData.ModelMetadata.ModelType == null)
            {
                return new MvcHtmlString(String.Empty);
            }

            TagBuilder tagBuilder = new TagBuilder("input");
            var name = ExpressionHelper.GetExpressionText(expression);
            string validation = String.Empty;
            //Try to get the attributes for the property
            Object[] objects = typeof(TModel).GetProperty(name).GetCustomAttributes(true);
            foreach (Attribute attribute in objects)
            {
                if (attribute.GetType() == typeof(RequiredAttribute))
                {
                    validation += "validate[required]";
                }
                if (attribute.GetType() == typeof(RangeAttribute))
                {
                    var min = ((RangeAttribute)attribute).Minimum;
                    var max = ((RangeAttribute)attribute).Maximum;
                    validation += String.Format("validate[required, min[{0}],max[{1}]]", min, max);
                }
                if (attribute.GetType() == typeof(StringLengthAttribute))
                {
                    var minimumLength = ((StringLengthAttribute)attribute).MinimumLength;
                    var maximumLength = ((StringLengthAttribute)attribute).MaximumLength;
                    string validator = String.Format("maxSize[{0}]", maximumLength);

                    if (minimumLength >= 0)
                    {
                        validator += String.Format(",minSize[{0}]", minimumLength);
                    }
                    validation += String.Format("validate[required, {0}", validator);
                }
            }

            tagBuilder.GenerateId(name);
            tagBuilder.AddCssClass(validation);
            return new MvcHtmlString(tagBuilder.ToString());
        }


        //public MvcHtmlString ActionImage(this HtmlHelper htmlHelper,
        //                                 string controller,
        //                                 string action,
        //                                 object routeValues,
        //                                 string imagePath,
        //                                 string alternateText = "",
        //                                 object htmlAttributes = null)
        //{

        //    var anchorBuilder = new TagBuilder("a");
        //    var url = new UrlHelper(ViewContext.RequestContext);

        //    anchorBuilder.MergeAttribute("href", url.Action(action, controller, routeValues));

        //    var imgBuilder = new TagBuilder("img");
        //    imgBuilder.MergeAttribute("src", url.Content(imagePath));
        //    imgBuilder.MergeAttribute("alt", alternateText);

        //    var attributes = (IDictionary<string, object>)HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
        //    imgBuilder.MergeAttributes(attributes);
        //    string imgHtml = imgBuilder.ToString(TagRenderMode.SelfClosing);

        //    anchorBuilder.InnerHtml = imgHtml;
        //    return MvcHtmlString.Create(anchorBuilder.ToString());
        //}
    }
}