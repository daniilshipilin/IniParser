using System.Collections.Generic;
using System.IO;
using System.Text;
using IniParser.Lib.Exceptions;

namespace IniParser.Lib;

/// <summary>
/// IniParser class.
/// </summary>
public class IniParser : IParser
{
    /// <summary>
    /// Section key pair/value dictionary.
    /// </summary>
    private readonly Dictionary<SectionKeyPair, string> keyPairs = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="IniParser"/> class.
    /// </summary>
    /// <exception cref="FileNotFoundException">Thrown when the provided ini file path doesnt exist.</exception>
    public IniParser(string iniFilePath)
    {
        if (!File.Exists(iniFilePath))
        {
            throw new FileNotFoundException(nameof(iniFilePath));
        }

        this.IniFilePath = iniFilePath;
        this.EnumerateSectionKeyPairs();
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
        return this.keyPairs.TryGetValue(skp, out string keyValue) ? keyValue : null;
    }

    /// <summary>
    /// Adds/replaces a key value for given section.
    /// </summary>
    public void SetValue(string section, string sectionKey, string keyValue)
    {
        var skp = new SectionKeyPair(section, sectionKey);

        if (this.keyPairs.ContainsKey(skp))
        {
            // replace existing key value
            this.keyPairs[skp] = keyValue.Trim();
        }
        else
        {
            this.keyPairs.Add(skp, keyValue.Trim());
        }

        this.ChangesPending = true;
        this.CheckAutoSaveRequired();
    }

    /// <summary>
    /// Gets keys and their values for given section.
    /// </summary>
    public Dictionary<string, string?> GetSectionKeysAndValues(string section)
    {
        var keysAndValues = new Dictionary<string, string?>();

        foreach (var skp in this.keyPairs.Keys)
        {
            if (skp.Section.Equals(section))
            {
                string? keyValue = this.GetValue(section, skp.SectionKey);
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

        if (this.keyPairs.ContainsKey(skp))
        {
            this.keyPairs.Remove(skp);
            this.ChangesPending = true;
            keyIsDeleted = true;
        }

        this.CheckAutoSaveRequired();

        return keyIsDeleted;
    }

    /// <summary>
    /// Commit/save settings to ini file.
    /// </summary>
    public void SaveIni()
    {
        var sections = new List<string>();

        foreach (var sectionKeyPair in this.keyPairs.Keys)
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

            foreach (var sectionKeyPair in this.keyPairs.Keys)
            {
                if (sectionKeyPair.Section.Equals(section))
                {
                    sb.AppendLine($"{sectionKeyPair.SectionKey}={this.keyPairs[sectionKeyPair]}");
                }
            }
        }

        File.WriteAllText(this.IniFilePath, sb.ToString(), new UTF8Encoding(false));
        this.ChangesPending = false;
    }

    /// <summary>
    /// Reloads the ini file.
    /// </summary>
    public void ReloadIni()
    {
        this.ChangesPending = false;
        this.EnumerateSectionKeyPairs();
    }

    /// <summary>
    /// Enables ini file automatic commit/save functionality and initiates ini commit/save, if there are pending changes.
    /// </summary>
    public void EnableIniAutoSave()
    {
        this.IniAutoSaveEnabled = true;
        this.CheckAutoSaveRequired();
    }

    /// <summary>
    /// Disables ini file automatic commit/save functionality.
    /// </summary>
    public void DisableIniAutoSave()
    {
        this.IniAutoSaveEnabled = false;
    }

    /// <summary>
    /// Reads the ini file and enumerates its values.
    /// </summary>
    /// <exception cref="IniFileEmptyException">Thrown when the ini file is empty.</exception>
    /// <exception cref="IniEnumerationFailedException">Thrown when the ini key/value pair enumeration failed.</exception>
    private void EnumerateSectionKeyPairs()
    {
        this.keyPairs.Clear();

        string[] iniFile = File.ReadAllLines(this.IniFilePath, Encoding.UTF8);

        if (iniFile.Length == 0)
        {
            throw new IniFileEmptyException($"'{this.IniFilePath}' file is empty");
        }

        string currentSection = string.Empty;

        foreach (string line in iniFile)
        {
            string currentLine = line;

            // check for ';' or '#' chars, and ignore the rest of the string (comments)
            int pos = currentLine.IndexOfAny([';', '#']);

            if (pos >= 0)
            {
                string[] split = currentLine.Split(currentLine[pos]);
                currentLine = split[0];
            }

            currentLine = currentLine.Trim();

            if (currentLine != string.Empty)
            {
                if (currentLine.StartsWith("[") && currentLine.EndsWith("]"))
                {
                    currentSection = currentLine.Substring(1, currentLine.Length - 2);
                }
                else
                {
                    string[] keyPair = currentLine.Split(['='], 2);

                    if (keyPair.Length != 2)
                    {
                        throw new IniEnumerationFailedException("Ini key/value pair enumeration failed");
                    }

                    var skp = new SectionKeyPair(currentSection, keyPair[0].Trim());
                    this.keyPairs.Add(skp, keyPair[1].Trim());
                }
            }
        }
    }

    /// <summary>
    /// Method, that checks whether SaveIni method needs to be called.
    /// </summary>
    private void CheckAutoSaveRequired()
    {
        if (this.IniAutoSaveEnabled && this.ChangesPending)
        {
            this.SaveIni();
        }
    }

    /// <summary>
    /// SectionKeyPair struct.
    /// </summary>
    private readonly struct SectionKeyPair(string section, string sectionKey)
    {
        public readonly string Section = section;
        public readonly string SectionKey = sectionKey;
    }
}
