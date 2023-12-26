using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace WeatherApp
{
    public partial class Form1 : Form
    {
        private HttpClient httpClient;
        private const string ApiBaseUrl = "https://api.weatherapi.com/v1/forecast.json";
        private const string ApiKey = "55081ae9ed64409aa62171059231912";

        public Form1()
        {
            InitializeComponent();
            httpClient = new HttpClient();
        }

        public class WeatherForecast
        {
            public LocationData Location { get; set; }
            public ForecastData Forecast { get; set; }
        }

        public class LocationData
        {
            public string Name { get; set; }
            public string Region { get; set; }
            public string Country { get; set; }
            public double Lat { get; set; }
            public double Lon { get; set; }
            public string Tz_id { get; set; }
            public long Localtime_epoch { get; set; }
            public string Localtime { get; set; }
        }

        public class ForecastData
        {
            public List<ForecastDayData> Forecastday { get; set; }
        }

        public class ForecastDayData
        {
            public string Date { get; set; }
            public DayData Day { get; set; }
        }

        public class DayData
        {
            public double Maxtemp_c { get; set; }
            public double Mintemp_c { get; set; }
            public double Avgtemp_c { get; set; }
            public ConditionData Condition { get; set; }
        }

        public class ConditionData
        {
            public string Text { get; set; }
            public string Icon { get; set; }
            public int Code { get; set; }
        }

        private async Task GetDataFromApi(string location)
        {
            string apiUrl = $"{ApiBaseUrl}?key={ApiKey}&q={location}&lang=pl&days=7";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonData = await response.Content.ReadAsStringAsync();

                        WeatherForecast weatherForecast = JsonConvert.DeserializeObject<WeatherForecast>(jsonData);

                        int dayIndex = 0;

                        var sortedForecast = weatherForecast.Forecast.Forecastday.OrderBy(d => DateTime.Parse(d.Date)).ToList();

                        foreach (var day in sortedForecast)
                        {
                            if (dayIndex < 7)
                            {
                                string dayOfWeek = DateTime.Parse(day.Date).ToString("dddd", new CultureInfo("pl-PL"));
                                string labelText = $"{day.Day.Avgtemp_c} °C\n{dayOfWeek} \n{day.Date}  ";

                                var pictureBox = Controls.Find($"pictureBox{dayIndex + 1}", true)[0] as PictureBox;

                                if (pictureBox != null)
                                {
                                    pictureBox.ImageLocation = $"https:{day.Day.Condition.Icon}";
                                }

                                Controls.Find($"label{dayIndex + 1}", true)[0].Text = labelText;

                                dayIndex++;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show($"B³¹d podczas pobierania danych: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Wyst¹pi³ b³¹d: {ex.Message}");
                }
            }
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            string location = textBox1.Text.Trim();

            if (!string.IsNullOrEmpty(location))
            {
                await GetDataFromApi(location);
            }
            else
            {
                MessageBox.Show("Pole nie mo¿e byæ puste");
            }
        }
    }
}
