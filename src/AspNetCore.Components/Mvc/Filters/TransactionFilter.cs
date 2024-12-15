using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AspNetCore.Components.Mvc;

public class TransactionFilter : IResourceFilter
{
    private IDbContextTransaction Transaction { get; }

    public TransactionFilter(DbContext context)
    {
        Transaction = context.Database.BeginTransaction();
    }

    public void OnResourceExecuting(ResourceExecutingContext context)
    {
    }
    public void OnResourceExecuted(ResourceExecutedContext context)
    {
        if (context.Exception == null)
            Transaction.Commit();

        Transaction.Dispose();
    }
}
