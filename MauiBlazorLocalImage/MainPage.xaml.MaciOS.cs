using Foundation;
using Microsoft.AspNetCore.Components.WebView;
using System.Runtime.Versioning;
using WebKit;

namespace MauiBlazorLocalImage
{
    public partial class MainPage
    {
        private partial void BlazorWebViewInitializing(object sender, BlazorWebViewInitializingEventArgs e)
        {
            e.Configuration.SetUrlSchemeHandler(new MySchemeHandler(), "myfile");
        }

        private partial void BlazorWebViewInitialized(object sender, BlazorWebViewInitializedEventArgs e)
        {
        }

        private class MySchemeHandler : NSObject, IWKUrlSchemeHandler
        {
            [Export("webView:startURLSchemeTask:")]
            [SupportedOSPlatform("ios11.0")]
            public void StartUrlSchemeTask(WKWebView webView, IWKUrlSchemeTask urlSchemeTask)
            {
                if (urlSchemeTask.Request.Url == null)
                {
                    return;
                }

                var path = urlSchemeTask.Request.Url?.Path ?? "";
                if (File.Exists(path))
                {
                    byte[] bytes = File.ReadAllBytes(path);
                    using var response = new NSHttpUrlResponse(urlSchemeTask.Request.Url, 200, "HTTP/1.1", null);
                    urlSchemeTask.DidReceiveResponse(response);
                    urlSchemeTask.DidReceiveData(NSData.FromArray(bytes));
                    urlSchemeTask.DidFinish();
                }
            }

            [Export("webView:stopURLSchemeTask:")]
            public void StopUrlSchemeTask(WKWebView webView, IWKUrlSchemeTask urlSchemeTask)
            {
            }
        }
    }
}
