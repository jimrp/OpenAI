using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static Xamarin.Forms.Internals.Profile;

namespace OpenAI.Services
{
    public class AIAskImage
    {
        public string prompt { get; set; }
        public int n { get; set; }
        public string size { get; set; }
    }
    public class AIImageResponse
    {
        public int created { get; set; }
        public List<Datum> data { get; set; }

        public class Datum
        {
            public string url { get; set; }
        }
    }

    public class AIAsk
    {
        public string model { get; set; }
        public List<Message> messages { get; set; }
    }

    public class Message
    {
        public string role { get; set; }
        public string content { get; set; }
    }

    public class AIResponse
    {
        public string id { get; set; }
        public string @object { get; set; }
        public int created { get; set; }
        public List<Choice> choices { get; set; }
        public Usage usage { get; set; }

        public class Choice
        {
            public int index { get; set; }
            public Message message { get; set; }
            public string finish_reason { get; set; }
        }

        public class Usage
        {
            public int prompt_tokens { get; set; }
            public int completion_tokens { get; set; }
            public int total_tokens { get; set; }
        }
    }

    public class HttpClients
    {
        private HttpClient client;
        // !!! ADD HERE YOUR CHATGPT API KEY !!!
        private string apiKey = "";

        public HttpClients()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            client = new HttpClient(clientHandler);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<Message>> AskQuestion(List<Message> questions)
        {
            try
            {
                client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);

                var quest = new AIAsk
                {
                    model = "gpt-3.5-turbo",
                    messages = questions
                };
                var content = new StringContent(JsonConvert.SerializeObject(quest), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
                var responseRead = await response.Content.ReadAsStringAsync();
                var objResponse = JsonConvert.DeserializeObject<AIResponse>(responseRead);
                if (objResponse != null)
                {
                    List<Message> resp = new List<Message>();
                    if (objResponse.choices != null)
                    {
                        if (objResponse.choices.Count > 0)
                        {
                            for (int i = 0; i < objResponse.choices.Count; i++)
                            {
                                resp.Add(objResponse.choices[i].message);
                            }
                            return resp;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was an exception. " + ex);
            }

            return null;
        }

        public async Task<List<string>> CreateImage(string prompt)
        {
            try
            {
                client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);

                var quest = new AIAskImage
                {
                    prompt = prompt,
                    n = 2,
                    size = "1024x1024"
                };
                var content = new StringContent(JsonConvert.SerializeObject(quest), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://api.openai.com/v1/images/generations", content);
                var responseRead = await response.Content.ReadAsStringAsync();
                var objResponse = JsonConvert.DeserializeObject<AIImageResponse>(responseRead);
                if (objResponse != null)
                {
                    List<string> resp = new List<string>();
                    if (objResponse.data != null)
                    {
                        if (objResponse.data.Count > 0)
                        {
                            for (int i = 0; i < objResponse.data.Count; i++)
                            {
                                resp.Add(objResponse.data[i].url);
                            }
                            return resp;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was an exception. " + ex);
            }
            return null;
        }
    }
}

