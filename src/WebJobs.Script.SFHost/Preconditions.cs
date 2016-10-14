// ---------------------------------------------------------------------------------
// <copyright file="Preconditions.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------

namespace WebJobs.Script.SFHost
{
    using System;

    public static class Preconditions
    {
        /// <summary>
        ///     Check an boolean expression.
        /// </summary>
        public static void CheckArgument(bool expression, string errorMessage)
        {
            if (!expression)
            {
                throw new ArgumentException(errorMessage);
            }
        }

        /// <summary>
        ///     Check an boolean expression.
        /// </summary>
        public static void CheckArgument(bool expression, string errorMessageTemplate, params object[] errorMessageArgs)
        {
            if (!expression)
            {
                throw new ArgumentException(InvariantString.Format(errorMessageTemplate, errorMessageArgs));
            }
        }

        /// <summary>
        ///     Check if this is a valid Guid.
        /// </summary>
        public static Guid CheckIsGuid([ValidatedNotNull] string guid, string errorMessage)
        {
            Guid newGuid;
            if (!Guid.TryParse(guid, out newGuid))
            {
                throw new ArgumentNullException(errorMessage);
            }

            return newGuid;
        }

        /// <summary>
        ///     Check if this is a valid guid.
        /// </summary>
        public static Guid CheckIsGuid([ValidatedNotNull] string guid, string errorMessageTemplate, params object[] errorMessageArgs)
        {
            Guid newGuid;
            if (!Guid.TryParse(guid, out newGuid))
            {
                throw new ArgumentNullException(InvariantString.Format(errorMessageTemplate, errorMessageArgs));
            }

            return newGuid;
        }

        public static T CheckNotNull<T>([ValidatedNotNull] T value, string errorMessage)
        {
            if (value == null)
            {
                throw new ArgumentNullException(errorMessage);
            }

            return value;
        }

        public static T CheckNotNull<T>([ValidatedNotNull] T value, string errorMessageTemplate, params object[] errorMessageArgs)
        {
            if (value == null)
            {
                throw new ArgumentNullException(InvariantString.Format(errorMessageTemplate, errorMessageArgs));
            }

            return value;
        }

        public static string CheckNotNullOrWhiteSpace([ValidateNotNullOrEmpty] string value, string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(errorMessage);
            }

            return value;
        }

        public static string CheckNotNullOrWhiteSpace([ValidateNotNullOrEmpty] string value, string errorMessageTemplate, params object[] errorMessageArgs)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(InvariantString.Format(errorMessageTemplate, errorMessageArgs));
            }

            return value;
        }

        internal sealed class ValidatedNotNullAttribute : Attribute
        {
        }

        internal sealed class ValidateNotNullOrEmptyAttribute : Attribute
        {
        }
    }
}