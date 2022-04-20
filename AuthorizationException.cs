using System;

namespace CloudHospital.ProductMaintenanceFunctions;

public class AuthorizationException : Exception
{
    public AuthorizationException(string message) : base(message)
    {

    }
}