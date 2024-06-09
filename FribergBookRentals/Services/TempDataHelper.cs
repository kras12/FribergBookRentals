using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace FribergBookRentals.Services
{
    /// <summary>
    /// A service class for storing and retrieving temp data between requests. 
    /// </summary>
    public class TempDataHelper : ITempDataHelper
    {
        #region Methods

        /// <summary>
        /// Stores data in a temp data dictionary. 
        /// </summary>
        /// <typeparam name="T">The type of data to store.</typeparam>
        /// <param name="tempData">The temp data dictionary to store the data in.</param>
        /// <param name="key">The key for the data.</param>
        /// <param name="value">The data to store.</param> 
        /// <exception cref="ArgumentException"></exception>
        public void Set<T>(ITempDataDictionary tempData, string key, T value)
        {
            #region Checks

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException($"The value of parameter '{nameof(key)}' can't be empty");
            }

            #endregion

            tempData[key] = JsonConvert.SerializeObject(value);
        }
        /// <summary>
        /// Retrieves data from a temp data dictionary.
        /// </summary>
        /// <typeparam name="T">The type of data to retrieve.</typeparam>
        /// <param name="tempData">The temp data dictionary to retrieve the data from.</param>
        /// <param name="key">The key for the data.</param>
        /// <param name="data">The retrieved data if the operation was successful. Null if the operation failed.</param>
        /// <returns>True if the operation was successful. False if the operation failed.</returns>
        /// <exception cref="ArgumentException"></exception>
        public bool TryGet<T>(ITempDataDictionary tempData, string key, [NotNullWhen(returnValue: true)] out T? data)
        {
            #region Checks

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException($"The value of parameter '{nameof(key)}' can't be empty");
            }

            #endregion

            data = default;

            if (tempData.TryGetValue(key, out object? value))
            {
                Remove(tempData, key);
                data = JsonConvert.DeserializeObject<T>((string)value!);
                return data is not null;
            }

            return false;
        }

        /// <summary>
        /// Removes data from a temp dictionary.
        /// </summary>
        /// <param name="tempData">The temp data dictionary to remove the data from.</param>
        /// <param name="key">The key for the data to remove.</param>
        /// <exception cref="ArgumentException"></exception>
        public void Remove(ITempDataDictionary tempData, string key)
        {
            #region Checks

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException($"The value of parameter '{nameof(key)}' can't be empty");
            }

            #endregion

            tempData.Remove(key);
        }

        #endregion
    }
}
