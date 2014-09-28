/// <summary>
/// Provides basic operations on strings.
/// </summary>
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module FsRandom.String

/// <summary>
/// Returns a random string which is composed of characters in the given string.
/// </summary>
[<CompiledName("RandomByString")>]
val randomByString : (string -> int -> GeneratorFunction<string>)
/// <summary>
/// Returns a random string which is composed of non-control non-space ASCII characters.
/// </summary>
[<CompiledName("RandomAscii")>]
val randomAscii : (int -> GeneratorFunction<string>)
/// <summary>
/// Returns a random string which is composed of digits.
/// </summary>
[<CompiledName("RandomNumeric")>]
val randomNumeric : (int -> GeneratorFunction<string>)
/// <summary>
/// Returns a random string which is composed of alphabets.
/// </summary>
[<CompiledName("RandomAlphabet")>]
val randomAlphabet : (int -> GeneratorFunction<string>)
/// <summary>
/// Returns a random string which is composed of alphabets or digits.
/// </summary>
[<CompiledName("RandomAlphanumeric")>]
val randomAlphanumeric : (int -> GeneratorFunction<string>)
/// <summary>
/// Concatenates random strings into one random string.
/// </summary>
[<CompiledName("RandomConcat")>]
val randomConcat : (string -> GeneratorFunction<string> list -> GeneratorFunction<string>)
