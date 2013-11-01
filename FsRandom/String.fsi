[<RequireQualifiedAccess>]
module FsRandom.String

/// <summary>
/// Returns a random string which is composed of characters in the given string.
/// </summary>
val randomByString : (string -> int -> GeneratorFunction<string>)
/// <summary>
/// Returns a random string which is composed of non-control non-space ASCII characters.
/// </summary>
val randomAscii : (int -> GeneratorFunction<string>)
/// <summary>
/// Returns a random string which is composed of digits.
/// </summary>
val randomNumeric : (int -> GeneratorFunction<string>)
/// <summary>
/// Returns a random string which is composed of alphabets.
/// </summary>
val randomAlphabet : (int -> GeneratorFunction<string>)
/// <summary>
/// Returns a random string which is composed of alphabets or digits.
/// </summary>
val randomAlphanumeric : (int -> GeneratorFunction<string>)
/// <summary>
/// Concatenates random strings into one random string.
/// </summary>
val randomConcat : (string -> GeneratorFunction<string> list -> GeneratorFunction<string>)
