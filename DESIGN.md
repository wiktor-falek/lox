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
let y: UInt8 = 0

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

// uninitialized variables throw if type is not declared
let x: Int // Option(Int)

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

// if you need to format the string with multiple values use printf()
const a = 1;
const b = 2;
const c = a + b;

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
Int - 64 bit signed integer
Int32 - 32 bit signed integer
UInt32 - 32 bit unsigned integer
...
Int8 - 8 bit signed integer

<!-- https://www.geeksforgeeks.org/ieee-standard-754-floating-point-numbers/ -->

Float - 64 bit signed double precision floating point number

Bool - True | False
String - immutable sequence of characters
Atom/Symbol ?

# Data structures

List<T> - sequence of items
Dict<K, V> - key value pairs
Tuple<...> - Immutable fixed size group of items
Set<T>
Map
OrderedMap

# Utility Types

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
let i: number = 10
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
fn divide_by(a: number, b: number) -> Result<Int, String> {
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

// unwrap to throw Error or return the value
const value = divide_by(2, 0).unwrap() // throws Error

// or handle the error without throwing
const value, err = divide_by(2, 0)
if (err == null) {
  print("ERROR", err.name, err.message, err.stacktrace)
}
print(value)
```

# Features

## Pipe operator

```js
let foo = "foo";

foo |> Str.reverse() |> Str.repeat(3);
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
    // Green(shade) -> "#00FF00"
    // Blue(shade) -> "#0000FF"
  }
}

rgb_color(Red)

type Point {
  x: Int
  y: Int
}

```

## Use

```js
fn a() -> Result<Int, None>
fn b() -> Result<Int, None>

fn use_example(input: Int) {
  // if either is an Error it will early return the Error, otherwise it returns the ResultOk type
  use a_result = a(input)
  use b_result = b(input)
  return a_result + b_result
}

```
