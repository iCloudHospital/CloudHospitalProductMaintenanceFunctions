using System.Collections.Generic;

namespace CloudHospital.ProductMaintenanceFunctions;

public class BlobStorageOptions
{
    public string ConnectionString { get; set; }

    public string ContainerName { get; set; }

    public IEnumerable<string> ContainerNames
    {
        get => ContainerName.Split(",", System.StringSplitOptions.RemoveEmptyEntries);
    }
}
