using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using PmaEntities;

namespace ParseLogs
{
    public class PmaLogProcessor
    {
        private string m_serverUri;
        private const int ChunckSize = 5000;
        public PmaLogProcessor(string serverUri)
        {
            m_serverUri = serverUri;
        }
        public void ProcessLogs(string protectedLogFileName, string recoveryLogFileName)
        {
            PmaLogParser logParser = new PmaLogParser();
            List<PmaRawEntity> protectedRawList = logParser.Parse(protectedLogFileName);
            List<PmaRawEntity> recoveryRawList = logParser.Parse(recoveryLogFileName);
            PmaRawEntitiesInterpulator interpolator = new PmaRawEntitiesInterpulator();
            List<PmaRawEntity> protectedPmaRawEntities = interpolator.BuildInterpolatedList(protectedRawList);
            List<PmaRawEntity> recoveryPmaRawEntities = interpolator.BuildInterpolatedList(recoveryRawList);

            List<PmaRawEntity> mergedPmaRawEntities = interpolator.MergeLists(protectedPmaRawEntities,
                recoveryPmaRawEntities);

            SendDataToFile(logParser.GetPmaHeaders(), mergedPmaRawEntities);
          //  SendDataToServerInChuncks(mergedPmaRawEntities);
        }

        private void SendDataToFile(List<string> headers, List<PmaRawEntity> pmaEntities)
        {
            using (var streamWriter = new StreamWriter("C:\\pma\\output.csv"))
            {
                streamWriter.WriteLine(string.Join(",", headers.ToArray()));
                foreach (PmaRawEntity pmaEntity in pmaEntities)
                {
                    streamWriter.WriteLine(pmaEntity.ToString());
                }
                streamWriter.Flush();
                streamWriter.Close();
            }
        }

        private void SendDataToServerInChuncks(List<PmaRawEntity> pmaEntities)
        {
            int startInd = 0;
            while (startInd < pmaEntities.Count)
            {
                SendDataToServer(pmaEntities.GetRange(startInd, Math.Min(pmaEntities.Count - startInd, ChunckSize)));
                startInd += ChunckSize;
            }
        }

        private void SendDataToServer(List<PmaRawEntity> pmaEntities)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(m_serverUri);
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
