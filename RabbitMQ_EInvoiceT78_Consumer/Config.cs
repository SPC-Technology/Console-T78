using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pbs.Helper;
using pbs.UI;

namespace RabbitMQ_EInvoiceT78_Consumer
{
    class Config
    {

        public static void RegisterDataSelectorServices()
        {
            pbs.Helper.UIServices.WaitingPanelService.RegisterUIService(new pbs.UI.WaitingPanelService());

            pbs.Helper.UIServices.OpenFileService.RegisterUIService(new pbs.UI.OpenUrlService());
            pbs.Helper.UIServices.ValueSelectorService.RegisterUIService(new pbs.UI.ValueSelectorUIService());
            pbs.Helper.UIServices.MultiValueSelectorService.RegisterUIService(new pbs.UI.MultiValueSelectorUIService());
            pbs.Helper.UIServices.RangeValueSelectorService.RegisterUIService(new pbs.UI.RangeValueSelectorUIService());

        }


        public static pbs.Helper.DataServices.IFileStorageService GetStorageService()
        {
            var _dmd = pbs.BO.DM.DMD.GetDMDInfo();
            if(_dmd.StorageService.Equals(typeof(pbs.BO.Azure.DocLinkBlobService).ToString(),StringComparison.OrdinalIgnoreCase))
            {
                return new pbs.BO.Azure.DocLinkBlobService(_dmd.AzureShareName, _dmd.AzureDirectory);
            }
            else if(_dmd.StorageService.Equals(typeof(pbs.BO.Azure.DocLinkFileService).ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return new pbs.BO.Azure.DocLinkFileService(_dmd.AzureShareName, _dmd.AzureDirectory);
            }
            else if(_dmd.StorageService.Equals(typeof(pbs.BO.DataServices.SQLFileStreamService).ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return new pbs.BO.DataServices.SQLFileStreamService();
            }
            else
            {
                return new pbs.BO.DataServices.LocalFileService();
            }
        }
        public static void RegisterServices()
        {
            pbs.Helper.AzureServices.TemplateService.RegisterUIService(new pbs.BO.Azure.CloudTemplateService());
            pbs.Helper.AzureServices.FormStorageService.RegisterUIService(new pbs.BO.Azure.CloudFormStorageService());
            pbs.Helper.AzureServices.LayoutService.RegisterUIService(new pbs.BO.Azure.CloudLayoutService());
            pbs.Helper.AzureServices.AzureBlobUploadService.RegisterUIService(new pbs.BO.Azure.AzureStorage());
            pbs.Helper.UIServices.CloudAuthenticationDialogService.RegisterUIService(new pbs.UI.Authentication.CloudAuthenticationDialogService());

            pbs.Helper.ConfigService.DBConnectionService.RegisterService(new pbs.BO.DB.DBConnectionStringService());

            SPC.DocumentAPI.ModuleInitializer.Register();
            SPC.ServicesContainer.Register<SPC.Services.Cloud.IKeyVault>(new pbs.BO.Azure.API.KeyVault_Imp());

            SPC.ServicesContainer.Register<SPC.Services.Cloud.IBlobFactory>(new pbs.BO.Azure.BlobFactory());

            pbs.BO.Azure.ModuleInitializer.Register();
            SPC.ServicesContainer.Register<SPC.Services.Cloud.IStorageQueueFactory>(new pbs.BO.Azure.StorageQueueFactory());

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            };

        }

    }
}
