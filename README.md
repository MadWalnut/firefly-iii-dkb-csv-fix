# Firefly III DKB CSV Fixer

This application checks if a given file is a valid [DKB](https://www.dkb.de/) transaction report (checking account or credit card). It then converts the file from ANSI to UTF-8, after removing several unwanted lines in order to prepare it for the [Firefly III CSV importer](https://github.com/firefly-iii/csv-importer).

These are the lines that are removed:
- First few as they only contain information about your account and Firefly III CSV importer is only capable of ignoring a single header line.
- Any line containing "KREDITKARTENABRECHNUNG VISA" (including quotation marks). This will remove your credit card bill from the checking account transaction list. It will *not* remove it from your credit card transaction list though. Whithout this, your credit card bill would show up twice in Firefly III after importing both your checking account CSV and your credit card CSV.
- Any line containing "Abschluss" (including quotation marks). This will remove the fake transaction in the CSV file which is created on the first of every month and only shows the current account balance. As this is not a real transaction, the Firefly III CSV importer will not be able to parse this line.

*If you would like the removal of the lines to be an optional feature (e.g. to be enabled via a parameter like `--removeCreditCardBill`), let me know by opening an issue.*

## Installation

There are three options on how to get Firefly III DKB CSV Fixer.

### Download Framework-independant binary

Head over to the [release page](https://github.com/MadWalnut/firefly-iii-dkb-csv-fix/releases) and grab yourself the self-contained version for your system. Done.

*Because Firefly III DKB CSV Fixer relies on .NET, these binaries are rather large as they contain the .NET Framework alongside the actual binary. If you want a single, small file you will have to use the Framework-dependant binaries.*

### Download Framework-dependant binary

DKB CSV Fixer requires the .NET runtime (or SDK). Install .NET 5.0 or higher from [here](https://dotnet.microsoft.com/download/dotnet) (available for Linux, macOS and Windows). If you are on Windows, you may have the runtime installed already. You can check with `dotnet --version`.

Then, [download the compiled release](https://github.com/MadWalnut/firefly-iii-dkb-csv-fix/releases) for your platform. You **don't** need the self-contained version.

### Build from source

If you prefer to build yourself, see [here](#building).

## Usage

1. Download both CSV files from your DKB account (checking account and credit card).
2. Call Firefly III DKB CSV Fixer by providing the path of the CSV file as the first parameter. If you are using Windows, [here is a quick tip for a shortcut](#windows-explorer-integration).
```
FireflyFixTransactionReportDKB "C:\Path\To\The\CSV file.csv"
```
3. Import as usual to the Firefly III CSV importer, using the [import configuration](https://github.com/firefly-iii/import-configurations/tree/main/de/dkb). Remember that you may need to change `import-account` in the configuration files to match your Firefly III account IDs.

## Windows Explorer Integration
You don't have to call the program manually via command line every time. You can integrate it into Windows Explorer, so when right-clicking on any CSV file, you will see the option *Fix DKB export for Firefly*. To achieve this, just add a few registry entries. The fastest way is to create an empty text file and copy these contents into the file:
```
Windows Registry Editor Version 5.00

[HKEY_CLASSES_ROOT\SystemFileAssociations\.csv]

[HKEY_CLASSES_ROOT\SystemFileAssociations\.csv\shell]

[HKEY_CLASSES_ROOT\SystemFileAssociations\.csv\shell\fixDKB]
@="Fix DKB export for Firefly"

[HKEY_CLASSES_ROOT\SystemFileAssociations\.csv\shell\fixDKB\command]
@="\"C:\\Users\\USERNAME\\PATH\\TO\\THE\\APPLICATION\\FireflyFixTransactionReportDKB.exe\" \"%1\""
```
**Remember to change the path to the application!**

Then save as a `.reg` file and execute it. You may need to restart your computer. 

![Contextmenu](https://user-images.githubusercontent.com/33835479/117704414-1c86b600-b1cb-11eb-9f49-730679ab7d6e.png)

To remove this shortcut, just delete the keys from the registry.

## Building

Compiling yourself is easy. Just make sure you have at least .NET 5.0 **SDK** installed (download from [here](https://dotnet.microsoft.com/download/dotnet)).

Download the source and `cd` into the project directory. Adapt the command to your needs:

`dotnet publish --configuration Release --runtime win-x86 --output C:\Users\USERNAME\Desktop\dotnet --self-contained false`

- Set the build target accordingly. You can find the options [here](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog#using-rids).
- Set the output path for your binaries.
- Only set `self-contained true` if you want to want to run the application on a system that has **no** .NET runtime installed.

For more information on the `dotnet publish` command, see [here](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-publish).

### Issues

If you get a `.pdb` file after publishing, you can add `-p:DebugType=None` as a parameter to your `dotnet publish` command. You don't need this file.

If you end up with multiple output files instead of one (e.g. .dll files), add this parameter to your `dotnet publish` command: `-p:PublishSingleFile=true`

*Background information: Both these issues appear when you are not using the correct .csproj file from the repo. Copying that file next to your Program.cs should fix both.*

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change. 

## Credits

Icon made by [Freepik](https://www.flaticon.com/authors/freepik) from [Flaticon](https://www.flaticon.com).

[Firefly III](https://www.firefly-iii.org/) by James Cole.

## License
[MIT License](https://choosealicense.com/licenses/mit/)

```
MIT License

Copyright (c) 2021 MadWalnut

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
```
