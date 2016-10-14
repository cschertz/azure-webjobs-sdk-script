// ---------------------------------------------------------------------------------
// <copyright file="ConfigurationReader.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------

namespace WebJobs.Script.SFHost
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Fabric;
    using System.Fabric.Description;
    using System.Net;
    using System.Reflection;
    using System.Security;

    // This class implements the functionality to read configuration from Settings.xml
    public static class ConfigurationReader
    {
        private static ICodePackageActivationContext codePackageActivationContext;

        public static void Initialize(ServiceContext serviceContext)
        {
            StatefulServiceContext initParams = serviceContext as StatefulServiceContext;
            if (initParams != null)
            {
                codePackageActivationContext = initParams.CodePackageActivationContext;
            }
            else
            {
                // We know this cant be null if we got here 
                StatelessServiceContext statelessParams = serviceContext as StatelessServiceContext;
                codePackageActivationContext = serviceContext.CodePackageActivationContext;
            }
        }
#pragma warning disable 618 // service needs to run on V2.1 cluster, so use obsolete API 

        // CodePackageActivationContext.GetConfigurationPackage
        public static T GetConfigValue<T>(string configPackageName, string sectionName, string parameterName, T defaultValue, bool isOptional = false)
        {
            T value = defaultValue;

            ConfigurationPackage configPackageDesc = codePackageActivationContext.GetConfigurationPackageObject(configPackageName);
            if (configPackageDesc != null)
            {
                ConfigurationSettings configSettings = configPackageDesc.Settings;
                if ((null != configSettings) && (null != configSettings.Sections) && configSettings.Sections.Contains(sectionName))
                {
                    ConfigurationSection section = configSettings.Sections[sectionName];
                    if ((null != section.Parameters) && section.Parameters.Contains(parameterName))
                    {
                        ConfigurationProperty property = section.Parameters[parameterName];
                        string propertyValue = null;

                        // TODO add support for returning the secure string if the T is secure string
                        if (property.IsEncrypted)
                        {
                            try
                            {
                                SecureString decrtypedValue = property.DecryptValue();

                                // Hack to get the string value for now 
                                propertyValue = new NetworkCredential(string.Empty, decrtypedValue).Password;
                            }
                            catch (Exception e)
                            {
                                ServiceEventSource.Current.Informational("32836006-9340-46A9-875B-CCEEFA3508B7", "Failed to read encrtyped value {0}", e);
                                if (!isOptional)
                                {
                                    throw;
                                }

                                return defaultValue;
                            }
                        }
                        else
                        {
                            propertyValue = property.Value;
                        }

                        if (!TryParse(propertyValue, out value))
                        {
                            value = defaultValue;
                        }
                    }
                }
            }

            return value;
        }

#pragma warning restore 618

        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Double.Parse(System.String)", Justification = "This is fine these are config settings")]
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.Parse(System.String)", Justification = "This is fine these are config settings")]
        private static object Parse(string value, Type type)
        {
            Preconditions.CheckNotNull(value, "value can not be null");
            Preconditions.CheckNotNull(type, "type can not be null");

            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            try
            {
                if (type == typeof(string))
                {
                    return value;
                }

                if (type == typeof(bool))
                {
                    return bool.Parse(value);
                }

                if (type == typeof(int))
                {
                    return int.Parse(value);
                }

                if (type == typeof(double))
                {
                    return double.Parse(value);
                }

                if (type == typeof(Uri))
                {
                    return new Uri(value);
                }

                if (type.IsEnum)
                {
                    return Enum.Parse(type, value, true);
                }

                if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)))
                {
                    return value.Length == 0 ? null : Parse(value, type.GetGenericArguments()[0]);
                }

                if (type.IsArray && (type.GetElementType() == typeof(byte)))
                {
                    return Convert.FromBase64String(value);
                }

                MethodInfo parseMethod = type.GetMethod("Parse", new[] { typeof(string) });

                return parseMethod.Invoke(null, new object[] { value });
            }
            catch (ArgumentException)
            {
                // CommonServiceEventSource.Current.Message("{0}: Invalid argument.", EntityName);
                ServiceEventSource.Current.Message("F775DEC4-E2DB-40A8-8C82-938B094F8FA8", "Invalid argument.");

                return null;
            }
            catch (FormatException)
            {
                return null;
            }
            catch (TargetException)
            {
                // CommonServiceEventSource.Current.Message("{0}: Target of the Parse method is invalid.", EntityName);
                ServiceEventSource.Current.Informational("201D1595-6A71-49D5-A2CF-8048DA658B53", "Target of the Parse method is invalid.");
                return null;
            }
        }

        /// <summary>
        ///     Parse the string and return a typed settingValue.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "makes the name more specific")]
        private static bool TryParse<TValue>(string valueString, out TValue value)
        {
            Preconditions.CheckNotNullOrWhiteSpace(valueString, "valueString can not be null or empty");

            object result = Parse(valueString, typeof(TValue));
            if (result == null)
            {
                value = default(TValue);
                return false;
            }

            value = (TValue)result;
            return true;
        }
    }
}