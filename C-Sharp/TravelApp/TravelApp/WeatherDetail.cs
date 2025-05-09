﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Weather_space
{
    class WeatherDetail
    {
        // 定义省、市、区的模型对象
        public PlaceModel province;
        public PlaceModel city;
        public PlaceModel district;

        // 构造函数，初始化省、市、区的模型对象
        public WeatherDetail(PlaceModel province, PlaceModel city, PlaceModel district)
        {
            this.province = province;
            this.city = city;
            this.district = district;
        }

        // 定义属性，用于存储未来1到7天的天气信息
        public string[] Day_1To7 { private set; get; }
        public string[] Day_7To15 { private set; get; }
        public string[] Info_1To7 { private set; get; }
        public string[] Info_7To15 { private set; get; }
        public string[] Temperature_1To7 { private set; get; }
        public string[] Temperature_7To15 { private set; get; }
        public string[] Wind_1To7 { private set; get; }
        public string[] Wind1_7To15 { private set; get; }
        public string[] Wind2_7To15 { private set; get; }
        public WeatherStatus[] WeatherStatus_1To7 { private set; get; }
        public WeatherStatus[] WeatherStatus_7To15 { private set; get; }

        // 处理天气数据
        public void HandleWeather()
        {
            using (WebClient webClient = new WebClient() { Encoding = Encoding.UTF8 })
            {
                // 根据城市、区的ID构建天气预报查询的URL
                string url = $"http://www.weather.com.cn/weather/";
                if (city.ID == "00")
                {
                    url += $"{province.ID}{district.ID}{city.ID}.shtml";
                }
                else if (district.ID.Length > 2)
                {
                    url += $"{district.ID}.shtml";
                }
                else
                {
                    url += $"{province.ID}{city.ID}{district.ID}.shtml";
                }

                // 下载网页内容
                byte[] contentBytes = webClient.DownloadData(url);
                using (MemoryStream stream = new MemoryStream(contentBytes))
                using (StreamReader reader = new StreamReader(stream))
                {
                    // 读取网页内容
                    string content = reader.ReadToEnd();
                    // 使用正则表达式提取天气信息
                    MatchCollection matches = Regex.Matches(content, "<ul class=\"t clearfix\">.*?</ul>", RegexOptions.Singleline);
                    if (matches.Count > 0)
                    {
                        matches = Regex.Matches(matches[0].Value, "<li class=\"sky skyid.*?</li>", RegexOptions.Singleline);
                        if (matches.Count == 7)
                        {
                            // 定义列表存储解析出来的天气信息
                            List<string> dayList = new List<string>();
                            List<string> infoList = new List<string>();
                            List<string> tempList = new List<string>();
                            List<string> windList = new List<string>();
                            List<WeatherStatus> statusList = new List<WeatherStatus>();
                            MatchCollection matches_temp;
                            foreach (Match match in matches)
                            {
                                // 提取日期信息
                                matches_temp = Regex.Matches(match.Value, "<h1>.*?</h1>");
                                if (matches_temp.Count > 0) dayList.Add(RemoveAngleBrackets(matches_temp[0].Value));
                                else dayList.Add("");

                                // 提取天气信息
                                matches_temp = Regex.Matches(match.Value, "<p.*class=\"wea\".*?</p>");
                                if (matches_temp.Count > 0)
                                {
                                    infoList.Add(RemoveAngleBrackets(matches_temp[0].Value));
                                    WeatherStatus status = GetWeatherStatus(RemoveAngleBrackets(matches_temp[0].Value));
                                    if (status == WeatherStatus.Weizhi)
                                    {
                                        status = GetUnkownWeatherStatus(RemoveAngleBrackets(matches_temp[0].Value));
                                    }
                                    statusList.Add(status);
                                }
                                else
                                {
                                    dayList.Add("");
                                    statusList.Add(WeatherStatus.Weizhi);
                                }

                                // 提取温度信息
                                matches_temp = Regex.Matches(match.Value, "<span>.*?</span>");
                                string temperature = "";
                                string wind = "";
                                if (matches_temp.Count > 0)
                                {
                                    temperature = RemoveAngleBrackets(matches_temp[0].Value) + "℃";
                                }
                                matches_temp = Regex.Matches(match.Value, "<i>.*?</i>");
                                if (matches_temp.Count >= 1)
                                {
                                    temperature = string.IsNullOrEmpty(temperature) ? RemoveAngleBrackets(matches_temp[0].Value) : $"{temperature}/{RemoveAngleBrackets(matches_temp[0].Value)}";
                                }
                                if (matches_temp.Count >= 2)
                                {
                                    wind = RemoveAngleBrackets(matches_temp[1].Value);
                                }
                                tempList.Add(temperature);
                                windList.Add(wind);
                            }

                            // 将解析出来的数据存储到相应的属性中
                            this.Day_1To7 = dayList.ToArray();
                            this.Info_1To7 = infoList.ToArray();
                            this.Temperature_1To7 = tempList.ToArray();
                            this.Wind_1To7 = windList.ToArray();
                            this.WeatherStatus_1To7 = statusList.ToArray();
                        }
                    }
                }

                // 查询未来8到15天的天气预报数据
                url = $"http://www.weather.com.cn/weather15d/";
                if (city.ID == "00")
                {
                    url += $"{province.ID}{district.ID}{city.ID}.shtml";
                }
                else if (district.ID.Length > 2)
                {
                    url += $"{district.ID}.shtml";
                }
                else
                {
                    url += $"{province.ID}{city.ID}{district.ID}.shtml";
                }

                // 下载网页内容
                contentBytes = webClient.DownloadData(url);
                using (MemoryStream stream = new MemoryStream(contentBytes))
                using (StreamReader reader = new StreamReader(stream))
                {
                    // 读取网页内容
                    string content = reader.ReadToEnd();
                    // 使用正则表达式提取天气信息
                    MatchCollection matches = Regex.Matches(content, "<ul class=\"t clearfix\">.*?</ul>", RegexOptions.Singleline);
                    if (matches.Count > 0)
                    {
                        matches = Regex.Matches(matches[0].Value, "<li.*?</li>", RegexOptions.Singleline);
                        if (matches.Count == 8)
                        {
                            // 定义列表存储解析出来的天气信息
                            List<string> dayList = new List<string>();
                            List<string> infoList = new List<string>();
                            List<string> tempList = new List<string>();
                            List<string> wind1List = new List<string>();
                            List<string> wind2List = new List<string>();
                            List<WeatherStatus> statusList = new List<WeatherStatus>();
                            MatchCollection matches_temp;
                            foreach (Match match in matches)
                            {
                                // 提取日期信息
                                matches_temp = Regex.Matches(match.Value, "<span class=\"time\">.*?</span>");
                                if (matches_temp.Count > 0) dayList.Add(RemoveAngleBrackets(matches_temp[0].Value));
                                else dayList.Add("");

                                // 提取天气信息
                                matches_temp = Regex.Matches(match.Value, "<span class=\"wea\">.*?</span>");
                                if (matches_temp.Count > 0)
                                {
                                    infoList.Add(RemoveAngleBrackets(matches_temp[0].Value));
                                    WeatherStatus status = GetWeatherStatus(RemoveAngleBrackets(matches_temp[0].Value));
                                    if (status == WeatherStatus.Weizhi)
                                    {
                                        status = GetUnkownWeatherStatus(RemoveAngleBrackets(matches_temp[0].Value));
                                    }
                                    statusList.Add(status);
                                }
                                else
                                {
                                    dayList.Add("");
                                    statusList.Add(WeatherStatus.Weizhi);
                                }

                                // 提取温度信息
                                matches_temp = Regex.Matches(match.Value, "<span class=\"tem\".*?</span>");
                                if (matches_temp.Count > 0) tempList.Add(RemoveAngleBrackets(matches_temp[0].Value));
                                else tempList.Add("");

                                // 提取风力信息
                                matches_temp = Regex.Matches(match.Value, "<span class=\"wind\">.*?</span>");
                                if (matches_temp.Count > 0) wind1List.Add(RemoveAngleBrackets(matches_temp[0].Value));
                                else wind1List.Add("");

                                matches_temp = Regex.Matches(match.Value, "<span class=\"wind1\">.*?</span>");
                                if (matches_temp.Count > 0) wind2List.Add(RemoveAngleBrackets(matches_temp[0].Value));
                                else wind2List.Add("");
                            }

                            // 将解析出来的数据存储到相应的属性中
                            this.Day_7To15 = dayList.ToArray();
                            this.Info_7To15 = infoList.ToArray();
                            this.Temperature_7To15 = tempList.ToArray();
                            this.Wind1_7To15 = wind1List.ToArray();
                            this.Wind2_7To15 = wind2List.ToArray();
                            this.WeatherStatus_7To15 = statusList.ToArray();
                        }
                    }
                }
            }
        }

        // 去除字符串中的尖括号
        private string RemoveAngleBrackets(string input)
        {
            int firstStartIndex = input.IndexOf('<');
            int firstEndIndex = input.IndexOf('>');
            string result = input.Remove(firstStartIndex, firstEndIndex - firstStartIndex + 1);
            firstStartIndex = result.LastIndexOf('<');
            firstEndIndex = result.LastIndexOf('>');
            result = result.Remove(firstStartIndex, firstEndIndex - firstStartIndex + 1);
            return result;
        }

        // 根据天气信息字符串返回对应的WeatherStatus枚举值
        private WeatherStatus GetWeatherStatus(string weatherInfo)
        {
            switch (weatherInfo)
            {
                case "晴": return WeatherStatus.Qing;
                case "多云": return WeatherStatus.Duoyun;
                case "阴": return WeatherStatus.Yin;
                case "阵雨": return WeatherStatus.Zhenyu;
                case "雷阵雨": return WeatherStatus.Leizhenyu;
                case "雷阵雨并伴有冰雹": return WeatherStatus.LeizhenyuBingpao;
                case "雨夹雪": return WeatherStatus.Yujiaxue;
                case "小雨": return WeatherStatus.Xiaoyu;
                case "中雨": return WeatherStatus.Zhongyu;
                case "大雨": return WeatherStatus.Dayu;
                case "暴雨": return WeatherStatus.Baoyu;
                case "大暴雨": return WeatherStatus.Dabaoyu;
                case "特大暴雨": return WeatherStatus.Tedabaoyu;
                case "阵雪": return WeatherStatus.Zhenxue;
                case "小雪": return WeatherStatus.Xiaoyu;
                case "中雪": return WeatherStatus.Zhongxue;
                case "大雪": return WeatherStatus.Daxue;
                case "暴雪": return WeatherStatus.Baoxue;
                case "雾": return WeatherStatus.Wu;
                case "冻雨": return WeatherStatus.Dongyu;
                case "沙尘暴": return WeatherStatus.Shachenbao;
                case "小雨转中雨": return WeatherStatus.XiaoyuZhuangZhongyu;
                case "中雨转大雨": return WeatherStatus.ZhongyuZhuangDayu;
                case "大雨转暴雨": return WeatherStatus.DayuZhuangBaoyu;
                case "暴雨转大暴雨": return WeatherStatus.BaoyuZhuangDabaoyu;
                case "大暴雨转特大暴雨": return WeatherStatus.DabaoyuZhuangTedabaoyu;
                case "小雪转中雪": return WeatherStatus.XiaoxueZhuangZhongxue;
                case "中雪转大雪": return WeatherStatus.ZhongxueZhuangDaxue;
                case "大雪转暴雪": return WeatherStatus.DaxueZhuangBaoxue;
                case "浮尘": return WeatherStatus.Fuchen;
                case "扬沙": return WeatherStatus.Yasha;
                case "强沙尘暴": return WeatherStatus.Qiangshachenbao;
                case "小到中雨": return WeatherStatus.Xiaodaozhongyu;
                case "中到大雨": return WeatherStatus.Zhongdaodayu;
                case "大到暴雨": return WeatherStatus.Dadaobaoyu;
                case "小到中雪": return WeatherStatus.Xiaodaozhongxue;
                case "中到大雪": return WeatherStatus.Zhongdaodaxue;
                case "大到暴雪": return WeatherStatus.Dadaobaoxue;
                case "小阵雨": return WeatherStatus.Xiaozhenyu;
                case "阴天": return WeatherStatus.Yintian;
                case "霾": return WeatherStatus.Mai;
                case "雾霾": return WeatherStatus.Wumai;
                default: return WeatherStatus.Weizhi;
            }
        }

        // 处理未知的天气状态
        private WeatherStatus GetUnkownWeatherStatus(string weatherInfo)
        {
            string result = weatherInfo;

            int index = 0;
            if (result.Contains("间"))
            {
                index = result.IndexOf("间");
                result = result.Remove(index, result.Length - index);
            }
            if (result.Contains("转"))
            {
                index = result.IndexOf("转");
                result = result.Remove(index, result.Length - index);
            }

            if (result.Contains("雨")) return WeatherStatus.Xiaoyu;
            if (result.Contains("雪")) return WeatherStatus.Xiaoxue;
            if (result.Contains("雾")) return WeatherStatus.Wu;
            if (result.Contains("霾")) return WeatherStatus.Mai;

            return GetWeatherStatus(result);
        }
    }
}
