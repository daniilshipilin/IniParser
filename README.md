# IniParserLibrary

### Description
Simple INI file parser library.

### Usage

###### Instantiate IniParser:
`var ini = new IniParser(iniFilePath);`

###### Get key value:
`string keyValue = ini.GetValue("SECTION_NAME", "KeyName");` (*returns null, if key doesn't exist*)

###### Set key value:
`ini.SetValue("SECTION_NAME", "KeyName", "KeyValue");`

###### Delete key:
`ini.DeleteKey("SECTION_NAME", "KeyName");` (*returns true, if existing key was deleted*)

###### Save/commit changes:
`ini.SaveIni();` (*actual ini file save is initiated only, if changes have been made - override this with optional forceSave parameter*)

-----

### Options

###### Instantiate IniParser and enable ini file autosave functionality (no need to call SaveIni method after SetValue or DeleteKey):
`var ini = new IniParser(iniFilePath, true);`

###### Enable/disable ini file autosave functionality after class has been instantiated:
`ini.EnableIniAutoSave();`

`ini.DisableIniAutoSave();`

-----

### Changelog

###### v0.1.0 - 2018-09-14
* Initial project/library version.
