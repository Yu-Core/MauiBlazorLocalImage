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
            e.Configuration.SetUrlSchemeHandler(new MySchemeHandler(e.Configuration.GetUrlSchemeHandler("app")), "app");
        }

        private partial void BlazorWebViewInitialized(object sender, BlazorWebViewInitializedEventArgs e)
        {
        }

        private class MySchemeHandler : NSObject, IWKUrlSchemeHandler
        {
            private IWKUrlSchemeHandler WKUrlSchemeHandler { get; }
            public MySchemeHandler(IWKUrlSchemeHandler wKUrlSchemeHandler)
            {
                WKUrlSchemeHandler = wKUrlSchemeHandler;
            }

            [Export("webView:startURLSchemeTask:")]
            [SupportedOSPlatform("ios11.0")]
            public void StartUrlSchemeTask(WKWebView webView, IWKUrlSchemeTask urlSchemeTask)
            {
                if (urlSchemeTask.Request.Url == null)
                {
                    return;
                }

                var path = urlSchemeTask.Request.Url?.AbsoluteString ?? "";
                if (File.Exists(path))
                {

                    byte[] bytes = File.ReadAllBytes(path);
                    //using (var dic = new NSMutableDictionary<NSString, NSString>())
                    //{
                    //    dic.Add((NSString)"Content-Length", (NSString)(responseBytes.Length.ToString(CultureInfo.InvariantCulture)));
                    //    dic.Add((NSString)"Content-Type", (NSString)contentType);
                    //    // Disable local caching. This will prevent user scripts from executing correctly.
                    //    dic.Add((NSString)"Cache-Control", (NSString)"no-cache, max-age=0, must-revalidate, no-store");
                    //if (urlSchemeTask.Request.Url != null)
                    //{
                        using var response = new NSHttpUrlResponse(urlSchemeTask.Request.Url, 200, "HTTP/1.1", null);
                        urlSchemeTask.DidReceiveResponse(response);
                    //}

                    //}
                    urlSchemeTask.DidReceiveData(NSData.FromArray(bytes));
                    urlSchemeTask.DidFinish();
                }
                else
                {
                    WKUrlSchemeHandler.StartUrlSchemeTask(webView, urlSchemeTask);
                }
            }

            [Export("webView:stopURLSchemeTask:")]
            public void StopUrlSchemeTask(WKWebView webView, IWKUrlSchemeTask urlSchemeTask)
            {
                WKUrlSchemeHandler.StopUrlSchemeTask(webView, urlSchemeTask);
            }
        }
    }
}
