using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.ServiceModel;
using KTA_OPMT_SDK_Helper_Web_Services.ktaCaptureDocumentService;
using System.Net;

namespace UnitTestFramework
{
    [TestClass]
    public class UnitTest1
    {

        string serverNameOrIP = "kofaxtenant.web.tstest.com";
        string url = "https://kofaxtenant.web.tstest.com/TotalAgility/Services/Sdk/CaptureDocumentService.svc";
        string folderID = "d980ec59-7510-4d43-92e2-a99801163ce7";
        string sessionID = "A70A387DD519B34BB0E354124553E139";

        [TestMethod]
        public void TestMergeFolder()
        {
            
            BasicHttpsBinding binding = new BasicHttpsBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.OpenTimeout = new TimeSpan(0, 30, 0);
            binding.CloseTimeout = new TimeSpan(0, 40, 0);
            binding.SendTimeout = new TimeSpan(0, 30, 0);
            binding.ReceiveTimeout = new TimeSpan(0, 30, 0);

            EndpointAddress endpointAddress = new EndpointAddress(url);
            CaptureDocumentServiceClient client = new CaptureDocumentServiceClient(binding, endpointAddress);

            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            //Get Original Folder
            Folder folderOriginal = client.GetFolder(sessionID, new ReportingData(), folderID);           

            //Merge Documents
            CaptureDocumentServiceHelper.CaptureDocumentServiceHelper cdsh = new CaptureDocumentServiceHelper.CaptureDocumentServiceHelper();

           // CaptureDocumentServiceHelper cdsh = new CaptureDocumentServiceHelper();
            cdsh.MergeFolder(serverNameOrIP, sessionID, folderID);

            //Get Updated Folder
            Folder folderUpdated = client.GetFolder(sessionID, new ReportingData(), folderID);

            //Close connection
            client.Close();
        }
    }
}
