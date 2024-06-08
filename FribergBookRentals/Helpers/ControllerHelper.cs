using Microsoft.AspNetCore.Mvc;

namespace FribergBookRentals.Helpers
{
    internal static class ControllerHelper
    {
        public static string GetControllerName<T>() where T : Controller
        {
            return typeof(T).Name.Replace("Controller", "", StringComparison.CurrentCultureIgnoreCase);
        }

        public static string GetControllerName<T>(T type) where T : Controller
        {
            return GetControllerName<T>();
        }
    }
}
