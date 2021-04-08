# IniParserLibrary

### Description

Simple INI file parser library written in C# (.NET Standart v2.1).

-----

### Format

The basic element contained in an ini file is the key. Every key has a name and a value, delimited by an equals sign '='. The name appears to the left of the equals sign.

The characters equal sign '=' or semi colon ';' or '#' are reserved characters, and cannot be used in section names, key names or key values.

Section and key names are case sensitive.

Start of line comments and inline comments using ';' or '#' character are ignored.

-----

### Usage

###### Instantiate IniParser:
```csharp
var ini = new IniParser(iniFilePath);
```

###### Get key value:
```csharp
string? keyValue = ini.GetValue("Section", "Key"); // returns null, if key doesn't exist
```

###### Set key value:
```csharp
ini.SetValue("Section", "Key", "Value");
```

###### Delete key:
```csharp
ini.DeleteKey("Section", "Key"); // returns true, if existing key was deleted
```

###### Get all keys and their values for the specific section:
```csharp
Dictionary<string, string?> keysAndValues = ini.GetSectionKeysAndValues("Section");
```

###### Save/commit changes to ini file:
```csharp
ini.SaveIni();
```

-----

### Options

###### Enable/disable ini file autosave functionality (no need to call SaveIni method after SetValue or DeleteKey) after class has been instantiated:
```csharp
ini.EnableIniAutoSave();
ini.DisableIniAutoSave();
```

###### Reload ini file:
```csharp
ini.ReloadIni();
```
