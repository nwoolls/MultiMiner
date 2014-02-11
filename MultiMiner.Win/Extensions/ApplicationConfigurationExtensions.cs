using MultiMiner.Utility.Serialization;
using System.Linq;

namespace MultiMiner.Win.Extensions
{
    public static class ApplicationConfigurationExtensions
    {
        public static Remoting.Server.Data.Transfer.Configuration.Application ToTransferObject(this Data.Configuration.ApplicationConfiguration modelObject)
        {
            Remoting.Server.Data.Transfer.Configuration.Application transferObject = new Remoting.Server.Data.Transfer.Configuration.Application();
            
            ObjectCopier.CopyObject(modelObject, transferObject, "HiddenColumns");
            transferObject.HiddenColumns = modelObject.HiddenColumns.ToArray();

            return transferObject;
        }

        public static Data.Configuration.ApplicationConfiguration ToModelObject(this Remoting.Server.Data.Transfer.Configuration.Application transferObject)
        {
            Data.Configuration.ApplicationConfiguration modelObject = new Data.Configuration.ApplicationConfiguration();

            ObjectCopier.CopyObject(transferObject, modelObject, "HiddenColumns");
            modelObject.HiddenColumns = transferObject.HiddenColumns.ToList();

            return modelObject;
        }
    }
}
