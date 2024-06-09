using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Diagnostics.CodeAnalysis;

namespace FribergBookRentals.Services
{
    public interface ITempDataHelper
    {
        void Remove(ITempDataDictionary tempData, string key);
        void Set<T>(ITempDataDictionary tempData, string key, T value);
        bool TryGet<T>(ITempDataDictionary tempData, string key, [NotNullWhen(true)] out T? data);
    }
}