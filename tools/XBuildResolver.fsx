#load "XmlEditor.fs"
#r "System.Xml.Linq.dll"

open XmlEditor
open System.Xml.Linq

let editProject rawProjectXml =
   let project = XDocument.Parse rawProjectXml
   query {
      for import in project.Descendants (XName.Get ("Import", "http://schemas.microsoft.com/developer/msbuild/2003")) do
      let project = import.Attribute (XName.Get ("Project"))
      where (project.Value.EndsWith ("Microsoft.FSharp.Targets"))
      select (import.Attribute (XName.Get ("Condition")))
   }
   |> Seq.iter (fun attribute -> attribute.Remove ())
   project

main editProject
