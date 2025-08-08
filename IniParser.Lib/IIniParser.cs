using System.Collections.Generic;

namespace IniParser.Lib;

public interface IIniParser
{
    bool ChangesPending { get; }

    bool IniAutoSaveEnabled { get; }

    string IniFilePath { get; }

    bool DeleteKey(string section, string sectionKey);

    void DisableIniAutoSave();

    void EnableIniAutoSave();

    Dictionary<string, string?> GetSectionKeysAndValues(string section);

    string? GetValue(string section, string sectionKey);

    void ReloadIni();

    void SaveIni();

    void SetValue(string section, string sectionKey, string keyValue);
}
