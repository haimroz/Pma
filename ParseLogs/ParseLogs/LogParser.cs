using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO;

namespace ParseLogs
{
    public class LogParser
    {

        public LogParser()
        { }
        public void Parse(string fileFullName)
        {
            JObject logJObject = JObject.Parse(File.ReadAllText(fileFullName));
            List<string> headers = (List<string>)logJObject["headers"][1].ToObject(typeof(List<string>));


            List<string>  data = (List<string>)logJObject["data"][1].ToObject(typeof(List<string>));
        }

 //       public Dicti
    }
}
