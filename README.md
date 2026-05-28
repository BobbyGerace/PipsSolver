# PipsSolver

Solves NYT Pips games.

I wrote this by hand as a personal project to practice C#.

# Usage

`dotnet run path/to/gamefile.txt`

## File syntax

The input is a text file which represents a Pips game board.

The text file has three sections, separated by a `---` line.

1. Grid - a character representation of the grid, where the `*` character represents cells and letter characters represent constraint groups.
2. Constrains - Each constraint goes on its own line and takes the form `<char>: <op> <value?>`. Where
   - `char` corresponds to the cells marked with that character in the grid
   - `op` is one of `==` (all equal), `!=` (none equal), `=` (sum equals value), `>`, `<`
   - `value` is a number, only applicable for `=`, `<`, and `>`
3. Dominos - a list of dominos, each on its own line, with left and right values separated by a space

Example:

```
*A C*
 *BB  EE
   D

---

A: = 3
B: ==
C: < 1
D: > 5
E: !=

---

6 4
0 3
4 2
3 1
5 6
```

## Output

If the puzzle is solvable, prints a domino layout representing the solution.

Example:

```
┌───────┐   ┌───────┐
│ 1   3 │   │ 0   3 │
└───┬───┴───┼───┬───┘   ┌───────┐
    │ 2   4 │ 4 │       │ 5   6 │
    └───────┤   │       └───────┘
            │ 6 │
            └───┘
```
