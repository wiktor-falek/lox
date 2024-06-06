# Comments

```js
// This is a comment

/* This is a multiline comment */

/**
 * This is a docstring
 */
```

# Variables

```js
// Inferred as Int from the value
let x = 69

// Forward declaration
let y: Int = 0

let x: Int // Error - Expected Int, found None

// Variables are immutable by default
x = 70 // Error

// To declare a mutable variable, use the 'mut' keyword
let mut x = 420

x = x + 1 // 421

// variables can be redeclared by using let
let x: List<Int> = [1, 2, 3]

// constants being the exception
const MY_CONST: Float = 3.14
const MY_CONST: String = "" // Error

let y: Int = "this will throw TypeError"

// const variables cannot be reassigned
const greeting = "Hello"
greeting = "Hi" // Error

// and can be assigned later
x = 10
x = None
```

# Printing

```js
print("Hello World"); // "Hello World\n"

// print can also take multiple arguments, they will be separated by a space
print("Hello", "World"); // "Hello World\n"

// print by default places newline character
// this can be omitted by changing end = "\n" to end = ""
print("Hello World", (end = "")); // "Hello World"

// to format a string with values use printf()
let a = 1;
let b = 2;
let c = a + b;

// each curly brace '{}' will be replaced with the value passed after
printf("{} + {} = {}", a, b, c);

// failing to match amount of curly braces and arguments will throw an error
printf("{}, {}", a); // Error

// variables can also be placed between the brackets
printf("{a} + {b} = {c}");

// you can also debug values
printf("{a=}"); // "a=1"
```

# Primitive data types

...
Int - Dynamic size bit signed integer

<!-- https://www.geeksforgeeks.org/ieee-standard-754-floating-point-numbers/ -->

Float - 64 bit signed double precision floating point number

Bool - True | False
String - immutable sequence of characters
Atom/Symbol ?

# Data structures

List<T> - sequence of items
Dict<K, V> - key value pairs
Tuple<..Items> - Immutable fixed size group of items
Set<T>
Map
OrderedMap

# Utility Types

Nil
Result<T, E>
Option<T> -> Some<T> | None

# Conditions

```js
let a = 1

if a == 1 {
  // code block 1
}
elif a == 0 {
  // code block 2
}
else {
  // code block 3
}
```

# Ranges

```js
0..3 // Generator(0, 1, 2)
1.2.7 // Generator(2, 4, 6)
-1.-1.-4 // Generator(-1, -2, -3)

List.range(1, 7, 2) // [2, 4, 6]
```

# Loops

## For loop

```js

for (let i in 0..3) {
  print(i, end = " ")
}
// 0 1 2

const list = [1, 2, 3]

for (let item in list) {
  print(item) // Int
}

for (let i, item in List.enum(list)) {
  printf("{i}={item}")
}
```

## While loop

```js
let mut i = 10
while (i) {
  i = i - 1
}
```

# Functions

```js
fn add(a: Int, b: Int) {
  return a + b
}
```

# Error handling

```js
fn divide_by(a: Int, b: Int) -> Result<Int, String> {
  if (b == 0) {
    return Error("Cannot divide by 0")
  } else {
    return Ok(a / b)
  }

  // or using match statement
  return case b {
    0 => Error("Cannot divide by 0")
    _ => Ok(a / b)
  }
}

// handling both paths
case divide_by(2, 0) {
  Ok(value) -> print(value)
  Error(msg) -> printf("Error: {msg}")
}

// alternatively get the value or throw the error
const value = divide_by(2, 0)?
```

# Features

## Pipe operator

```gleam
let foo = "foo"

foo
|> Str.reverse()
|> Str.repeat(3)
// "oofoofoof"
```

## Pattern Matching

```js
let list= [1, 2]
let result = case list {
  [] -> 0
  [Some(a)] -> 1
  [Some(a), Some(b)] -> 2
  [..xs] -> List.sum(xs)
}
```

## Custom Types

```js
type Shade {
  Bright
  Normal
  Dark
}

type Color {
  Red(Shade)
  Green(Shade)
  Blue(Shade)
}

fn rgb_color(color: Color) {
  return case color {
    Red(shade) -> {
      case shade {
        Bright -> "a"
        Normal -> "b"
        Dark -> "c"
        }
      }
    Green(_shade) -> "#00FF00"
    Blue(_shade) -> "#0000FF"
  }
}

rgb_color(Red)

struct Point {
  x: Int = 0,
  y: Int = 0,
}

```

## Use

```js
fn a() -> Result<Int, None>
fn b() -> Result<Int, None>

fn use_example(input: Int) {
  // returns Error on ResultError, otherwise the function continues and variable is ResultOk
  use a_result <- a(input)
  use b_result <- b(input)
  return a_result + b_result
}

```

## Destructuring

```rust
let tuple = (1, "a");
let number, string = tuple;

let numbers = [1, 2, 3, 4, 5]

let first, ..rest, last = numbers
```

## Assert

```
assert 1 == 1
let value <- assert Some() = get_option()
let assert Some() value = get_option()
<!-- let value = assert Some() get_option() -->
<!-- let value = assert Ok() get_result() -->
```

## Imports

TODO

## Throwing errors

```js
fn value_or_error() -> Result<Int, Nil>

let a = value_or_error()? // Throws if Error
```

```js
let condition = True

case condition {
  True -> print("Success")
  False -> panic
}
```

## Classes

```js
pub class Person {
  pub name: String
  pub static public_static_variable = "foo"
  static private_static_variable: Int = 2

  // constructor
  new(name: String) {
    this.name = name
  }

  private_method() {}

  pub public_method() {}

  static private_static_method() {}

  pub static public_static_method() {}

  pub static async public_static_async_method() -> Task<T> {}
}
```

# Standard Library (auto import)

## String

```gleam
Str.repeat(a: String) -> String
Str.reverse(a: String) -> String
Str.slice(a: String, start: Int, stop: Int) -> String
Str.at(a: String, index: Int) -> Result<String, Nil>
Str.split(a: String, separator: Int, max: Int) -> List<String>
Str.join(a: List<String>, delimiter: String) -> String
```

## List

```gleam
List.new<T>() -> List<T>
List.clone(a: List<T>) -> List<T>
List.append(a: List<T>, value: T) -> List<T>
List.prepend(a: List<T>, value: T) -> List<T>
List.insert(a: List<T>, value: T, index: Int) -> List<T>
List.slice(a: List<T>, start: Int, stop: Int, step: Int = 1) -> List<T>
List.reverse(a: List<T>) -> List<T>
List.at(a: List<T>, index: Int) -> Result<T, Nil>
List.map(a: List<T>, func: fn i -> R) -> List<R>
List.for_each(a: List<T>, func: fn i -> R) -> Nil
List.filter(a: List<T>) -> List<T>
List.fold(a: List<T>, initial: R, func: fn i -> R) -> R
```

## Dict

```gleam
Dict.new<K, V>() -> Dict<K, V>
Dict.set<K, V>(d: Dict, k: K, v: T) -> Dict<K, V>
Dict.get<K, V>(d: Dict, k: K) -> V
```

## Set

```gleam
Set.new<T>() -> Set<T>
Set.from_list<T>(a: List<T>) -> Set<T>
Set.intesect<T>(a: Set<T>, b: Set<T>) -> Set<T>
```

## Int

```gleam
Int.parse(a: String) -> Result<String, Nil>
Int.to_float(a: Int) -> Float
```

## Float

```gleam
Float.parse(a: String) -> Result<Float, Nil>
Float.truncate(a: Float) -> Int
Float.floor(a: Float) -> Float
Float.ceil(a: Float) -> Float
Float.round(a: Float) -> Float
```

## Math

```gleam
Math.PI -> Float
Math.E -> Float
Math.TAU -> Float
Math.sqrt(a: Int | Float) -> Float
Math.min<T = Int | Float>(a: T, b: T) -> T
Math.max<T = Int | Float>(a: T, b: T) -> T
Math.clamp<T = Int | Float>(a: T, b: T) -> T
Math.abs(a: Int | Float) -> Int
Math.to_degrees()
Math.to_radians()
Math.sin()
Math.cos()
Math.tan()
Math.atan()
Math.atan2()
```

## Random

```gleam
Random.rand() -> Float
Random.randint(start: Int, stop: Int, step: Int = 1) -> Int
Random.choice<T>(a: List<T>) -> Result<T, Nil>
Random.choices(a: List<T>, count: Int, weights: List<Int>) -> Result<List<T>, Nil>
```
