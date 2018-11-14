using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using KTA_OPMT_SDK_Helper_Web_Services.ktaCaptureDocumentService;

namespace CaptureDocumentServiceHelper
{
    public class CaptureDocumentServiceHelper
    {
        //Merge all documents in a folder
        public void MergeFolder(string serverlocation, string sessionID, string folderID)
        {
            //Initialize a client
            BasicHttpsBinding binding = new BasicHttpsBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.OpenTimeout = new TimeSpan(0, 30, 0);
            binding.CloseTimeout = new TimeSpan(0, 30, 0);
            binding.SendTimeout = new TimeSpan(0, 30, 0);
            binding.ReceiveTimeout = new TimeSpan(0, 30, 0);

            EndpointAddress endpointAddress = new EndpointAddress("https://" + serverlocation + "/TotalAgility/Services/Sdk/CaptureDocumentService.svc");
            CaptureDocumentServiceClient client = new CaptureDocumentServiceClient(binding, endpointAddress);

            //Get Source Document
            Folder folder = client.GetFolder(sessionID, new ReportingData(), folderID);

            if (folder.Documents.Count < 2)
            {
                return;
            }

            StringCollection documents = new StringCollection();

            //For each
            foreach (Document doc in folder.Documents)
            {
                documents.Add(doc.Id);
            }


            //Merge docs
            client.MergeDocuments(sessionID, documents);

            //Close connection
            client.Close();

        }
    }
}
