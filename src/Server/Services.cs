using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web; // Nécessite le .NET framework 4.0 (pas client profile)
using System.IO;
using System.Net;
using TaskLeader.DAL;
using TaskLeader.GUI;
using TaskLeader.BO;

namespace TaskLeader.Server
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true), ServiceContract]
    public partial class GuiService
    {

        [OperationContract]
        [WebGet(UriTemplate = "getActiveDatabases",
            ResponseFormat = WebMessageFormat.Json)]
        public List<String> getActiveDatabases() {
            return TrayIcon.activeDBs.ToList<String>();
        }

        [OperationContract]
        [WebGet(UriTemplate = "getDBentities",
            ResponseFormat = WebMessageFormat.Json)]
        public DBentity[] getDBentities() {
            return DB.entities;
        }

        [OperationContract]
        [WebGet(UriTemplate = "getFilters?db={dbName}",
            ResponseFormat = WebMessageFormat.Json)]
        public object[] getFilters(String dbName) {
            return TrayIcon.dbs[dbName].getTitres(DB.filtre);
        }

        [OperationContract]
        [WebGet(UriTemplate = "getDBentityValues?db={dbName}&entityID={entityID}&parent={parentValue}",
            ResponseFormat = WebMessageFormat.Json)]
		public object[] getDBentityValues(String dbName, int entityID, String parentValue){
            return TrayIcon.dbs[dbName].getTitres(DB.entities[entityID],parentValue);
		}

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "getActions",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        public DTanswer getActions(DTrequest request)
        {
            if(!request.paramsAreValid()){
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                return null;               
            }

            return request.getData();
        }

        [OperationContract]
        [WebGet(UriTemplate = "aide")]
        public Stream GetHelp()
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream, System.Text.Encoding.UTF8);
            writer.Write(Properties.Resources.webservices);
            writer.Flush();
            stream.Position = 0;

            WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
            return stream;
        }

        [OperationContract]
        [WebGet(UriTemplate = "client/{*filePath}")]
        public Stream GetFile(string filePath) {

            if (string.IsNullOrEmpty(filePath)){
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
                return File.OpenRead("client/index.html");
            }

            if (!File.Exists("client/" + filePath))
            {
				WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
                MemoryStream stream = new MemoryStream();
                StreamWriter writer = new StreamWriter(stream, System.Text.Encoding.UTF8);
                writer.Write("Le fichier client/" + filePath + " n'existe pas");
                writer.Flush();
                stream.Position = 0;
                return stream;
			}

            switch (Path.GetExtension(filePath).ToLower())
            {
                case (".css"):
                    WebOperationContext.Current.OutgoingResponse.ContentType = "text/css";
                    break;
                case (".js"):
                    WebOperationContext.Current.OutgoingResponse.ContentType = "application/javascript";
                    break;
                default:
                    WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
                    break;
            }

            return File.OpenRead("client/"+filePath);
        }
        
    }

}
