# MCalc

A clean, high-precision calculator for Android, built with .NET 10 MAUI. MCalc prioritizes readability, customization, and the advanced functions most basic mobile calculators leave out.

[![Get it on Obtainium](assets/badge_obtainium.png)](https://apps.obtainium.imranr.dev/redirect?r=obtainium%3A%2F%2Fapp%2F%257B%2522id%2522%253A%2522com.mcalc.app%2522%252C%2522url%2522%253A%2522https%253A%252F%252Fgithub.com%252FIRefactoredThat%252FMCalc%2522%252C%2522author%2522%253A%2522IRefactoredThat%2522%252C%2522name%2522%253A%2522MCalc%2522%257D)

## Features

### Expression Evaluation

- **Real-time calculation** - results update as you build an expression, not just when you press =
- **Arithmetic operations** - add (`+`), subtract (`-`), divide (`÷`), multiply (`x`)
- **Scientific operations** - power (`^`), square root (`√`), cube root (`∛`), natural log (`ln`), base-10 log (`log`), factorial (`n!`), modulo (`%`)
- **Trigonometric operations** - standard (`sin`, `cos`, `tan`), inverse (`asin`, `acos`, `atan`), reciprocal (`sec`, `csc`, `cot`), their inverses (`asec`, `acsc`, `acot`), and the complete **hyperbolic** set: `sinh`, `cosh`, `tanh`, `csch`, `sech`, `coth`, `asinh`, `acosh`, `atanh`, `acsch`, `asech`, `acoth`
- **Angle modes** - switch between radians (RAD) and degrees (DEG)
- **Constants** - built-in `π` and `e`

### Customizable Keypad

- **Three layouts** - `Simple` (classic phone layout), `Mixed` (adds scientific keys), or `Custom`
- **Fully custom layouts** - design your own keypad in the spreadsheet-style layout editor. Place any function on any cell, arrange keys to your liking, and save it (will be further worked on)
- **Inverse functions at a tap** - toggle between trigonometric and hyperbolic functions and their inverses without cluttering the UI

### Appearance & Theming

- **Material 3 dynamic theming** - seven scheme styles: Vibrant, TonalSpot, Expressive, Rainbow, FruitSalad, Content, Fidelity
- **Wallpaper-based colors** - on Android 12+, pull colors straight from your wallpaper
- **Manual color control** - pick a seed color and MCalc derives the entire palette
- **Light, dark, or auto** - follow the system theme or lock to one
- **Darker mode** - darken the tone of colors to reduce eye strain and save battery
- **Fine-tuned typography** - three monospace fonts or your system font, with independent scaling for input, output, history, and every settings screen (open to changing fonts)

### Usability

- **Expression history** - calculations save after clicking =; tap any entry to recall it
- **Click to copy** - automatically copies expression or result from history into the current calculation
- **Live formatting** - configurable decimal separator (comma or period)
- **Smart E-notation** - force scientific notation on, off, or let it switch automatically by magnitude
- **Configurable animations** - toggle or tune click, transition, and morph animations
- **Readable at a glance** - large, legible input and output

### In-app screenshots

| | | |
|:---:|:---:|:---:|
| <img src="Screenshots/display.png" width="230" alt="Main calculator display"> | <img src="Screenshots/settings.png" width="230" alt="Settings page"> | <img src="Screenshots/formatting.png" width="230" alt="Number formatting settings"> |
| <img src="Screenshots/layout.png" width="230" alt="Custom layout editor"> | <img src="Screenshots/history.png" width="230" alt="Expression history"> | |

## Development

The build toolchain (the .NET SDK with the Android workload baked into its
store root, the pinned Android SDK, and a JDK) is provided by a Nix flake in
this repository — nothing needs to be installed system-wide.

```sh
direnv allow .      # once: auto-activates the shell on cd (needs direnv)
# or enter it explicitly:
nix develop
```

Then build as usual:

```sh
dotnet build CalculatorApp/CalculatorApp.csproj -c Debug
```

The flake pins `nixpkgs` and `tadfisher/android-nixpkgs`, so the toolchain is
reproducible and cached after the first build.

### Updating the toolchain

The .NET Android workload is store-packaged from a hash-pinned lock
(`nix/android-workload-packs.nix`), and the SDK line the toolchain is built
from is pinned in `nix/sdk-pin.nix`. One command per operation refreshes the
lock and rebuilds the toolchain, so the two can never drift apart:

```sh
nix run .#update-workload    # newest published .NET Android workload
nix run .#update-sdk         # newest .NET SDK patch of the pinned line
nix run .#upgrade-sdk 11     # move the toolchain to another .NET SDK line
nix run .#status             # what is pinned, and whether an update exists
```

`update-workload` bootstraps a throwaway workload state with
`dotnet workload restore`, so it always picks up the newest published packs;
`update-sdk` first moves the `nixpkgs` pin forward; `upgrade-sdk` rewrites the
SDK line. Each command ends by rebuilding the store-packaged SDK, so an
inconsistent lock is caught right away — and if an update fails half-way, the
pin and the lock are restored.

Moving to another SDK line also touches the project: the target frameworks and
the MAUI package versions follow the new SDK. The tool prints what is left to
change.

To lock an existing workload state instead of bootstrapping a new one, run the
generator directly:

```sh
DOTNET_STATE=~/.dotnet ./nix/gen-android-workload.sh
```
