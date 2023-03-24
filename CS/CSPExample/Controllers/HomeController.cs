using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.ReportDesigner;
using DevExpress.AspNetCore.Reporting.QueryBuilder;
using DevExpress.AspNetCore.Reporting.ReportDesigner;
using DevExpress.XtraReports.Web.WebDocumentViewer;
using DevExpress.AspNetCore.Reporting.WebDocumentViewer;
using System;
using System.Security.Cryptography;

namespace CSPExample.Controllers {
    public class HomeController : Controller {
        public IActionResult Error() {
            Models.ErrorModel model = new Models.ErrorModel();
            return View(model);
        }

        public async Task<IActionResult> Designer(
            [FromServices] IReportDesignerClientSideModelGenerator clientSideModelGenerator,
            [FromQuery] string reportName) {
            var nonceBytes = new byte[32];
            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(nonceBytes);
            var nonce = Convert.ToBase64String(nonceBytes);
            HttpContext.Response.Headers.Add("Content-Security-Policy",
                        string.Format("script-src 'self' 'nonce-{0}';", nonce) +
                        "img-src data: https: http:;" +
                        "style-src 'self';" +
                        "connect-src 'self';" +
                        "worker-src 'self' blob:;" +
                        "frame-src 'self' blob:;"
                    );
            Models.ReportDesignerCustomModel model = new Models.ReportDesignerCustomModel();
            model.ReportDesignerModel = await CreateDefaultReportDesignerModel(clientSideModelGenerator, reportName, null);
            model.Nonce = nonce;
            return View(model);
        }

         public static Dictionary<string, object> GetAvailableDataSources() {
            var dataSources = new Dictionary<string, object>();
            return dataSources;
        }

        public static async Task<ReportDesignerModel> CreateDefaultReportDesignerModel(IReportDesignerClientSideModelGenerator clientSideModelGenerator, string reportName, XtraReport report) {
            reportName = string.IsNullOrEmpty(reportName) ? "TestReport" : reportName;
            var dataSources = GetAvailableDataSources();
            if(report != null) {
                return await clientSideModelGenerator.GetModelAsync(report, dataSources, ReportDesignerController.DefaultUri, WebDocumentViewerController.DefaultUri, QueryBuilderController.DefaultUri);
            }
            return await clientSideModelGenerator.GetModelAsync(reportName, dataSources, ReportDesignerController.DefaultUri, WebDocumentViewerController.DefaultUri, QueryBuilderController.DefaultUri);
        }
        public async Task<IActionResult> Viewer(
            [FromServices] IWebDocumentViewerClientSideModelGenerator clientSideModelGenerator,
            [FromQuery] string reportName) {
            var nonceBytes = new byte[32];
            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(nonceBytes);
            var nonce = Convert.ToBase64String(nonceBytes);
            HttpContext.Response.Headers.Add("Content-Security-Policy",
                        string.Format("script-src 'self' 'nonce-{0}';", nonce) +
                        "img-src data: https: http:;" +
                        "style-src 'self';" +
                        "connect-src 'self';" +
                        "worker-src 'self' blob:;" +
                        "frame-src 'self' blob:;"
                    );
            var reportToOpen = string.IsNullOrEmpty(reportName) ? "TestReport" : reportName;
            var model = new Models.ViewerModel {
                ViewerModelToBind = await clientSideModelGenerator.GetModelAsync(reportToOpen, WebDocumentViewerController.DefaultUri),
                Nonce = nonce
            };
            return View(model);
        }
    }
}
