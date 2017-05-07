using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using WikiConnect.Models;

namespace WikiConnect
{
    public delegate void OnSearchStarted(object sender, EventArgs e);
    public delegate void OnResultsFetched(object sender, OnResultsFetchedArgs e);
    public delegate void OnSearchError(object sender, OnSearchErrorArgs e);

    public class OnSearchErrorArgs : EventArgs
    {
        public OnSearchErrorArgs(string message)
        {
            Message = message;
        }

        public string Message;
    }

    public class OnResultsFetchedArgs : EventArgs
    {
        public OnResultsFetchedArgs(SixDegreesModel model)
        {
            Model = model;
        }

        public SixDegreesModel Model;
    }

    public class ResultLoader
    {
        private static readonly string API_KEY = "DkCt0RCDcoPob6Q1K3gl";
        private static SixDegreesModel resultCache;

        public event OnResultsFetched OnSearchResultsFetched;
        public event OnSearchStarted OnSearchStarted;
        public event OnSearchError OnSearchError;

        public ResultLoader() { }

        public async void GetResultAsync(string start, string end)
        {


            string startEncoded = WebUtility.UrlEncode(start);
            string endEncoded = WebUtility.UrlEncode(end);

            var hc = new HttpClient();

            OnSearchStarted?.Invoke(this, EventArgs.Empty);
            try
            {
                var stream = await hc.GetStreamAsync("http://wk0.ca/DegreesReServer/?start=" + startEncoded + "&end=" + endEncoded + "&key=" + API_KEY);
                var serializer = new DataContractJsonSerializer(typeof(SixDegreesModel));
                resultCache = (SixDegreesModel)serializer.ReadObject(stream);
                OnSearchResultsFetched?.Invoke(this, new OnResultsFetchedArgs(resultCache));
            }
            catch (Exception e)
            {
                OnSearchError?.Invoke(this, new OnSearchErrorArgs(e.Message));
            }
        }
    }
}
