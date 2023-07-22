using Android.Webkit;
using Microsoft.AspNetCore.Components.WebView;
using Microsoft.AspNetCore.Components.WebView.Maui;

namespace MauiBlazorLocalImage
{
    public partial class MainPage
    {
        private partial void BlazorWebViewInitializing(object sender, BlazorWebViewInitializingEventArgs e)
        {
        }

        private partial void BlazorWebViewInitialized(object sender, BlazorWebViewInitializedEventArgs e)
        {
           
            e.WebView.SetWebViewClient(new MyWebViewClient(e.WebView.WebViewClient));
        }

        private class MyWebViewClient : WebViewClient
        {
            private WebViewClient WebViewClient { get; }

            public MyWebViewClient(WebViewClient webViewClient)
            {
                WebViewClient = webViewClient;
            }

            public override bool ShouldOverrideUrlLoading(Android.Webkit.WebView view, IWebResourceRequest request)
            {
                return WebViewClient.ShouldOverrideUrlLoading(view, request);
            }

            public override WebResourceResponse ShouldInterceptRequest(Android.Webkit.WebView view, IWebResourceRequest request)
            {
                var resourceResponse = WebViewClient.ShouldInterceptRequest(view, request);
                if (resourceResponse == null)
                    return null;
                if (resourceResponse.StatusCode == 404)
                {
                    var path = request.Url.Path;
                    if (File.Exists(path))
                    {
                        string mime = MimeTypeMap.Singleton.GetMimeTypeFromExtension(Path.GetExtension(path));
                        string encoding = "UTF-8";
                        Stream stream = File.OpenRead(path);
                        return new(mime, encoding, stream);
                    }
                }
                //Debug.WriteLine("路径：" + request.Url.ToString());
                return resourceResponse;
            }

            public override void OnPageFinished(Android.Webkit.WebView view, string url)
            => WebViewClient.OnPageFinished(view, url);

            protected override void Dispose(bool disposing)
            {
                if (!disposing)
                    return;

                WebViewClient.Dispose();
            }
        }
    }
}
