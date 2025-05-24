using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TravelApp.controller
{
    public partial class WeatherController : UserControl
    {
        private string amapKey = "065e38a43cab8972362f3ee0ea32f1f6"; // 请替换为你的高德 Web 服务 Key
        public WeatherController()
        {
            InitializeComponent();
        }

        private void fetchWeatherButton_Click(object sender, EventArgs e)
        {
            string city = cityTextBox.Text.Trim();
            if (string.IsNullOrEmpty(city))
            {
                MessageBox.Show("请输入城市名");
                return;
            }

            try
            {
                // 首先获取城市编码
                string adcode = GetCityAdcode(city, amapKey);
                if (string.IsNullOrEmpty(adcode))
                {
                    MessageBox.Show("未找到该城市的编码信息");
                    return;
                }

                var weatherResult = GetWeather(adcode, amapKey);
                var forecastResult = GetForecast(adcode, amapKey);

                resultTextBox.Text = "[实时天气]\r\n" + weatherResult;
                forecastTextBox.Text = "[三天天气预报]\r\n" + forecastResult;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"获取天气信息失败：{ex.Message}");
            }
        }

        private string GetCityAdcode(string city, string key)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    string url = $"https://restapi.amap.com/v3/config/district?key={key}&keywords={city}&subdistrict=0";
                    
                    string response = client.DownloadString(url);
                    JObject jo = JObject.Parse(response);

                    if (jo["status"].ToString() != "1")
                    {
                        throw new Exception(jo["info"].ToString());
                    }

                    JArray districts = (JArray)jo["districts"];
                    if (districts.Count == 0)
                    {
                        return null;
                    }

                    // 返回第一个匹配的城市编码
                    return districts[0]["adcode"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"获取城市编码失败：{ex.Message}");
                return null;
            }
        }

        public static string GetWeather(string adcode, string key)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    string url = $"https://restapi.amap.com/v3/weather/weatherInfo?key={key}&city={adcode}&extensions=base";
                    
                    string response = client.DownloadString(url);
                    JObject jo = JObject.Parse(response);

                    if (jo["status"].ToString() != "1")
                    {
                        return $"获取天气失败：{jo["info"]}";
                    }

                    JArray lives = (JArray)jo["lives"];
                    if (lives.Count == 0)
                    {
                        return "未找到该城市的天气信息";
                    }

                    var weather = lives[0];
                    return $"城市：{weather["city"]}\r\n" +
                           $"天气：{weather["weather"]}\r\n" +
                           $"温度：{weather["temperature"]}℃\r\n" +
                           $"风向：{weather["winddirection"]}\r\n" +
                           $"风力：{weather["windpower"]}级\r\n" +
                           $"湿度：{weather["humidity"]}%\r\n" +
                           $"发布时间：{weather["reporttime"]}";
                }
            }
            catch (Exception ex)
            {
                return $"获取实时天气失败：{ex.Message}";
            }
        }

        public static string GetForecast(string adcode, string key)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    string url = $"https://restapi.amap.com/v3/weather/weatherInfo?key={key}&city={adcode}&extensions=all";
                    
                    string response = client.DownloadString(url);
                    JObject jo = JObject.Parse(response);

                    if (jo["status"].ToString() != "1")
                    {
                        return $"获取预报失败：{jo["info"]}";
                    }

                    JArray forecasts = (JArray)jo["forecasts"];
                    if (forecasts.Count == 0)
                    {
                        return "未找到该城市的预报信息";
                    }

                    StringBuilder result = new StringBuilder();
                    JArray casts = (JArray)forecasts[0]["casts"];
                    
                    for (int i = 0; i < casts.Count; i++)
                    {
                        var day = casts[i];
                        result.AppendLine($"{day["date"]} 星期{day["week"]}");
                        result.AppendLine($"天气：{day["dayweather"]} / {day["nightweather"]}");
                        result.AppendLine($"温度：{day["nighttemp"]}~{day["daytemp"]}℃");
                        result.AppendLine($"风向：{day["daywind"]}");
                        result.AppendLine($"风力：{day["daypower"]}级");
                        result.AppendLine();
                    }

                    return result.ToString();
                }
            }
            catch (Exception ex)
            {
                return $"获取预报失败：{ex.Message}";
            }
        }
    }
}
