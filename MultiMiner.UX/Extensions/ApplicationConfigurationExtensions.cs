using MultiMiner.Utility.Serialization;
using System.Linq;

namespace MultiMiner.UX.Extensions
{
    public static class ApplicationConfigurationExtensions
    {
        public static Remoting.Data.Transfer.Configuration.Application ToTransferObject(this UX.Data.Configuration.Application modelObject)
        {
            Remoting.Data.Transfer.Configuration.Application transferObject = new Remoting.Data.Transfer.Configuration.Application();
            
            ObjectCopier.CopyObject(modelObject, transferObject, "HiddenColumns");
            if (modelObject.HiddenColumns != null)
                transferObject.HiddenColumns = modelObject.HiddenColumns.ToArray();

            return transferObject;
        }

        public static UX.Data.Configuration.Application ToModelObject(this Remoting.Data.Transfer.Configuration.Application transferObject)
        {
            UX.Data.Configuration.Application modelObject = new UX.Data.Configuration.Application();

            ObjectCopier.CopyObject(transferObject, modelObject, "HiddenColumns");
            if (transferObject.HiddenColumns != null)
                modelObject.HiddenColumns = transferObject.HiddenColumns.ToList();

            return modelObject;
        }
    }
}
