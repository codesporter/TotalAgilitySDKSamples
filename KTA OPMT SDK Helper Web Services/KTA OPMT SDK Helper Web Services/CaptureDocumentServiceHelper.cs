using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using KTA_OPMT_SDK_Helper_Web_Services.ktaCaptureDocumentService;
using KTA_OPMT_SDK_Helper_Web_Services.ktaJobService;
using System.Net;
using System.IO;

namespace ktaCaptureDocumentServiceHelper
{
    public class CaptureDocument_ServiceHelper
    {
        public void CreateJobWithDocumentsHelper(string serverlocation, string sessionID, string folderID)
        {

            //Initialize a client
            BasicHttpsBinding binding = new BasicHttpsBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.OpenTimeout = new TimeSpan(0, 30, 0);
            binding.CloseTimeout = new TimeSpan(0, 30, 0);
            binding.SendTimeout = new TimeSpan(0, 30, 0);
            binding.ReceiveTimeout = new TimeSpan(0, 30, 0);

            EndpointAddress endpointAddress = new EndpointAddress("https://" + serverlocation + "/TotalAgility/Services/Sdk/JobService.svc");
            JobServiceClient client = new JobServiceClient(binding, endpointAddress);

            EndpointAddress endpointAddressCDS = new EndpointAddress("https://" + serverlocation + "/TotalAgility/Services/Sdk/CaptureDocumentService.svc");
            CaptureDocumentServiceClient clientCDS = new CaptureDocumentServiceClient(binding, endpointAddressCDS);


            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };


            string imageFolder = @"C:\images\sbd\tiff";



            string[] files = Directory.GetFiles(imageFolder);

            if (files == null && files.Length == 0)
            {
                throw new Exception("Directory is empty.");
            }

            List<KTA_OPMT_SDK_Helper_Web_Services.ktaJobService.PageImageData> pageImageDataCollection = new List<KTA_OPMT_SDK_Helper_Web_Services.ktaJobService.PageImageData>();
            KTA_OPMT_SDK_Helper_Web_Services.ktaCaptureDocumentService.PageImageData pageImageData = new KTA_OPMT_SDK_Helper_Web_Services.ktaCaptureDocumentService.PageImageData();

            string batchid = Guid.NewGuid().ToString();

            foreach (var item in files)
            {

                
                pageImageData = clientCDS.SavePageImage(sessionID, batchid, File.ReadAllBytes(item), "image/tiff");


                KTA_OPMT_SDK_Helper_Web_Services.ktaJobService.PageImageData pageImageData2 = new KTA_OPMT_SDK_Helper_Web_Services.ktaJobService.PageImageData();
                pageImageData2.BatchId = pageImageData.BatchId;
                pageImageData2.ImageId = pageImageData.ImageId;
                pageImageData2.Height = pageImageData.Height;
                pageImageData2.Width = pageImageData.Width;
                pageImageData2.HorizontalResolution = pageImageData.HorizontalResolution;
                pageImageData2.VerticalResolution = pageImageData.VerticalResolution;
                pageImageDataCollection.Add(pageImageData2);
            }
            ProcessIdentity pi = new ProcessIdentity() { Name = "Capture Loan App" };

            JobWithDocumentsInitialization jwdi = new JobWithDocumentsInitialization();

            RuntimeDocument rd = new RuntimeDocument();

                      
            rd.PageImageDataCollection = pageImageDataCollection.ToArray();

            List<RuntimeDocument> documentCollection = new List<RuntimeDocument>();
            documentCollection.Add(rd);


            jwdi.RuntimeDocumentCollection = documentCollection.ToArray();

            client.CreateJobWithDocuments(sessionID, pi, jwdi);
        }
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

            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            //Get root folder
            Folder folder = client.GetFolder(sessionID, new ReportingData(), folderID);

            if (folder.Documents.Count < 2)
            {
                return;
            }

            KTA_OPMT_SDK_Helper_Web_Services.ktaCaptureDocumentService.StringCollection documents = new KTA_OPMT_SDK_Helper_Web_Services.ktaCaptureDocumentService.StringCollection();

            //For each add to documents collection
            foreach (Document doc in folder.Documents)
            {
                documents.Add(doc.Id);
            }

            //Merge all documents in documents collection
            client.MergeDocuments(sessionID, documents);

            //Close connection
            client.Close();

        }
    }
}
