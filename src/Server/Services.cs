using System;
using System.ServiceModel;
using System.ServiceModel.Web; // Nécessite le .NET framework 4.0 (pas client profile)
using System.Runtime.Serialization;
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
        [WebGet(UriTemplate = "getDBentities", ResponseFormat = WebMessageFormat.Json)]
        DBentity[] getDBentities(); //TODO: ajouter un ID ?		
		
        [OperationContract]
        [WebGet(UriTemplate = "getDBentityValues?entity={entity}&db={db}", ResponseFormat = WebMessageFormat.Json)]
        object[] getDBentityValues(String entity, String db); //TODO: éventuellement valeur parente			
		
		// Liste des web-services à prévoir		
		// getDatabases() => [DAL.DB] exposer uniquement les attributs 'name' + ne retourner que les DB actives
		
        #endregion
    }

    public partial class GuiService : IGuiService
    {
        #region ImplementedMethods

        public DBentity[] getDBentities()
        {
			return DB.entities;
		}
		
		public object[] getDBentityValues(String entityName, String dbName){
			return TrayIcon.dbs[dbName].getTitres(DB.entities[0]);
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
