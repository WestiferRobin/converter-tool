# Convertertool

## Current Notes
Java and C# is now in work right now.
XML and JSON conversion is complete.

## Individual files
ConverterTool.exe sourceFile outputFile
- ConverterTool.exe "C:\Temp\asdf.xml" "C:\Temp\asdf.json"
- ConverterTool.exe "C:\Temp\asdf.java" "C:\Temp\asdf.cs"

## Multiple files
- ConverterTool.exe sourceType sourceDirectory targetType targetDirectory 
    - ConverterTool.exe -java "C:\JavaProject" -cs "C:\CsharpProj"
    - ConverterTool.exe -xml "C:\OldXmls" -json "C:\NewJsons"
- If log files are needed then apply the -log flag and path and it will be applied.
    - ConverterTool.exe -java "C:\JavaProject" -cs "C:\CsharpProj" -log "C:\Temp"

## Future updates
1. Embedded Update for Converter Tool
- For C++, C, and Assembly (TBD for what set archetecture).
2. Fullstack Update for Converter Tool
- For Python, Javascript, Golang, and Typescript.
