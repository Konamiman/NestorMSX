using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Konamiman.NestorMSX.Exceptions;

namespace Konamiman.NestorMSX.Misc
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Gets the value of the configuration for the specified key, converted
        /// to the specified type. If direct conversion is not possible,
        /// a suitable method will be searched for in the Convert class.
        /// </summary>
        /// <typeparam name="T">The type to get the configuration value converted to</typeparam>
        /// <param name="config">The dictionary containing the configuration</param>
        /// <param name="key">The key whose value is to be retrieves</param>
        /// <returns>The configuration value, appropriately converted</returns>
        /// <exception cref="ConfigurationException">The key does not exist, or the conversion failed</exception>
        public static T GetValue<T>(this IDictionary<string, object> config, string key)
        {
            if(!config.ContainsKey(key))
                throw new ConfigurationException($"Configuration key '{key}' not found");

            return AdaptValue<T>(key, config[key]);
        }

        /// <summary>
        /// Gets the value of the configuration for the specified key or a suitable default value, converted
        /// to the specified type. If direct conversion is not possible,
        /// a suitable method will be searched for in the Convert class.
        /// </summary>
        /// <typeparam name="T">The type to get the configuration value converted to</typeparam>
        /// <param name="config">The dictionary containing the configuration</param>
        /// <param name="key">The key whose value is to be retrieves</param>
        /// <param name="defaultValue">The value to return if the key does not exist</param>
        /// <returns>The configuration value or the default value, appropriately converted</returns>
        /// <exception cref="ConfigurationException">The conversion failed</exception>
        public static T GetValueOrDefault<T>(this IDictionary<string, object> config, string key, T defaultValue = default(T))
        {
            var value = config.ContainsKey(key) ? config[key] : defaultValue;

            return AdaptValue<T>(key, value);
        }

        /// <summary>
        /// Gets the value of the configuration for the specified key as a dictionary.
        /// If the key does not exist, returns an empty dictionary. 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IDictionary<string, object> GetDictionaryOrDefault(this IDictionary<string, object> config, string key)
        {
            if (!config.ContainsKey(key))
                return new Dictionary<string, object>();

            var value = config[key];

            if (!(value is IDictionary<string, object>))
                throw new ConfigurationException($"Configuration key '{key}' has value '{value.ToString()}', that can't be converted to a dictionary");

            return (IDictionary<string, object>)value;
        }

        /// <summary>
        /// Gets the full path of a file that is placed in the machine directory,
        /// in the shared directory, or in the application directory.
        /// </summary>
        /// <param name="config">The dictionary containing the configuration,
        /// it is assumed to be the dictionary passed by NestorMSX to the plugin constructor.</param>
        /// <param name="fileName">The name of the file</param>
        /// <param name="throwIfFileNotFound">If true, a <see cref="ConfigurationException"/>will be thrown
        /// if the file is not found. Otherwise, null will be returned.</param>
        /// <returns>The full path of the file in the machine directory
        /// or in the shared directory, wherever the file is found first</returns>
        /// <exception cref="ConfigurationException">File not found in either directory</exception>
        public static string GetMachineFilePath(this IDictionary<string, object> config, string fileName, bool throwIfFileNotFound = true)
        {
            var searchLocationKeys = new[]
            {
                "NestorMSX.machineDirectory",
                "NestorMSX.sharedDirectory",
                "NestorMSX.applicationDirectory"
            };

            foreach(var key in searchLocationKeys) {
                if(!config.ContainsKey(key))
                    continue;

                var filePath = Path.Combine(config.GetValue<string>(key), fileName);
                if(File.Exists(filePath))
                    return filePath;
            }

            if(throwIfFileNotFound)
                throw new ConfigurationException($"Machine file not found: {fileName}");
            else
                return null;
        }

        /// <summary>
        /// Gets the full path of a file that is placed in the machine directory,
        /// in the shared directory, in the application directory, or a certain data directory.
        /// </summary>
        /// <param name="config">The dictionary containing the configuration,
        /// it is assumed to be the dictionary passed by NestorMSX to the plugin constructor.</param>
        /// <param name="fileName">The name of the file</param>
        /// <param name="throwIfFileNotFound">If true, a <see cref="ConfigurationException"/>will be thrown
        /// if the file is not found. Otherwise, the file path will be returned (relative to the data directory).</param>
        /// <param name="basePath">The data directory to search the file in.
        /// If null or not specified, <see cref="StringExtensions.DefaultBasePath"/> is used.</param>
        /// <returns>The full path of the file in the machine directory
        /// or in the shared directory, wherever the file is found first</returns>
        /// <exception cref="ConfigurationException">File not found in either directory</exception>
        /// <remarks>The default value for <see cref="StringExtensions.DefaultBasePath"/> is set
        /// from the "dataDirectory" key in the "sharedPluginsConfig" section in either machine.config
        /// or NestorMSX.Config. If no such key exists, "$MyDocuments$\NestorMSX" is used.</remarks>
        public static string GetMachineOrDataFilePath(this IDictionary<string, object> config, string fileName,
            string basePath = null, bool throwIfFileNotFound = true)
        {
            var path = config.GetMachineFilePath(fileName, false);
            if(path != null)
                return path;

            path = fileName.AsAbsolutePath(basePath);
            if(File.Exists(path))
                return path;

            path = fileName.AsApplicationFilePath();
            if (File.Exists(path) || !throwIfFileNotFound)
                return path;

            throw new ConfigurationException($"File file not found: {fileName}");
        }

        private static T AdaptValue<T>(string key, object value)
        {
            return (T)AdaptValue(typeof(T), key, value);
        }

        private static object AdaptValue(Type destinationType, string key, object value)
        {
            if(value == null)
                return null;

            if(destinationType.IsInstanceOfType(value))
                return value;

            if(destinationType.IsArray)
            {
                if(!value.GetType().IsArray)
                    throw new ConfigurationException($"Configuration key '{key}' has value '{value.ToString()}', that can't be converted to an array");

                var originalArray = (Array)value;
                var arrayItemsType = destinationType.GetElementType();

                var arrayLength = originalArray.GetLength(0);
                var adaptedArray = Array.CreateInstance(arrayItemsType, arrayLength);
                for(int i = 0; i < arrayLength; i++)
                    adaptedArray.SetValue(AdaptValue(arrayItemsType, key, originalArray.GetValue(i)), i);

                return adaptedArray;
            }

            MethodInfo convertMethod = null;
            object[] convertMethodArguments;
            if(value is string)
            {
                convertMethod = GetConvertMethod(destinationType, new[] {typeof(string), typeof(int)});
                if(convertMethod == null)
                {
                    convertMethod = GetConvertMethod(destinationType, new[] {typeof(object)});
                    convertMethodArguments = new object[] { value };
                }
                else
                {
                    var stringValue = (string)value;
                    if(stringValue.StartsWith("0x"))
                        convertMethodArguments = new object[] { stringValue.Substring(2), 16 };
                    else if (stringValue.StartsWith("#"))
                        convertMethodArguments = new object[] { stringValue.Substring(1), 16 };
                    else
                        convertMethodArguments = new object[] { stringValue, 10 };
                }
            }
            else
            {
                convertMethod = GetConvertMethod(destinationType, new[] { typeof(object) });
                convertMethodArguments = new object[] { value };
            }
            
            if(convertMethod == null)
                throw new ConfigurationException($"Configuration key '{key}' has value '{value.ToString()}', that can't be converted to '{destinationType.Name}'");

            try
            {
                return convertMethod.Invoke(null, convertMethodArguments);
            }
            catch(TargetInvocationException ex)
            {
                throw new ConfigurationException(
                    $"Error when converting value {value.ToString()} for key '{key}' to {destinationType.Name}: {ex.InnerException.Message}",
                    ex.InnerException);
            }
        }

        private static MethodInfo GetConvertMethod(Type destinationType, Type[] argumentTypes)
        {
            return typeof(Convert).GetMethod(
                "To" + destinationType.Name,
                BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly,
                null,
                CallingConventions.Any,
                argumentTypes, null);
        }

        /// <summary>
        /// Merges the dictionary into another one. Merging is done by
        /// adding the keys that don't already exist in the destination.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public static void MergeInto(this IDictionary<string, object> source, IDictionary<string, object> destination)
        {
            foreach(var key in source.Keys)
                if(!destination.ContainsKey(key))
                    destination[key] = source[key];
        }
    }
}
