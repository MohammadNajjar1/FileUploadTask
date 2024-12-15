using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AspNetCore.Components.Mvc;

public class ClientValidatorProvider : IClientModelValidatorProvider
{
    private HashSet<Type> IntegerTypes { get; }
    private HashSet<Type> DecimalTypes { get; }

    public ClientValidatorProvider()
    {
        IntegerTypes = new HashSet<Type>
        {
            typeof(Byte),
            typeof(SByte),
            typeof(Int16),
            typeof(UInt16),
            typeof(Int32),
            typeof(UInt32),
            typeof(Int64),
            typeof(UInt64)
        };
        DecimalTypes = new HashSet<Type>
        {
            typeof(Single),
            typeof(Double),
            typeof(Decimal)
        };
    }

    public void CreateValidators(ClientValidatorProviderContext context)
    {
        if (context.ModelMetadata.UnderlyingOrModelType == typeof(DateTime))
            context.Results.Add(new ClientValidatorItem { Validator = new DateValidator(), IsReusable = true });
        else if (DecimalTypes.Contains(context.ModelMetadata.UnderlyingOrModelType))
            context.Results.Add(new ClientValidatorItem { Validator = new NumberValidator(), IsReusable = true });
        else if (IntegerTypes.Contains(context.ModelMetadata.UnderlyingOrModelType))
            context.Results.Add(new ClientValidatorItem { Validator = new IntegerValidator(), IsReusable = true });
    }
}
