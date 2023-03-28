<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/618345379/2023.1)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T1156672)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->
# Reporting for ASP.NET Core  - Content Security Policy (CSP)

This example demonstrates how to implement a nonce-based [Content Security Policy (CSP)](https://developer.mozilla.org/en-US/docs/Web/HTTP/CSP) for an ASP.NET Core Application through an HTTP response header.

Use the nonce-based approach to disallow inline script and style execution.

## Example Overview

In the *HomeController.cs* file, generate the nonce value. In this example, the [RandomNumberGenerator](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.randomnumbergenerator?view=net-6.0) class is used to generate cryptographically strong random values. Add an HTTP header with the Content Security Policy with nonce for the `script-src` directive.

The following code snippet shows how to add a nonce-based CSP for the Report Designer component:

```cs
//...
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
//...
```

The new nonce value is generated each time the page loads. 

On the page, pass the nonce value to the `Nonce` method:

```cshtml
@{
    var designerRender = Html.DevExpress().ReportDesigner("reportDesigner")
        .Height(null)
        .Width(null)
        .Nonce(Model.Nonce)
        .CssClassName("my-reporting-component")
        .Bind(Model.ReportDesignerModel);
    @designerRender.RenderHtml()
}
```

## Files to Review

- [DashboardModel.cs](./CS/CSPExample/Controllers/HomeController.cs)
- [Designer.cshtml](./CS/CSPExample/Views/Home/Designer.cshtml)
- [Viewer.cshtml](./CS/CSPExample/Views/Home/Viewer.cshtml)

<!-- ## Documentation

- [Content Security Policy](https://docs.devexpress.com/XtraReports/404141/web-reporting/web-reporting-application-security/content-security-policy?p=netframework) -->
