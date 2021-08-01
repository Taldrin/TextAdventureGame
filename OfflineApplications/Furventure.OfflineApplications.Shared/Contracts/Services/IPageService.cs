using System;

namespace Test.Contracts.Services
{
    public interface IPageService
    {
        Type GetPageType(string key);
    }
}
