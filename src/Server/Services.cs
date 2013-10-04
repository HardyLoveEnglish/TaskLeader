using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web; // Nécessite le .NET framework 4.0 (pas client profile)
using System.IO;
using System.Net;
using TaskLeader.DAL;
using TaskLeader.GUI;


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
        object[] getDBentityValues(String db, int entity, String parent);		
		
		// Liste des web-services à prévoir		
		
        #endregion
    }

    public partial class GuiService : IGuiService
    {
        #region ImplementedMethods

        public List<String> getActiveDatabases() { return TrayIcon.activeDBs.ToList<String>(); }
        public DBentity[] getDBentities() { return DB.entities; }

        public object[] getFilters(String dbName) { return TrayIcon.dbs[dbName].getTitres(DB.filtre); }
		public object[] getDBentityValues(String dbName, int entityID, String parentValue){
            return TrayIcon.dbs[dbName].getTitres(DB.entities[entityID],parentValue);
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
