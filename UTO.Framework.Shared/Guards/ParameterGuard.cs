using System;
using System.Collections.Generic;
using System.Linq;

namespace UTO.Framework.Shared.Guards
{
    /// <summary>
    /// Parameter Guard
    /// </summary>
    public static class ParameterGuard
    {
        /// <summary>
        /// Against Null String Parameter
        /// </summary>
        /// <param name="value"></param>
        public static void AgainstNullStringParameter(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Against Null String Parameter
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        public static void AgainstNullStringParameter(string value, string parameterName)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(parameterName);
        }

        /// <summary>
        /// Against Null Parameter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        public static void AgainstNullParameter<T>(T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Against Null Parameter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        public static void AgainstNullParameter<T>(T value, string parameterName)
        {
            if (value == null)
                throw new ArgumentNullException(parameterName);
        }

        /// <summary>
        /// Against Zero Value
        /// </summary>
        /// <param name="value"></param>
        public static void AgainstZeroValue(int value)
        {
            if (value == 0)
                throw new ArgumentOutOfRangeException(nameof(value));
        }

        /// <summary>
        /// Against Zero Value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        public static void AgainstZeroValue(int value, string parameterName)
        {
            if (value == 0)
                throw new ArgumentOutOfRangeException(parameterName);
        }

        /// <summary>
        /// Against Null Date Value
        /// </summary>
        /// <param name="value"></param>
        public static void AgainstNullDateValue(DateTime value)
        {
            if (value == DateTime.MinValue)
                throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Against Null Date Value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        public static void AgainstNullDateValue(DateTime value, string parameterName)
        {
            if (value == DateTime.MinValue)
                throw new ArgumentNullException(parameterName);
        }

        /// <summary>
        /// Against Illegal Characters In String
        /// </summary>
        /// <param name="value"></param>
        /// <param name="illegalCharacters"></param>
        public static void AgainstIllegalCharactersInString(string value, List<char> illegalCharacters)
        {
            AgainstNullStringParameter(value);

            if (value.All(letter => illegalCharacters.Contains(letter)))
                throw new ArgumentNullException(nameof(value));
        }
    }
}
