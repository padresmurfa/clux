# Clux

The Clux library is a command-line parser for C#, created with the core philosophy of providing maximum user capabilities through minimum developer effort.

Clux derives its deterministic command-line syntax from class/struct definitions in an opinionated fashion, using reflection and a minimal amount of metadata.

#### Example

```C#
using Clux;

public class Hello
{
    [Positional]
    [Usage("The greeting")]
    public string Hello;

    [Usage("The craziness factor")]
    public uint16? Crazy;

    [Usage("The world that is crazy")]
    public string World;
}

// usage: program [hello] [-c <crazy>] [-w <world>]
//         hello: The greeting
//   -c, --crazy: The craziness factor
//   -w, --world: The world that is crazy

public static void Main(string[] args)
{
    args = args ?? new [] {
        "--crazy", "3",
        "-w", "Earth"
    };

    var parsed = Parser<Hello>.Parse(args);

    Console.WriteLine($"craziness: {parser.Crazy}");
    Console.WriteLine($"    world: {parser.World}");
}

// Output:
// ------------
// craziness: 3
//     world: Earth

```

## 1. Properties and Fields

Every public property or field of the template class/struct that is not decorated with an **[Ignore]** attribute will be interpreted by Clux as a command-line argument definition.

**_No distinction is made between properties and fields by Clux, and both are referred to as properties in the Clux documentation, for simplicity's sake._**

## 2. Positional or Named arguments?

Command line arguments can interpreted by Clux as either  _positional_ or _named_, but never as both.

A positional argument must be specified in the correct location on the command line, relative to other positional arguments.

A named argument must be specified by name or abbreviation.  A named argument that is declared between two positional arguments must appear between them on the command line.

### 2.1. Positional arguments

Any property that is decorated with the **[Positional]** attribute will be interpreted as a positional argument by Clux.

#### Example
```C#
public class Args
{
    [Positional]
    [Usage("The greeting")]
    public string Hello;

    [Positional]
    [Usage("The world")]
    public string World;
}

// usage: program [hello] [world]
//         hello: The greeting
//         world: The world

```
Positional arguments are interpreted from left to right on the command-line, and from top to bottom in the source definition.  Thus in the example above, hello must occur before world on the command line, since Hello occurs before World in the class definition.

### 2.2. Named arguments

Any property that is **NOT** decorated with the **[Positional]** attribute will be interpreted as a named argument by Clux.

A named argument can be specified either by _long option_ (e.g. _--foo_) or by _short option_ (e.g. _-f_).

#### Example
```C#
public class Args
{
    [Usage("The greeting")]
    public string Hello;

    [Usage("The world")]
    public string World;
}

// usage: program [-h <hello>] [-w <world>]
//   -h, --hello: The greeting
//   -w, --world: The world

```
Named arguments can occur in any order on the command-line, so long as they appear before any subsequently declared **[Positional]** property.

#### Example
```C#
public class Args
{
    [Usage("The greeting")]
    [Positional]
    public string Hel;

    [Usage("The greeting, part 2")]
    public string Lo;

    [Usage("The world")]
    [Positional]
    public string World;
}

//   valid: --lo hello hello world
//   valid: hello --lo hello world
// invalid: hello world --lo hello

```

Named options may optionally be specified on the command-line using the equals operator.

#### Example
```C#
public class Args
{
    public string Hello;
}

//   valid: -h world
//   valid: -h=world
//   valid: --hello world
//   valid: --hello=world

```

#### 2.2.1. Long options

Clux will map properties to command-line arguments using [_kebab-case_](https://en.wikipedia.org/wiki/Letter_case#Special_case_styles).  Thus a property named FooBlatID would use the long option _--foo-blat-i-d_ while FooBlatId would use the long option _--foo-blat-id_.

#### 2.2.2. Short options

Clux will by default allocate the first letter in lower-case of each long-option as a short option. 

The property FooBlat can therefor by default be passed via the command-line using either the long option _--foo-blat_ or the short option _-f_.

#### Example
```C#
public class Args
{
    public string Hello;
}

//   valid: --hello world
//   valid: -h world
// INVALID: -H world
```

#### 2.2.3. Merged short options

Boolean short options (aka flags) can be merged/squashed.  Thus the following command lines are considered identical:
* fooblat _-bad_
* fooblat _-b -a -d_

## 3. Optional or Required?

Properties that use a reference or nullable type, such as _arrays_, _strings_ or _ints?_ will be considered optional by Clux by default.  Other properties will be considered to be required by default.

An optional property can be declared to be required through using the **[Required]** attribute.

## 4. Basic property types

The following basic property types are supported as arguments by Clux:

| Type        | C#           | Nullable?  |
| ------------- |:-------------:| :-----:|
| string | String | N/A |
| bool | Boolean | N/A |
|  8-bit signed integer | SByte | yes |
| 16-bit signed integer | Int16 | yes |
| 32-bit signed integer | Int32 | yes |
| 64-bit signed integer | Int64 | yes |
|  8-bit unsigned integer | Byte | yes |
| 16-bit unsigned integer | UInt16 | yes |
| 32-bit unsigned integer | UInt32 | yes |
| 64-bit unsigned integer | UInt64 | yes |
| single-precision floating point | Float | yes |
| double-precision floating point | Double | yes |
| fixed point decimal | Decimal | yes |

## 5. Complex property types

The following complex property types are supported as arguments by Clux:

| Type        | C#           | Nullable?  |
| ------------- |:-------------:| :-----:|
| date/time | DateTime | yes |
| enumeration | ... | yes |
| array | ... | N/A |

### 5.1. Date/Time types

Date/Time fields must be specified using the format **yyyyMMddHHmmssffffff**, or any logical shorter version of the format.  Whitespace is allowed.

Command line arguments are always assumed to be provided by in **local time**, since that is most natural for humans in general.  The internal representation will however always be in **universal time**, since that is more natural for software in general.

#### Example
```C#
public class Args
{
    public DateTime Hello;
}

// usage: program -h "20181231 120000 000"
// usage: program -h "20181231 120000000"
// usage: program -h "20181231 120000"
// usage: program -h "20181231 1200"
// usage: program -h 201812311200
```

### 5.2. Enumerations

Enumeration values can be specified by name.

#### Example
```C#
public enum Blat
{
    arg, smarg, garg
}

public class Foo
{
    public Blat Splat;
}

// usage: program --splat=garg
// usage: program --splat garg
// usage: program -s garg
// Assert.Equals(Blat.garg, parser.Splat)
```

### 5.3. Arrays

Arrays are filled with multiple arguments from the command line.  In the case of named options, the option may alternatively be specified multiple times to append to the array.

#### Example
```C#
public class Foo
{
    public string[] SArg;
}

// usage: program -s a b c d
// usage: program -s a -s b -s c -s d
// Assert.CollectionEqual(new string[]{ "a", "b", "c", "d" }, parser.SArg)

public class Blat
{
    public int[] NArg;
}

// usage: program -n 1 2 3 4
// usage: program -n 1 -n 2 -n 3 -n 4
// Assert.CollectionEqual(new int[]{ 1, 2, 3, 4 }, parser.NArg)

```

Arrays, as with other nullables, are optional by default.  

# Advanced Topics

## A1. Using base 2, 8 and 16 in command line arguments

Integral property types can be provided using decimal, binary, octal, or hex.  When converting signed numbers, two's complement is assumed.

#### Example
```C#
public class Args
{
    public UInt64 Hello;
}

// usage: program -h 15
// usage: program -h 0xF
// usage: program -h 0o17
// usage: program -h 0b1111
//
// Assert.Equal(15, parser.Hello)

public class GArgs
{
    public Int16 Hello;
}

// usage: program -h -20
// usage: program -h 0xFFEC
// usage: program -h 0o177754
// usage: program -h 0b1111111111101100
//
// Assert.Equal(-20, parser.Hello)

```

## A2. Array arguments that are not the last argument

Array arguments are pretty straight-forward when occurring as the last positional argument on the command line.  When an array argument is specified by name, or is succeeded by one or more positional arguments, things get a bit more complicated.

In such cases, its worth knowing that an array argument is terminated by encountering a new named or positional argument, or an argument that cannot be parsed as the correct input type.

Clux detects a positional argument by the - or -- prefix.  Subsequent positional arguments are detected by the number of required positionals that are unfilled by the parser at the time.

#### Example
```C#
public class Foo
{
    public string[] SArg;
    public string Next;
}

// usage: program -s a b c
// usage: program -s a b c -n d
// usage: program -s a -s b -s c --next d
// Assert.CollectionEqual(new string[]{ "a", "b", "c" }, parser.SArg)

public class Bar
{
    public string[] SArg;

    [Positional]
    public string Next;
}

// usage: program -s a b c d
// usage: program -s a -s b -s c d
// Assert.CollectionEqual(new string[]{ "a", "b", "c" }, parser.SArg)

public class Blat
{
    [Positional]
    public int[] NArg;

    [Positional]
    public string[] SArg;
}

// usage: program 1 2 3 a
// Assert.CollectionEqual(new int[]{ 1, 2, 3 }, parser.NArg)
// Assert.CollectionEqual(new string[]{ "a" }, parser.SArg)

```
## A3. Enumerations by value

Enumeration values can also be specified by underlying value, where appropriate

#### Example
```C#
public enum Bork : uint
{
    arg = 1,
    smarg = 3,
    garg = 8
}

public class Gork
{
    public Bork Mork;
}

// usage: program --mork=smarg
// usage: program --mork=3
// Assert.Equals(Bork.smarg, parser.Mork)

```

## A4. Special cases and ambiguity in Short Options

In the event that multiple named options have the same first letter, then neither option will have the abbreviation by default. 

This can be overridden by annotating the property(ies) with the **[Abbreviation(char)]** attribute.

The **[Abbreviation(char)]** attribute can also be used to assign upper-case short names to options.

#### Example
```C#
public class Args
{
    public string Hello;
}
//   valid: --hello world
//   valid: -h world
// INVALID: -H world

public class HArgs
{
    [Abbreviation('H')]
    public string Hello;
}
//   valid: --hello world
//   valid: -H world
// INVALID: -h world

public class QArgs
{
    [Abbreviation('q')]
    public string Hello;
}
//   valid: --hello world
//   valid: -q world
// INVALID: -h world
// INVALID: -H world

public class AmbiguousArgs
{
    public string Hello;

    public string Hi;
}
//   valid: --hello world
//   valid: --hi there
//   valid: --hello world --hi there
// INVALID: -h this-is-ambiguous
```

## A5. Limitations in detecting declaration order in C#

_**TL;DR** Please consider providing a Usage clause, otherwise your undecorated named options may be interpreted in a slightly uncool order._

Clux can determine the declaration order of arguments correctly for **structs** out of the box, as they are laid out in memory in declaration order (by default).

Likewise, Clux will correctly determine the order for any argument that is decorated with a Clux attribute such as **[Positional], [Required], [Usage], [Abbreviation]** or **[Constant]**.

If your classes contain non-decorated members, then you should keep in mind that the CLR will layout instances of the class in memory in whatever it considers to be the optimal order.  In other words, in an undefined order, as far as C# code is concerned.  Clux will order arguments deterministically in this case, but not neccessarily in declaration order.

Clux will sort all unattributed arguments before all attributed orders, so if you have a **[Positional]** argument, no unattribute argument may appear following it on the command line.  This may not be what you want, but is a decent default.

To change this behaviour without decorating individual members, you could require the CLR to layout instances of a class in memory in declaration order, by decorating the class itself with **[StructLayout(LayoutKind.Sequential)]**...

... **OR** you could e.g. add a **[Usage]** declaration to each of your arguments, which would have the additional benefit of providing a good help message. This is thus the recommended general approach.

#### Example
```C#
public class Args
{
    [Positional]
    public string Hello;

    public string World;
}
// usage: program [-w <world>] <hello>

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public class SArgs
{
    [Positional]
    public string Hello;

    public string World;
}
// usage: program <hello> [-w <world>] 

public class SArgs
{
    [Positional]
    [Usage("The greeting")]
    public string Hello;

    [Usage("The world")]
    public string World;
}
// usage: program <hello> [-w <world>] 

```

## A6. Impact of partial classes on declaration order and [Positional]

The declaration order is defined by line number, so you may e.g. create sections using partial classes in the same file, while partial classes crossing file boundaries will for all reasonable intents and purposes have an undefined ordering and should thus be avoided.

**[Positional]** declarations take note of the line number that they appear on, as do other Clux attributes.  Again, you should be able to create nice sections using this technique with partial classes within a file, while for all intents and purposes your positional arguments will have an undefined position when crossing file boundaries.  Thus again, this should be avoided.

## A7. Constants

A command line argument can be decorated as Constant, in which case the command line parser will not succeed unless the argument in question has the specified value.

If the argument is optional, then the constant constraint will not be considered violated unless the argument is specified.

Constants are useful in special cases, such as with Parser Unions (see A8), below.

#### Example
```C#
public class Args
{
    [Positional]
    public string Hello;

    [Constant("world")]
    public string World;
}
// usage: program <hello> [-w world]
// program hello -w world

```

## A8. Parser Unions

A Clux parser-union allows you to simultaneously attempt to parse multiple different argument sets, placing the burden of routing on Clux for the convenience of the developer.

Clux will scan through the provided argument types, and invoke the appropriate handler.

#### Example

```C#

    public class MeRun
    {
        [Constant("run")]
        [Positional]
        [Required]
        [Usage("The docker command")]
        public string Command;
        
        [Usage("The arguments to the 'run' docker command")]
        [Positional]
        public string[] Args;
    }
    
    public class MeStop
    {
        [Constant("stop")]
        [Positional]
        [Required]
        [Usage("The docker command")]
        public string Command;
        
        [Usage("The arguments to the 'stop' docker command")]
        [Positional]
        public string[] Args;
    }
    
    public class MeExec
    {
        [Constant("exec")]
        [Positional]
        [Required]
        [Usage("The docker command")]
        public string Command;
        
        [Usage("The arguments to the 'exec' docker command")]
        [Positional]
        public string[] Args;
    }

    public static void Main(string[] ignore)
    {
        var parser = Parser<MeRun,MeStop,MeExec>.Create();
        
        var stop = new []{ "stop", "bla" };
        var run = new []{ "run" };
        var exec = new []{ "exec" };

        var sre = new string[][]{
            stop, run, exec
        };

        foreach (var args in sre)
        {
            parser.Parse(args)
                .When((MeStop ms) => {
                    Assert.Single(ms.Args);
                    Assert.Equal("bla",ms.Args[0]);
                .When((MeRun ms) => {
                    Assert.Null(ms.Args);
                .When((MeExec ms) => {
                    Assert.Null(ms.Args);
                }).Else((e) => {
                    Assert.True(false);
                });
        }
    }
```

## A9. Remainders

Clux can provide you with the remainder when parsing fails, if you specifically request it.

```

        public class Foo
        {
            [Positional]
            [Required]
            [Usage("The command to perform, e.g. run, exec, kill, stop, ...")]
            public string Verb { get; set; }
        }

        string[] remainder;
        try
        {
            Parser<Docker>.Parse(out remainder, new string[0] );
            Assert.False(true);
        }
        catch (MissingRequiredOption)
        {
            var result = Parser<Docker>.Parse(out remainder, new string[] { "verb" } );
            Assert.Equal("verb", result.Verb);
        }
```

## Error Handling

**TODO**

## Help Messages
        
**TODO**
