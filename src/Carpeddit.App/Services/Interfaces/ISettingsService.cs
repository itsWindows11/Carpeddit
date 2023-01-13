using System;
using System.Diagnostics.Contracts;

namespace Carpeddit.App.Services
{
    public interface ISettingsService
    {
        /// <summary>
        /// Assigns a value to a settings key.
        /// </summary>
        /// <typeparam name="T">The type of the object bound to the key.</typeparam>
        /// <param name="key">The key to check.</param>
        /// <param name="value">The value to assign to the setting key.</param>
        void SetValue<T>(string key, T value);

        /// <summary>
        /// Reads a value from the current <see cref="IServiceProvider"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of the object to retrieve.</typeparam>
        /// <param name="key">The key associated to the requested object.</param>
        /// <returns>The requested object.</returns>
        [Pure]
        T GetValue<T>(string key);
    }
}
