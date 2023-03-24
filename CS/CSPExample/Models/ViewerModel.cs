using DevExpress.XtraReports.Web.WebDocumentViewer;

namespace CSPExample.Models {
    public class ViewerModel {
        public WebDocumentViewerModel ViewerModelToBind { get; set; }
        public string Nonce { get; set; }
    }
}
