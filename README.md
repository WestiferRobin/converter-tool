# Convertertool

## Current Notes
Java and C# is now in work right now.
XML and JSON conversion is complete.

## Individual files
ConverterTool.exe sourceFile outputFile
- ConverterTool.exe "C:\Temp\asdf.xml" "C:\Temp\asdf.json"
- ConverterTool.exe "C:\Temp\asdf.java" "C:\Temp\asdf.cs"

## Multiple files
ConverterTool.exe sourceType sourceDirectory targetType targetDirectory 
- ConverterTool.exe -java "C:\JavaProject\." -csharp "C:\CsharpProj\."
- ConverterTool.exe -xml "C:\OldXmls\." -json "C:\NewJsons\."

## Other flags
- default is nuking the files with default library classes so that conversion is perfect
- literal flag is an override to be literal instead of assumed like the default way.
    - ConverterTool.exe -java "C:\JavaProject\." -csharp "C:\CsharpProj\." -lit
- Log flag
    - ConverterTool.exe -java "C:\JavaProject\." -csharp "C:\CsharpProj\." -lit -log

## Future updates
Embedded Update for Converter Tool
- For C++, C, and Assembly (TBD for what set archetecture).
Fullstack Update for Converter Tool
- For Python, Javascript, Golang, and Typescript.