using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace libFace
{
    public class API
    {
        string appKey;
        string clientId;

        public API(string appKey, string clientId)
        {
            this.appKey = appKey;
            this.clientId = clientId;
        }

        private string DoRequest(byte[] image)
        {
            using (var client = new HttpClient())
            {
                var form = new MultipartFormDataContent();
                form.Add(new StringContent(this.appKey), "app_key");
                form.Add(new StringContent(this.clientId), "client_id");

                form.Add(new ByteArrayContent(image, 0, image.Length), "img", "img.jpg");
                var response = client.PostAsync("http://api.sightcorp.com/api/detect/", form).Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }

        private static Person parsePerson(JToken person)
        {
            Person result = new Person();

            var face = person["face"];
            result.face.x = face["x"].Value<int>();
            result.face.y = face["y"].Value<int>();
            result.face.w = face["w"].Value<int>();
            result.face.h = face["h"].Value<int>();

            var mood = person["mood"];
            result.mood.confidence = mood["confidence"].Value<int>();
            result.mood.value = Mood.parseMoodValue(mood["value"].Value<string>());

            var expression = person["expressions"];
            result.expression.sadness = expression["sadness"]["value"].Value<int>();
            result.expression.neutral = expression["neutral"]["value"].Value<int>();
            result.expression.disgust = expression["disgust"]["value"].Value<int>();
            result.expression.anger = expression["anger"]["value"].Value<int>();
            result.expression.surprise = expression["surprise"]["value"].Value<int>();
            result.expression.fear = expression["fear"]["value"].Value<int>();
            result.expression.happiness = expression["happiness"]["value"].Value<int>();

            return result;
        }

        public List<Person> Process(byte[] image)
        {
            List<Person> result = new List<Person>();

            string response = DoRequest(image);
            var doc = JObject.Parse(response);
            foreach (var person in doc["persons"].Children())
                result.Add(parsePerson(person));

            return result;
        }
    }
}
