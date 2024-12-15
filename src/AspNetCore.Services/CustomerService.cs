using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using AspNetCore.Components.Security;
using AspNetCore.Data;
using AspNetCore.Objects;
using System.Security.Claims;
using System.Security.Principal;

namespace AspNetCore.Services;
public class CustomerService : AService
{
    public CustomerService(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}
