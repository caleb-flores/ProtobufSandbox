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

let nsmgr = XmlNamespaceManager(config.NameTable)

printfn "%A" <| (config.DocumentElement.SelectNodes("//project")).Count

