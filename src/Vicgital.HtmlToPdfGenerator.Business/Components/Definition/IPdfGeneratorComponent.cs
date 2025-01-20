using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicgital.HtmlToPdfGenerator.Business.Components.Definition
{
    public interface IPdfGeneratorComponent
    {
        byte[] ConvertHtmlToPDF(string bodyHtml, string headerHtml = "", string footerHtml = "");
    }
}
