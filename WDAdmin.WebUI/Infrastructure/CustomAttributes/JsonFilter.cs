using System;
using System.IO;
using System.Web.Mvc;
using Newtonsoft.Json;
using WDAdmin.WebUI.Models;
using WDAdmin.Domain.Entities;

namespace WDAdmin.WebUI.Infrastructure
{
    /// <summary>
    /// ActionFilter to help deserialize complex JSON - using JSON.NET
    /// </summary>
    public class JsonFilter : ActionFilterAttribute
    {
        /// <summary>
        /// Gets or sets the parameter.
        /// </summary>
        /// <value>The parameter.</value>
        public string Param { get; set; }
        /// <summary>
        /// Gets or sets the type of the root.
        /// </summary>
        /// <value>The type of the root.</value>
        public Type RootType { get; set; }

        /// <summary>
        /// Called when [action executing].
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Request.InputStream.Position = 0;
            var jsonText = StReader(filterContext.HttpContext.Request.InputStream); //Get JSON string from InputStream

            var data = new object();
            
            try
            {
                if (RootType == typeof(Collection))
                {
                    data = JsonConvert.DeserializeObject<Collection>(jsonText); //Deserialize JSON to Collection
                    Logger.Log("JsonFilter CollectionReceived OK", LogType.JsonStringReceived, LogEntryType.Info);
                }
                else if (RootType == typeof (QrVideoData))
                {
                    data = JsonConvert.DeserializeObject<QrVideoData>(jsonText); //Deserialize JSON to VideoData
                    Logger.Log("JsonFilter VideoDataReceived OK", LogType.JsonStringReceived, LogEntryType.Info);
                }
                else if (RootType == typeof(VideoCategoryData))
                {
                    data = JsonConvert.DeserializeObject<VideoCategoryData>(jsonText); //Deserialize JSON to VideoData
                    Logger.Log("JsonFilter VideoDataReceived OK", LogType.JsonStringReceived, LogEntryType.Info);
                }
                else if (RootType == typeof(VideoUserViewData))
                {
                    data = JsonConvert.DeserializeObject<VideoUserViewData>(jsonText); //Deserialize JSON to VideoUserViewData
                    Logger.Log("JsonFilter VideoUserViewDataReceived OK", LogType.JsonStringReceived, LogEntryType.Info);
                }
                else
                {
                    data = JsonConvert.DeserializeObject<LoginData>(jsonText); //Deserialize JSON to LoginData
                    Logger.Log("JsonFilter LoginDataReceived OK", LogType.JsonStringReceived, LogEntryType.Info);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("JsonFilter Error", ex.Message, LogType.JsonError, LogEntryType.Error);
            }

            filterContext.ActionParameters[Param] = data;
        }

        /// <summary>
        /// Stream reader for InputStream JSON content
        /// </summary>
        /// <param name="s">Input stream</param>
        /// <returns>Output string</returns>
        public string StReader(Stream s)
        {
            s.Position = 0;

            using (var inputStream = new StreamReader(s))
            {
                return inputStream.ReadToEnd();
            }
        }
    }   
}