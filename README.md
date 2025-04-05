# BlockyScript
Broaden your perspective on how computer programs work by programming a robot to solve different tasks using simple commands.

![Game Screenshot 1](https://img.itch.zone/aW1hZ2UvMTMzMjM2OS84MDIxNzQyLnBuZw==/original/ulliGv.png)

# Commands

Available commands are:
- `move: SIDE DISTANCE`
- `jump`
- `pick: SIDE`
- `copy: SIDE`
- `drop: SIDE`

`SIDE` represents one of the letters: `N`, `S`, `E`, `W` (standing for North, South, East, West)
`DISTANCE` represents whole number which is bigger than `0`

_Note: All commands except for `jump` have colon (`:`) after command name, and one or two following parameters (`SIDE` and `DISTANCE`)._



# Functions
Function is named sequence of commands. Instead of copying and pasting same sequence multiple times just put it in a function and by writing the function name in main part of your program commands from function will be executed.

Example:
```
@MyFunction
move: S 3
copy: E
drop: W
jump

@Program
MyFunction
move: W 1
MyFunction
MyFunction
```

_Note: Syntax rule is to write `@` symbol before function name and all commands below will be part of that function. Also to tell robot what to execute we write special function called `Program`, and below write all commands and functions we want to be executed when pressing **Run**. When no custom function is defined the `Program` function is not needed, just  write commands!_


# New in version 1.6
You now can repeat command by writing exclamation mark (`!`) in front of command name followed by number of times to execute a command.

Example:

```
!15 copy: N
```

This is same as writing command `copy: N` 15 times

_Note: Space must be present before command name_

_Tip: Functions can also can repeated, like this:_
```
!9 MyFunction
```




