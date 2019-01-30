using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text;
using System.Threading;

namespace ConsoleApplication1
{
    class Class1
    {
        public static void Download()
        {
            var noises = new Dictionary<int, DateTime>();
            using (var reader = new StreamReader(@"C:\Users\rvidal\Desktop\Classificação\noise.csv"))
            {
                reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == null) continue;
                    var values = line.Split(';');

                    var dates = values[0].Split('-');
                    var year = int.Parse(dates[0]);
                    var month = int.Parse(dates[1]);
                    var day = int.Parse(dates[2]);
                    var publication = new DateTime(year, month, day);
                    var protocol = int.Parse(values[1]);
                    noises.Add(protocol, publication);
                }
            }

            //Parallel.ForEach(noises, noise =>
            //{

            //});

            foreach (var noise in noises)
            {
                var protocolo = noise.Key;
                var url = $"http://siteempresas.bovespa.com.br/consbov/VisualizaArquivo.asp?protocolo={protocolo}";
                var type = string.Empty;
                var path = $@"C:\Users\rvidal\Desktop\noise\";

                using (var httpClient = new HttpClient())
                {
                    var count = 0;
                    while (string.IsNullOrEmpty(type) && count < 10)
                    {
                        try
                        {
                            var response = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, url)).Result;
                            type = response.Content.Headers.ContentType.MediaType;
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine($"{noise} - {exception.Message}");
                            Thread.Sleep(1000);
                            count++;
                        }
                    }
                }

                using (var client = new WebClient())
                {
                    var file = string.Empty;
                    var count = 0;
                    while (string.IsNullOrEmpty(file) && count < 100)
                    {
                        switch (type)
                        {
                            case "application/pdf":
                                file = path + $"{protocolo}.pdf";
                                try
                                {
                                    Directory.CreateDirectory(path);
                                    if (!File.Exists(file))
                                        client.DownloadFile(url, file);
                                }
                                catch (Exception exception)
                                {
                                    Console.WriteLine($"{noise} - {exception.Message}");
                                    Thread.Sleep(1000);
                                    file = string.Empty;
                                    count++;
                                }
                                break;
                            default:
                                count = 100;
                                break;


                        }
                    }
                }
            }

        }



        // PUBLICATION;PROTOCOL
        // 2017-12-31;549350
        // 2017-12-31;549620
        // 2017-12-31;549621
        // 2017-12-31;552626
        // 2017-12-31;552633
        // 2017-12-31;554137
        // 2017-12-31;554632
        // 2017-12-31;555418
        // 2017-12-31;555782
        // 2017-12-31;587197
    }
}
