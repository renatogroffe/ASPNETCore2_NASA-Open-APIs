using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace SiteDadosNASA.Pages
{
    public class IndexModel : PageModel
    {
        public string dataImagem { get; set; }

        public void OnGet(
            [FromServices]IConfiguration config)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                string baseURL =
                    config.GetSection("NASA_OpenAPIs:BaseURL").Value;
                string key =
                    config.GetSection("NASA_OpenAPIs:Key").Value;
                DateTime dataBase = DateTime.Now.Date.AddDays(
                    new Random().Next(0, 7) * -1);
                HttpResponseMessage response = client.GetAsync(
                    baseURL + "apod?" +
                    $"api_key={key}&" +
                    $"date={dataBase.ToString("yyyy-MM-dd")}").Result;

                response.EnsureSuccessStatusCode();
                string conteudo =
                    response.Content.ReadAsStringAsync().Result;

                dynamic resultado = JsonConvert.DeserializeObject(conteudo);

                ImagemNASA imagem = new ImagemNASA();
                imagem.Data = dataBase;
                imagem.Titulo = resultado.title;
                imagem.Descricao = resultado.explanation;
                imagem.Url = resultado.url;
                imagem.MediaType = resultado.media_type;

                TempData["ImagemNASA"] = imagem;
            }
        }
    }
}