using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Konamiman.NestorMSX.Exceptions;
using System.Collections;
using System.IO;

namespace Konamiman.NestorMSX.Misc
{
    public static class ConfigurationUtils
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
        /// Gets the full path of a file that is placed in the machine directory
        /// or in the shared directory.
        /// </summary>
        /// <param name="config">The dictionary containing the configuration</param>
        /// <param name="fileName">The name of the file</param>
        /// <returns>The full path of the file in the machine directory
        /// or in the shared directory, wherever the file is found first</returns>
        /// <exception cref="ConfigurationException">File not found in either directory</exception>
        public static string GetPluginFilePath(this IDictionary<string, object> config, string fileName)
        {
            var filePath = Path.Combine(config.GetValue<string>("NestorMSX.machineDirectory"), fileName);
            if (!File.Exists(filePath))
            {
                filePath = Path.Combine(config.GetValue<string>("NestorMSX.sharedDirectory"), fileName);
                if (!File.Exists(filePath))
                {
                    throw new ConfigurationException($"File not found: {fileName}");
                }
            }

            return filePath;
        }

        private static T AdaptValue<T>(string key, object value)
        {
            return (T)AdaptValue(typeof(T), key, value);
        }

        private static object AdaptValue(Type type, string key, object value)
        {
            if(value == null)
                return null;

            if(type.IsInstanceOfType(value))
                return value;

            if(type.IsArray)
            {
                if(!value.GetType().IsArray)
                    throw new ConfigurationException($"Configuration key '{key}' has value '{value.ToString()}', that can't be converted to an array");

                var originalArray = (Array)value;
                var arrayItemsType = type.GetElementType();

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
                convertMethod = GetConvertMethod(type, new[] {typeof(string), typeof(int)});
                if(convertMethod == null)
                {
                    convertMethod = GetConvertMethod(type, new[] {typeof(object)});
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
                convertMethod = GetConvertMethod(type, new[] { typeof(object) });
                convertMethodArguments = new object[] { value };
            }
            
            if(convertMethod == null)
                throw new ConfigurationException($"Configuration key '{key}' has value '{value.ToString()}', that can't be converted to '{type.Name}'");

            try
            {
                return convertMethod.Invoke(null, convertMethodArguments);
            }
            catch(TargetInvocationException ex)
            {
                throw new ConfigurationException(
                    $"Error when converting value {value.ToString()} for key '{key}' to {type.Name}: {ex.InnerException.Message}",
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

        public static void MergeInto(this IDictionary<string, object> source, IDictionary<string, object> destination)
        {
            foreach(var key in source.Keys)
                if(!destination.ContainsKey(key))
                    destination[key] = source[key];
        }
    }
}
