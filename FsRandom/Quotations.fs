module internal FsRandom.Quotations.ComputationExpressionPatterns

open Microsoft.FSharp.Quotations.Patterns

let (|MethodNamedAs|_|) name = function
   | Call (Some (builder), mi, args) when mi.Name = name -> Some (builder, mi, args)
   | _ -> None
let (|Bind|_|) = function
   | MethodNamedAs "Bind" (builder, methodInfo, [m; f]) -> Some (builder, methodInfo, m, f)
   | _ -> None
let (|Delay|_|) = function
   | MethodNamedAs "Delay" (builder, methodInfo, [f]) -> Some (builder, methodInfo, f)
   | _ -> None
let (|Return|_|) = function
   | MethodNamedAs "Return" (builder, methodInfo, [x]) -> Some (builder, methodInfo, x)
   | _ -> None
let (|ReturnFrom|_|) = function
   | MethodNamedAs "ReturnFrom" (builder, methodInfo, [m]) -> Some (builder, methodInfo, m)
   | _ -> None
let (|Run|_|) = function
   | MethodNamedAs "Run" (builder, methodInfo, [f]) -> Some (builder, methodInfo, f)
   | _ -> None
let (|Combine|_|) = function
   | MethodNamedAs "Combine" (builder, methodInfo, [a; b]) -> Some (builder, methodInfo, a, b)
   | _ -> None
let (|For|_|) = function
   | MethodNamedAs "For" (builder, methodInfo, [source; f]) -> Some (builder, methodInfo, source, f)
   | _ -> None
let (|TryFinally|_|) = function
   | MethodNamedAs "TryFinally" (builder, methodInfo, [m; finalizer]) -> Some (builder, methodInfo, m, finalizer)
   | _ -> None
let (|TryWith|_|) = function
   | MethodNamedAs "TryWith" (builder, methodInfo, [m; handler]) -> Some (builder, methodInfo, m, handler)
   | _ -> None
let (|Using|_|) = function
   | MethodNamedAs "Using" (builder, methodInfo, [resource; f]) -> Some (builder, methodInfo, resource, f)
   | _ -> None
let (|While|_|) = function
   | MethodNamedAs "While" (builder, methodInfo, [guard; m]) -> Some (builder, methodInfo, guard, m)
   | _ -> None
let (|Yield|_|) = function
   | MethodNamedAs "Yield" (builder, methodInfo, [x]) -> Some (builder, methodInfo, x)
   | _ -> None
let (|YieldFrom|_|) = function
   | MethodNamedAs "YieldFrom" (builder, methodInfo, [m]) -> Some (builder, methodInfo, m)
   | _ -> None
let (|Zero|_|) = function
   | MethodNamedAs "Zero" (builder, methodInfo, []) -> Some (builder, methodInfo)
   | _ -> None
