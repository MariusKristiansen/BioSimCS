using Microsoft.AspNetCore.Razor.TagHelpers;
using System;

namespace CustomTagHelpers
{

    [HtmlTargetElement("simple")]
    public class CuteTagHelper : TagHelper
    {
        public string ImageLink { get; set; }
        public string AlternativeText { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Content.SetHtmlContent("THIS IS NOT SO SIMPLE");
        }
    }
    
}
