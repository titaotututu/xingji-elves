using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TravelApp
{
    class Client
    {
        public HttpClient CreateClient()
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = delegate { return true; };
            HttpClient client = new HttpClient(handler);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        public async Task<HttpResponseMessage> Post(string url, string data)
        {
            HttpResponseMessage result = new HttpResponseMessage();
            using (HttpClient client = CreateClient())
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, url);
                    request.Content = new StringContent(data, Encoding.UTF8, "application/json");
                    result = await client.SendAsync(request);
                }
                catch (Exception e)
                {
                    throw new Exception("Unable to connect to the server", e);
                }
            }
            return result;
        }

        public async Task<HttpResponseMessage> PostImage(string url, Stream imageStream, string fileName)
        {
            HttpResponseMessage result = new HttpResponseMessage();
            using (HttpClient client = CreateClient())
            {
                try
                {
                    var content = new MultipartFormDataContent();
                    string extension = Path.GetExtension(fileName);
                    if (string.IsNullOrEmpty(extension))
                    {
                        if (fileName.Contains(".jpg") || fileName.Contains(".jpeg"))
                            extension = ".jpg";
                        else if (fileName.Contains(".png"))
                            extension = ".png";
                        else if (fileName.Contains(".gif"))
                            extension = ".gif";
                        else
                            extension = ".jpg";
                    }
                    string baseFileName = Path.GetFileNameWithoutExtension(fileName);
                    string fileNameWithExtension = baseFileName + extension;
                    content.Add(new StreamContent(imageStream), "image", fileNameWithExtension);
                    result = await client.PostAsync(url, content);
                }
                catch (Exception e)
                {
                    throw new Exception("Unable to connect to the server", e);
                }
            }
            return result;
        }

        public async Task<HttpResponseMessage> Delete(string url)
        {
            HttpResponseMessage result = new HttpResponseMessage();
            using (HttpClient client = CreateClient())
            {
                try
                {
                    result = await client.DeleteAsync(url);
                }
                catch (Exception e)
                {
                    throw new Exception("Unable to connect to the server", e);
                }
            }
            return result;
        }

        public async Task<HttpResponseMessage> Get(string url)
        {
            HttpResponseMessage result = new HttpResponseMessage();
            using (HttpClient client = CreateClient())
            {
                try
                {
                    result = await client.GetAsync(url);
                }
                catch (Exception e)
                {
                    throw new Exception("Unable to connect to the server", e);
                }
            }
            return result;
        }

        public async Task<HttpResponseMessage> Put(string url, string data)
        {
            HttpResponseMessage result = new HttpResponseMessage();
            using (HttpClient client = CreateClient())
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Put, url);
                    request.Content = new StringContent(data, Encoding.UTF8, "application/json");
                    result = await client.SendAsync(request);
                }
                catch (Exception e)
                {
                    throw new Exception("Unable to connect to the server", e);
                }
            }
            return result;
        }


    }
}
