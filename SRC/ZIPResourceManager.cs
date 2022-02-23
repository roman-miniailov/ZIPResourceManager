// ***********************************************************************
// <copyright file="ZIPResourceManager.cs" company="Roman Minyaylov">
//     Copyright (c) 2022
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.IO;

namespace ZIPResourceManager
{
    /// <summary>
    /// Class ZIPResourceManager.
    /// </summary>
    public class ZIPResourceManager
    {
        /// <summary>
        /// The resource manager.
        /// </summary>
        private readonly System.Resources.ResourceManager _resourceManager;

        /// <summary>
        /// The culture.
        /// </summary>
        private readonly CultureInfo _culture;

        /// <summary>
        /// Gets the resources.
        /// </summary>
        /// <value>The resources.</value>
        public Dictionary<string, object> Resources { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZIPResourceManager"/> class.
        /// </summary>
        /// <param name="resourceManager">The resource manager.</param>
        /// <param name="culture">The culture.</param>
        public ZIPResourceManager(System.Resources.ResourceManager resourceManager, CultureInfo culture)
        {
            _resourceManager = resourceManager;
            _culture = culture;

            if (_culture == null)
            {
                _culture = CultureInfo.CurrentUICulture;
            }

            Resources = new Dictionary<string, object>();

            Load();
        }

        /// <summary>
        /// Determines whether the specified data is zip.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns><c>true</c> if the specified data is zip; otherwise, <c>false</c>.</returns>
        private bool IsZIP(byte[] data)
        {
            if (data[0] == 0x50 && data[1] == 0x4b && data[2] == 0x03 && data[3] == 0x04)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        private void Load()
        {
            var resourceSet = _resourceManager.GetResourceSet(_culture, true, true);
            foreach (DictionaryEntry entry in resourceSet)
            {
                string resourceKey = entry.Key.ToString();
                if (entry.Value is byte[] data)
                {
                    if (IsZIP(data))
                    {
                        LoadZIP(resourceKey, data);
                    }
                    else
                    {
                        Resources.Add(resourceKey, data);
                    }
                }
                else
                {
                    Resources.Add(resourceKey, entry.Value);
                }
            }
        }

        /// <summary>
        /// Loads the zip.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="data">The data.</param>
        private void LoadZIP(string name, byte[] data)
        {
            try
            {
                using (var zipMemoryStream = new MemoryStream(data))
                {
                    using (var zip = new ZipArchive(zipMemoryStream, ZipArchiveMode.Read))
                    {
                        foreach (var entry in zip.Entries)
                        {
                            using (var stream = entry.Open())
                            {
                                using (var ms = new MemoryStream())
                                {
                                    stream.CopyTo(ms);
                                    ms.Position = 0; // rewind
                                    var entryData = new byte[ms.Length];
                                    ms.Read(entryData, 0, entryData.Length);

                                    Resources.Add(entry.Name, entryData);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to load ZIP resource file: {name}");
            }
        }

        /// <summary>
        /// Gets the text value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        public string GetTextValue(string name)
        {
            if (!Resources.ContainsKey(name))
            {
                return null;
            }

            if (!(Resources[name] is byte[]))
            {
                return null;
            }

            return System.Text.Encoding.UTF8.GetString((byte[])Resources[name]);
        }
    }
}
