module FsRandom.Optimization

open Microsoft.FSharp.Linq
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.DerivedPatterns
open Microsoft.FSharp.Quotations.Patterns
open FsRandom.Quotations.ComputationExpressionPatterns

(*
The original expression is given:

   random {
      let! x = generator1
      let! y = generator2
      return (x, y)
   }

This is almost equivalent to:

   random.Delay (fun () ->
      random.Bind (generator1, fun x ->
         random.Bind (generator2, fun y ->
            random.Return ((x, y))
         )
      )
   )

After the optimization process, the expected expression is:

   GeneratorFunction (fun stateObject ->
      let x, stateObject = Random.next generator1 stateObject
      let y, stateObject = Random.next generator2 stateObject
      (x, y), stateObject
   )
*)

let unitVar = Var ("unitVar", typeof<unit>)
let unitExpr = <@ () @>

let rec loop = function
   | Let (var, expr, body) -> Expr.Let (var, expr, loop body)
   | Lambda (var, body) -> Expr.Lambda (var, loop body)
   | Bind (builder, mi, m, f) -> Expr.Call (builder, mi, [loop m; loop f])
   | Combine (builder, mi, a, b) -> Expr.Call (builder, mi, [loop a; loop b])
   | IfThenElse (guard, e1, e2) -> Expr.IfThenElse (guard, loop e1, loop e2)
   | For (builder, mi, guard, m) -> Expr.Call (builder, mi, [guard; loop m])
   | While (builder, mi, source, f) -> Expr.Call (builder, mi, [source; loop f])
   | Sequential (first, second) -> Expr.Sequential (loop first, loop second)
   | TryWith (builder, mi, m, handler) -> Expr.Call (builder, mi, [loop m; loop handler])
   // The finalizer function may not contain computation.
   | TryFinally (builder, mi, m, finalizer) -> Expr.Call (builder, mi, [loop m; finalizer])
   | Using (builder, mi, resource, f) -> Expr.Call (builder, mi, [resource; loop f])
   | ReturnFrom (builder, MethodWithReflectedDefinition m, x) ->
      let optimized = Expr.Lambda (unitVar, loop m)
      Expr.Application (optimized, unitExpr)
//   | Return (builder, MethodWithReflectedDefinition m, x) -> Expr.Call (builder, loop m, [x])
   | ReturnFrom (builder, mi, m) -> Expr.Call (builder, mi, [loop m])
   // There's nothing to optimize.
//   | Return (builder, mi, x) -> Expr.Call (builder, mi, [x])
//   | Zero (builder, mi) -> Expr.Call (builder, mi, [])
   | Delay (builder, mi, f) -> Expr.Call (builder, mi, [loop f])
   // TODO: Increasing in speed. Removing the getting property makes itself slower.
   | PropertyGet (None, PropertyGetterWithReflectedDefinition p, []) -> loop p
   | Application (f, args) -> Expr.Application (loop f, args)
   | Lambda (parameter, body) -> Expr.Lambda (parameter, loop body)
   | expr -> expr

let optimize (g:Expr<GeneratorFunction<'a>>) =
   let optimizedGenerator = loop g
   QuotationEvaluator.EvaluateUntyped optimizedGenerator :?> GeneratorFunction<'a>
