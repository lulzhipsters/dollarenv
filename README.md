# What is this?
DollarEnv ($env) aims to provide easy import of dotenv (.env) files for powershell in the same way many other languages support them, most famously, [nodejs](https://github.com/motdotla/dotenv)

# Supported format
These are explicitly supported. Other behaviours are undefined

## Keys
Can contain
  - Upper and lower case characters
  - Alphanumeric characters
  - Underscores

Cannot contain
  - Spaces
  - Line breaks
  - `#`

## Values
Supports quoting using `"`

When quoted, can contain other characters, such as
- `\ `
- `#`
- newlines
- whitespace

When not quoted, values can only contain non-special characters.

# Intended future support
  - Variable Expansion (see [dotenv-expand](https://github.com/motdotla/dotenv-expand))