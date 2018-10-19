// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogHelper.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The log helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lite.Tests
{
    using System;
    using System.Collections;

    /// <summary>
    /// The log helper.
    /// </summary>
    public static class LogHelper
    {
        /// <summary>
        /// The write dictionary content.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="intend">
        /// The intend.
        /// </param>
        public static void WriteDictionaryContent(IDictionary value, int intend)
        {
            foreach (DictionaryEntry entry in value)
            {
                WriteDictionaryEntry(entry, intend + 1);
            }
        }

        /// <summary>
        /// The write list.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="intend">
        /// The intend.
        /// </param>
        public static void WriteList(IList value, int intend)
        {
            string intendString = string.Empty.PadLeft(4 * intend, ' ');

            if (value is byte[])
            {
                Console.WriteLine("{0}byte[{1}]", intendString, ((byte[])value).Length);
                return;
            }

            foreach (object element in value)
            {
                Console.WriteLine("{0}{1}", intendString, element);
                if (element is IEnumerable)
                {
                    WriteEnumerableObject(element, intend + 1);
                }
            }
        }

        /// <summary>
        /// The write dictionary entry.
        /// </summary>
        /// <param name="dictionaryEntry">
        /// The dictionary entry.
        /// </param>
        /// <param name="intend">
        /// The intend.
        /// </param>
        private static void WriteDictionaryEntry(DictionaryEntry dictionaryEntry, int intend)
        {
            if (intend > 0)
            {
                Console.Write(string.Empty.PadLeft(4 * intend, ' '));
            }

            Console.WriteLine(string.Format("{0}: {1}", dictionaryEntry.Key, dictionaryEntry.Value));

            if (dictionaryEntry.Value is IEnumerable)
            {
                WriteEnumerableObject(dictionaryEntry.Value, intend + 1);
            }
        }

        /// <summary>
        /// The write enumerable object.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="intend">
        /// The intend.
        /// </param>
        private static void WriteEnumerableObject(object value, int intend)
        {
            if (value is IDictionary)
            {
                WriteDictionaryContent((IDictionary)value, intend);
            }
            else if (value is IList)
            {
                WriteList((IList)value, intend);
            }
        }
    }
}