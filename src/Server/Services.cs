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
    [ServiceContract]
    public interface IGuiService
    {
        #region OperationContracts
		
        [OperationContract]
        [WebGet(UriTemplate = "?f={filePath}")]
        Stream GetFile(string filePath);

        [OperationContract]
        [WebGet(UriTemplate = "aide")]
        Stream GetHelp();

        [OperationContract]
        [WebGet(UriTemplate = "getActiveDatabases", ResponseFormat = WebMessageFormat.Json)]
        List<String> getActiveDatabases();	

        [OperationContract]
        [WebGet(UriTemplate = "getDBentities", ResponseFormat = WebMessageFormat.Json)]
        DBentity[] getDBentities();

        [OperationContract]
        [WebGet(UriTemplate = "getFilters?db={db}", ResponseFormat = WebMessageFormat.Json)]
        object[] getFilters(String db);

        [OperationContract]
        [WebGet(UriTemplate = "getDBentityValues?db={db}&entityID={entity}&parent={parent}", ResponseFormat = WebMessageFormat.Json)]
        object[] getDBentityValues(String db, int entity, String parent); //TODO: non, tout doit être string

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "getActions?db={db}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        DTanswer getActions(String db, Filtre filtre);
		
        #endregion
    }

    public partial class GuiService : IGuiService
    {
        #region ImplementedMethods

        public List<String> getActiveDatabases() {
            return TrayIcon.activeDBs.ToList<String>();
        }

        public DBentity[] getDBentities() {
            return DB.entities;
        }

        public object[] getFilters(String dbName) {
            return TrayIcon.dbs[dbName].getTitres(DB.filtre);
        }

		public object[] getDBentityValues(String dbName, int entityID, String parentValue){
            return TrayIcon.dbs[dbName].getTitres(DB.entities[entityID],parentValue);
		}

        public DTanswer getActions(String db, Filtre filtre)
        {
            UriTemplateMatch uriMatch = WebOperationContext.Current.IncomingRequest.UriTemplateMatch;

            DTrequest request = new DTrequest(uriMatch.QueryParameters);
            if(!request.paramsAreValid()){
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                return null;               
            }

            return request.getData();
        }

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

        public Stream GetFile(string filePath) {
            if (string.IsNullOrEmpty(filePath)){
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                return null;
            }
			
			if(!File.Exists(filePath)){
				WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
				return null;
			}

            WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";

            return File.OpenRead(filePath);
        }
        
		#endregion
    }

}
