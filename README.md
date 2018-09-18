# IniParserLibrary

### Description

Simple INI file parser library written in C# (project uses .NET Framework v4.5.2).

-----

### Format

The basic element contained in an ini file is the key. Every key has a name and a value, delimited by an equals sign '='. The name appears to the left of the equals sign.

The characters equal sign '=' or semi colon ';' are reserved characters, and cannot be used in section names, key names or key values.

Section and key names are case sensitive.

Start of line comments and inline comments using ';' character are ignored.

-----

### Usage

###### Instantiate IniParser:
```
var ini = new IniParser(iniFilePath);
```

###### Get key value:
```
string keyValue = ini.GetValue("Section", "Key"); // returns null, if key doesn't exist
```

###### Set key value:
```
ini.SetValue("Section", "Key", "Value");
```

###### Delete key:
```
ini.DeleteKey("Section", "Key"); // returns true, if existing key was deleted
```

###### Save/commit changes to ini file:
```
ini.SaveIni();
```

-----

### Options

###### Instantiate IniParser and enable ini file autosave functionality (no need to call SaveIni method after SetValue or DeleteKey):
```
var ini = new IniParser(iniFilePath)
{
    IniAutoSaveEnabled = true
};
```

###### Enable/disable ini file autosave functionality after class has been instantiated:
```
ini.EnableIniAutoSave();
ini.DisableIniAutoSave();
```

###### Reload ini file:
```
ini.ReloadIni();
```

-----

### Changelog

###### v0.3.0 - 2018-09-18
**Added**

* IniAutoSaveEnabled property can be set during object initialization.

**Changed**

* SaveIni method now force saves ini file, without checking for pending changes.

###### v0.2.0 - 2018-09-15
**Added**

* Comment lines using ';' char support;
* Reload ini file method;
* Unit tests project.

###### v0.1.0 - 2018-09-14
* Initial project/library version.
