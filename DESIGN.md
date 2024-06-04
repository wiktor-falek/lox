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
print("Hello World", end = ""); // "Hello World"

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

# Loops

## For loop

```js

for (let i in 0..3) {
  print(i, end = " ")
}
// 0 1 2

const list = [1, 2, 3]

// iterating over items
for (let item in list) {
  print(item) // Int
}

// get index and the item
for (let i, item in List.enum(list)) {
  printf("{i}={item}")
}
```

## While loop

```js
let mut i = 10
while (i) {
  i--
}

// TODO
break
continue
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

// alternatively read the value or throw the error
const value = divide_by(2, 0)?
```

# Features

## Pipe operator

```js
let foo = "foo";

foo
|> Str.reverse() 
|> Str.repeat(3);
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
