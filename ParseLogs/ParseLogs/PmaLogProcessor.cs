using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using PmaEntities;

namespace ParseLogs
{
    public class PmaLogProcessor
    {
        public void ProcessLogs(string protectedLogFileName, string recoveryLogFileName)
        {
            PmaLogParser logParser = new PmaLogParser();
            List<PmaRawEntity> protectedRawList = logParser.Parse(protectedLogFileName);
            List<PmaRawEntity> recoveryRawList = logParser.Parse(recoveryLogFileName);
            PmaRawEntitiesInterpulator interpulator = new PmaRawEntitiesInterpulator();
            List<PmaRawEntity> protectedPmaRawEntities = interpulator.ProcessRawList(protectedRawList);
            List<PmaRawEntity> recoveryPmaRawEntities = interpulator.ProcessRawList(recoveryRawList);

            List<PmaRawEntity> mergedPmaRawEntities = interpulator.MergeLists(protectedPmaRawEntities,
                recoveryPmaRawEntities);

            SendDataToServer(mergedPmaRawEntities);
        }

        private void SendDataToServer(List<PmaRawEntity> pmaEntities)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:57904/api/pma");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {

                // JObject o = (JObject) JToken.FromObject(pmaEntities);
                string json = JsonConvert.SerializeObject(pmaEntities);//"{\"user\":\"test\"," +
                                                                       //"\"password\":\"bla\"}";

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }
        }
    }
}
