﻿using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace TravelApp
{
    internal class FileClient
    {
        private HttpClient CreateClient()
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = delegate { return true; };
            HttpClient client = new HttpClient(handler)
            {
                BaseAddress = new Uri("http://localhost:5199/") // 设置BaseAddress
            };
            return client;
        }

        public async Task<Image> Download(string url)
        {
            using (HttpClient client = CreateClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();
                        if (fileBytes.Length > 0)
                        {
                            return BytesToImage(fileBytes);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to connect to the server", ex);
                }
                return null;
            }
        }

        public async Task<bool> Upload(long journalId, string filePath)
        {
            using (HttpClient client = CreateClient())
            {
                string url = "http://localhost:5199/api/File/upload?journalId=" + journalId;
                string boundary = "----" + DateTime.Now.Ticks.ToString("x");
                MultipartFormDataContent content = new MultipartFormDataContent(boundary);
                content.Headers.ContentType = MediaTypeHeaderValue.Parse(string.Format("multipart/form-data; boundary={0}", boundary));
                if (string.IsNullOrEmpty(filePath) && !File.Exists(filePath))
                {
                    throw new Exception("文件不存在");
                }
                string fileName = Path.GetFileName(filePath);

                FileStream fStream = File.Open(filePath, FileMode.Open, FileAccess.Read);
                content.Add(new StreamContent(fStream, (int)fStream.Length), "file", fileName);
                try
                {
                    var result = await client.PostAsync(url, content);
                    if (result.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        string responseContent = await result.Content.ReadAsStringAsync();
                        Console.WriteLine($"状态码：{result.StatusCode}，响应内容：{responseContent}");
                        throw new Exception(result.StatusCode.ToString());
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("异常信息：" + ex.ToString());
                    throw new Exception(ex.Message);
                }
                finally
                {
                    //关闭文件流
                    fStream.Close();
                    client.Dispose();
                }
            }
        }

        public async void Delete(string url)
        {
            using (HttpClient client = CreateClient())
            {
                try
                {
                    HttpResponseMessage response = await client.DeleteAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to connect to the server", ex);
                }
            }
        }

        private Image BytesToImage(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream(buffer);
            Image image = Image.FromStream(ms);
            return image;
        }

        private byte[] ImageToBytes(Image image)
        {
            ImageFormat format = image.RawFormat;
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                return ms.ToArray();
            }
        }
    }
}
