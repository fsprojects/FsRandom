﻿[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module FsRandom.String

open System.Globalization

let ascii = [|'!' .. '~'|]
let digit = [|'0' .. '9'|]
let upper = [|'A' .. 'Z'|]
let lower = [|'a' .. 'z'|]
let alphabet = Array.concat [upper; lower]
let alphanumeric = Array.concat [digit; alphabet]

let inline makeString (array:char []) = System.String (array)
let inline randomStringByCharArray array length =
   if length = 0 then
      Random.singleton ""
   else
      Random.map makeString (Array.sampleWithReplacement length array)
let getCharacters s =
   let e = StringInfo.GetTextElementEnumerator (s)
   seq { while e.MoveNext () do yield string e.Current } |> Seq.toArray
let inline randomStringByStringArray array length =
   if length = 0 then
      Random.singleton ""
   else
      Random.map (String.concat "") (Array.sampleWithReplacement length array)

let randomByString (s:string) length = randomStringByStringArray (getCharacters s) length
let randomAscii length = randomStringByCharArray ascii length
let randomNumeric length = randomStringByCharArray digit length
let randomAlphabet length = randomStringByCharArray alphabet length
let randomAlphanumeric length = randomStringByCharArray alphanumeric length
let randomConcat separator randomStringGenerators = Random.mergeWith (String.concat separator) randomStringGenerators
