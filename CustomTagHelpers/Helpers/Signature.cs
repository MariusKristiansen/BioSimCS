using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace CustomTagHelpers.Helpers
{
    [HtmlTargetElement("signature")]
    public class Signature : TagHelper
    {
        public string ImageLink { get; set; }
        public string AlternativeText { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Content.SetHtmlContent("Marius S Kristiansen");
        }
    }

}
