#I "../packages"

#r "FAKE/tools/NuGet.Core"
#r "FAKE/tools/Newtonsoft.Json"
#r "FAKE/tools/Microsoft.Web.XmlTransform"
#r "FAKE/tools/FakeLib"

open Fake
open Fake.ConfigurationHelper
open Fake.XMLHelper
open System.Xml
let config = readConfig "ProtoModels.csproj"
let xmlString = @"
<Project DefaultTargets=""Build"" ToolsVersion=""4.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <PropertyGroup>
    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
    <ProjectGuid>{672A1EB8-4BC6-410A-BE7E-6251A0DB7AD6}</ProjectGuid>
    <OutputType>Library</OutputType>
    <RootNamespace>ProtoModels</RootNamespace>
    <AssemblyName>ProtoModels</AssemblyName>
    <TargetFrameworkVersion>v4.5</TargetFrameworkVersion>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include=""System"" />
  </ItemGroup>
  <ItemGroup>
    <Compile Include=""Properties\AssemblyInfo.cs"" />
    <Compile Include=""models\Test.cs"" />
    <Compile Include=""models\Request.cs"" />
  </ItemGroup>
  <Import Project=""$(MSBuildBinPath)\Microsoft.CSharp.targets"" />
  <ItemGroup>
    <Folder Include=""schemas\"" />
    <Folder Include=""models\"" />
  </ItemGroup>
</Project>"
let xml = new XmlDocument();
xml.LoadXml(xmlString); // suppose that myXmlString contains "<Names>...</Names>"

let xnList = 
    xml.SelectNodes("/Project/ItemGroup");
printfn "%A" xnList.Count
printfn "%A" <| (config.DocumentElement.SelectNodes("//ItemGroup")).Count

