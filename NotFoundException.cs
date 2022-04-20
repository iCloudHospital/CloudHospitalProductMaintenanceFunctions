using System;

namespace CloudHospital.ProductMaintenanceFunctions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {

    }
}