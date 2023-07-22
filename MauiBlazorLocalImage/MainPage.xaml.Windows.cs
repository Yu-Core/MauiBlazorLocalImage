using Microsoft.AspNetCore.Components.WebView;
using Windows.Storage.Streams;
using Windows.Storage;
using System.Runtime.InteropServices.WindowsRuntime;

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
                    var response = webview2.Environment.CreateWebResourceResponse(stream, 200, "OK", "");
                    args.Response = response;
                }
            };

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
