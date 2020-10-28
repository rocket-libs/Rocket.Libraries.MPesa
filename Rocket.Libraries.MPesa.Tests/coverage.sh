#!/bin/bash
dotnet clean
rm -r ./TestResults
mkdir ./TestResults
dotnet test /p:CollectCoverage=true /p:CoverletOutput=TestResults/ /p:CoverletOutputFormat=cobertura
cd ./TestResults
reportGenerated = dotnet reportgenerator -reports:coverage.cobertura.xml -targetdir:reports
if [$reportGenerated != `0`]
then
    reportgenerator -reports:coverage.cobertura.xml -targetdir:reports
fi
cd ..