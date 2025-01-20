using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using NReco.PdfGenerator;
using Vicgital.Core.Configuration.Services;
using Vicgital.HtmlToPdfGenerator.Business.Components.Definition;

namespace Vicgital.HtmlToPdfGenerator.Business.Components.Implementation
{
    public class PdfGeneratorComponent(
        ILogger<PdfGeneratorComponent> logger,
        IAppConfigurationService appConfiguration) : IPdfGeneratorComponent
    {

        private readonly OSPlatform _OSPlatform = GetCurrentOS();
        private readonly ILogger<PdfGeneratorComponent> _logger = logger;
        private readonly IAppConfigurationService _appConfiguration = appConfiguration;

        public byte[] ConvertHtmlToPDF(string bodyHtml, string headerHtml = "", string footerHtml = "")
        {
            var converter = InitializePDFConverter();

            if (!string.IsNullOrEmpty(headerHtml))
                converter.PageHeaderHtml = headerHtml;

            if (!string.IsNullOrEmpty(footerHtml))
                converter.PageFooterHtml = footerHtml;

            converter.Size = NReco.PdfGenerator.PageSize.Letter;
            converter.Orientation = NReco.PdfGenerator.PageOrientation.Landscape;

            byte[] file = converter.GeneratePdf(bodyHtml);
            return file;

        }

        private HtmlToPdfConverter InitializePDFConverter()
        {

            var converter = new NReco.PdfGenerator.HtmlToPdfConverter();

            string wkHtmlToPdfLicenseOwner = _appConfiguration.GetValue("WKHTML_TO_PDF_LICENSE_OWNER");
            string wkHtmlToPdfLicenseKey = _appConfiguration.GetValue("WKHTML_TO_PDF_LICENSE_KEY");
            string wkHtmlToPdfExeNameWindows = _appConfiguration.GetValue("WKHTML_TO_PDF_EXE_NAME_WINDOWS", "wkhtmltopdf.exe");
            string wkHtmlToPdfExeNameLinux = _appConfiguration.GetValue("WKHTML_TO_PDF_EXE_NAME_LINUX", "wkhtmltopdf");

            string licenseOwner = wkHtmlToPdfLicenseOwner;
            string licenseKey = wkHtmlToPdfLicenseKey;
            converter.License.SetLicenseKey(licenseOwner, licenseKey);
            string? rootFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            _logger.LogInformation("app root folder: {rootFolder}", rootFolder);

            if (_OSPlatform == OSPlatform.Windows)
            {
                converter.WkHtmlToPdfExeName = wkHtmlToPdfExeNameWindows;
                converter.PdfToolPath = rootFolder + "\\wkhtmltopdf\\windows";
            }
            else if (_OSPlatform == OSPlatform.Linux)
            {
                converter.WkHtmlToPdfExeName = wkHtmlToPdfExeNameLinux;
                converter.PdfToolPath = rootFolder + "/wkhtmltopdf/linux";
            }
            else if (_OSPlatform == OSPlatform.OSX)
            {
                converter.WkHtmlToPdfExeName = wkHtmlToPdfExeNameLinux;
                converter.PdfToolPath = rootFolder + "/wkhtmltopdf/osx";
            }


            _logger.LogInformation("WkHtmlToPdfExeName: '{WkHtmlToPdfExeName}', PdfToolPath: '{PdfToolPath}'", converter.WkHtmlToPdfExeName, converter.PdfToolPath);

            return converter;
        }

        private static OSPlatform GetCurrentOS()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return OSPlatform.Windows;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return OSPlatform.Linux;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return OSPlatform.OSX;

            return OSPlatform.Windows;

        }

    }
}
