namespace IniParser.Library
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    /// <summary>
    /// IniParser class.
    /// </summary>
    public class IniParser
    {
        /// <summary>
        /// Section key pair/value dictionary.
        /// </summary>
        private readonly Dictionary<SectionKeyPair, string> keyPairs;

        /// <summary>
        /// Initializes a new instance of the <see cref="IniParser"/> class.
        /// </summary>
        public IniParser(string iniFilePath)
        {
            if (!File.Exists(iniFilePath))
            {
                throw new FileNotFoundException(nameof(iniFilePath));
            }

            keyPairs = new Dictionary<SectionKeyPair, string>();
            IniFilePath = iniFilePath;
            EnumerateSectionKeyPairs();
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="SaveIni"/> method should be called automatically after changes have been made.
        /// </summary>
        public bool IniAutoSaveEnabled { get; private set; }

        /// <summary>
        /// Gets the source ini file full path.
        /// </summary>
        public string IniFilePath { get; }

        /// <summary>
        /// Gets a value indicating whether changes have been made.
        /// </summary>
        public bool ChangesPending { get; private set; }

        /// <summary>
        /// Returns the value for the given section key. Returns null, if key doesn't exist.
        /// </summary>
        public string? GetValue(string section, string sectionKey)
        {
            var skp = new SectionKeyPair(section, sectionKey);

            if (keyPairs.TryGetValue(skp, out string keyValue))
            {
                return keyValue;
            }

            return null;
        }

        /// <summary>
        /// Adds/replaces a key value for given section.
        /// </summary>
        public void SetValue(string section, string sectionKey, string keyValue)
        {
            var skp = new SectionKeyPair(section, sectionKey);

            if (keyPairs.ContainsKey(skp))
            {
                // replace existing key value
                keyPairs[skp] = keyValue.Trim();
            }
            else
            {
                keyPairs.Add(skp, keyValue.Trim());
            }

            ChangesPending = true;
            CheckAutoSaveRequired();
        }

        /// <summary>
        /// Gets keys and their values for given section.
        /// </summary>
        public Dictionary<string, string?> GetSectionKeysAndValues(string section)
        {
            var keysAndValues = new Dictionary<string, string?>();

            foreach (var skp in keyPairs.Keys)
            {
                if (skp.Section.Equals(section))
                {
                    string? keyValue = GetValue(section, skp.SectionKey);
                    keysAndValues.Add(skp.SectionKey, keyValue);
                }
            }

            return keysAndValues;
        }

        /// <summary>
        /// Remove a key for given section. Returns true, if existing key was deleted.
        /// </summary>
        public bool DeleteKey(string section, string sectionKey)
        {
            bool keyIsDeleted = false;
            var skp = new SectionKeyPair(section, sectionKey);

            if (keyPairs.ContainsKey(skp))
            {
                keyPairs.Remove(skp);
                ChangesPending = true;
                keyIsDeleted = true;
            }

            CheckAutoSaveRequired();

            return keyIsDeleted;
        }

        /// <summary>
        /// Commit/save settings to ini file.
        /// </summary>
        public void SaveIni()
        {
            var sections = new List<string>();

            foreach (var sectionKeyPair in keyPairs.Keys)
            {
                if (!sections.Contains(sectionKeyPair.Section))
                {
                    sections.Add(sectionKeyPair.Section);
                }
            }

            var sb = new StringBuilder();

            foreach (string section in sections)
            {
                sb.AppendLine($"[{section}]");

                foreach (var sectionKeyPair in keyPairs.Keys)
                {
                    if (sectionKeyPair.Section.Equals(section))
                    {
                        sb.AppendLine($"{sectionKeyPair.SectionKey}={keyPairs[sectionKeyPair]}");
                    }
                }
            }

            File.WriteAllText(IniFilePath, sb.ToString(), new UTF8Encoding(false));
            ChangesPending = false;
        }

        /// <summary>
        /// Reloads the ini file.
        /// </summary>
        public void ReloadIni()
        {
            ChangesPending = false;
            EnumerateSectionKeyPairs();
        }

        /// <summary>
        /// Enables ini file automatic commit/save functionality and initiates ini commit/save, if there are pending changes.
        /// </summary>
        public void EnableIniAutoSave()
        {
            IniAutoSaveEnabled = true;
            CheckAutoSaveRequired();
        }

        /// <summary>
        /// Disables ini file automatic commit/save functionality.
        /// </summary>
        public void DisableIniAutoSave()
        {
            IniAutoSaveEnabled = false;
        }

        /// <summary>
        /// Reads the ini file and enumerates its values.
        /// </summary>
        private void EnumerateSectionKeyPairs()
        {
            keyPairs.Clear();

            using var iniFile = new StreamReader(IniFilePath, Encoding.UTF8);
            string currentSection = string.Empty;
            string currentLine = iniFile.ReadLine();

            while (currentLine != null)
            {
                // check for ';' or '#' chars, and ignore the rest of the string (comments)
                int pos = currentLine.IndexOfAny(new char[] { ';', '#' });

                if (pos >= 0)
                {
                    var split = new List<string>(currentLine.Split(currentLine[pos]));
                    currentLine = split[0];
                }

                currentLine = currentLine.Trim();

                if (currentLine != string.Empty)
                {
                    if (currentLine.StartsWith("[") && currentLine.EndsWith("]"))
                    {
                        currentSection = currentLine[1..^1];
                    }
                    else
                    {
                        var keyPair = new List<string>(currentLine.Split(new char[] { '=' }, 2));

                        if (keyPair.Count != 2)
                        {
                            throw new Exception("Ini key/value pair enumeration failed");
                        }

                        var skp = new SectionKeyPair(currentSection, keyPair[0].Trim());
                        keyPairs.Add(skp, keyPair[1].Trim());
                    }
                }

                currentLine = iniFile.ReadLine();
            }
        }

        /// <summary>
        /// Method, that checks whether SaveIni method needs to be called.
        /// </summary>
        private void CheckAutoSaveRequired()
        {
            if (IniAutoSaveEnabled && ChangesPending)
            {
                SaveIni();
            }
        }

        /// <summary>
        /// SectionKeyPair struct.
        /// </summary>
        private struct SectionKeyPair
        {
            public readonly string Section;
            public readonly string SectionKey;

            public SectionKeyPair(string section, string sectionKey)
            {
                Section = section;
                SectionKey = sectionKey;
            }
        }
    }
}
