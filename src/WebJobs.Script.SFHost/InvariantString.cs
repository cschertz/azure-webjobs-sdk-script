// ---------------------------------------------------------------------------------
// <copyright file="InvariantString.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------

namespace WebJobs.Script.SFHost
{
    using System.Globalization;

    /// <summary>
    ///     Used for InvariantCulture formats without cluttering up the code so much.
    /// </summary>
    public static class InvariantString
    {
        public static string Format(string formatValue, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, formatValue, args);
        }
    }
}