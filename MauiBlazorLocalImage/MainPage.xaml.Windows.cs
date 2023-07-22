using Microsoft.AspNetCore.Components.WebView;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;

namespace MauiBlazorLocalImage
{
    public partial class MainPage
    {
        private partial void BlazorWebViewInitializing(object sender, BlazorWebViewInitializingEventArgs e)
        {
        }

        private partial void BlazorWebViewInitialized(object sender, BlazorWebViewInitializedEventArgs e)
        {
            var webview2 = e.WebView.CoreWebView2;

            webview2.WebResourceRequested += async (sender, args) =>
            {
                string path = new Uri(args.Request.Uri).AbsolutePath.TrimStart('/');
                path = Uri.UnescapeDataString(path);
                if (File.Exists(path))
                {
                    using var contentStream = File.OpenRead(path);
                    IRandomAccessStream stream = await CopyContentToRandomAccessStreamAsync(contentStream);
                    var response = webview2.Environment.CreateWebResourceResponse(stream, 200, "OK", null);
                    args.Response = response;
                }
            };

            //为什么这么写？我也不知道，Maui源码就是这么写的
            async Task<IRandomAccessStream> CopyContentToRandomAccessStreamAsync(Stream content)
            {
                using var memStream = new MemoryStream();
                await content.CopyToAsync(memStream);
                var randomAccessStream = new InMemoryRandomAccessStream();
                await randomAccessStream.WriteAsync(memStream.GetWindowsRuntimeBuffer());
                return randomAccessStream;
            }
        }
    }
}
