# Conventions & Naming Approaches

This document captures the naming and code conventions used across the solution
(`Calculator.sln`: `CalculatorApp`, `Calculator`, `Essentials`). The conventions below
were verified against the actual codebase, so existing code and new code should match them.

---

## 1. C# Naming Conventions

| Kind | Convention | Example |
|---|---|---|
| Private instance fields | `_camelCase` | `_entry`, `_presets`, `_tapCount` |
| Private `static readonly` | `PascalCase` (treated as constants) | `Mapper`, `AmoledColors`, `LanczosGammaCoefficients` |
| Private `const` | `PascalCase` | `MaxFontFactor`, `DefaultDecimalSeparator` |
| `public` / `protected` / `internal` members (properties, `BindableProperty` fields, singletons) | `PascalCase` | `PreviewBordersColor`, `IsToggledProperty`, `PiToken.Instance` |
| Local variables | `camelCase` | `firstToken`, `logicalPosition`, `insertIndex` |
| Method parameters | `camelCase` | `fontFamily`, `availableWidth`, `source` |

### Variable name quality

Names should express **what the value is**, not just abbreviate or echo its type.

- Prefer a noun that states the role: `minSize`/`maxSize`/`bestSize` → best font size;
  `firstToken`/`lastToken` → first/last number token of a sequence.
- Name loop/binary-search values by role: `low`/`high`/`midIndex`/`firstIndex`, not `lo`/`hi`/`first`.
- Name split substrings by meaning: `mantissa`/`exponentPart`/`exponentSign`, not `leftE`/`rightE`/`next`.
- Name collection sizes by what they count: `insertCount`, `addedLength`, not bare `count`.
- Avoid: `val`, `first`, `last`, `start`, `best`, `mid` used alone when a role exists
  (`value`, `firstToken`, `lastToken`, `startIndex`, `bestSize`, `midSize`).
- Legitimate exceptions: standard loop counters (`i`), coordinates (`x`, `y`, `row`, `column`),
  event args (`e`, `sender`), and framework-required overrides.

## 2. Naming Approaches (behavioral patterns)

### BindableProperty change callbacks: `On<Property>Changed`

Every `BindableProperty.Create(..., propertyChanged: ...)` callback is named after the
property it observes: `property + Changed`, prefixed with `On`.

```csharp
public static readonly BindableProperty PreviewBordersColorProperty =
    BindableProperty.Create(
        nameof(PreviewBordersColor),
        typeof(Color),
        typeof(SpreadsheetGrid),
        defaultValue: Colors.Transparent,
        propertyChanged: OnPreviewBordersColorChanged);

private static void OnPreviewBordersColorChanged(
    BindableObject bindable, object oldValue, object newValue) => RedrawGrid(bindable);
```

- **One callback per property.** Sharing one callback across properties is avoided; shared
  logic belongs in a private helper (e.g., `RedrawGrid`, `RebuildPresets`).
- Callback methods are `private static`.

### Commands: `<Action>Command`, action-named

Commands express the intent of the action they perform, and their bodies are plain action
methods of the same name.

```csharp
public ICommand EvaluateCommand { get; }
// ...
EvaluateCommand = new Command<IInputOutput>(Evaluate);
// ...
private void Evaluate(IInputOutput inputOutput) { ... }
```

| Old | New |
|---|---|
| `AngleButtonClickedCommand` / `OnAngleButtonClicked` | `ToggleAngleModeCommand` / `ToggleAngleMode` |
| `BackspaceLongPressedCommand` / `OnClearButtonClicked` | `ClearInputCommand` / `ClearInput` |
| `BackspaceShortPressedCommand` / `BackspaceButtonShortPressed` | `DeleteTokenCommand` / `DeleteToken` |
| `EqualButtonClickedCommand` / `OnEqualButtonClicked` | `EvaluateCommand` / `Evaluate` |
| `InverseButtonClickedCommand` / `OnInverseButtonClicked` | `ToggleInverseModeCommand` / `ToggleInverseMode` |
| `LayoutModeCommand` / `OnLayoutModeChanged` | `ToggleLayoutModeCommand` / `ToggleLayoutMode` |
| `ErrorsCopyRequested` | `CopyErrorsCommand` |
| `ClearAllItems` | `DeleteAll` |

### Event handlers: `On<Event>`, named after the event

Handler methods are named after the event they handle.

- `Clicked` → `OnClicked`, `OnTitleClicked`, `OnOpenClicked`, `OnNavigateClicked`
- `Loaded` → `OnLoaded`, `TextChanged` → `OnTextChanged`, `SizeChanged` → `OnSizeChanged`
- `StartInteraction` / `DragInteraction` / `EndInteraction` → `OnStartInteraction`, `OnDragInteraction`, `OnEndInteraction`
- Collection changes → `OnItemsCollectionChanged`

### Others

- User-visible strings live in code-behind/`x:Static` sources (e.g., `fonts:MaterialSymbols`),
  not inline magic strings.
- A command property and its body share the same verb: `DeleteAllCommand` executes `DeleteAll`,
  `ClearInputCommand` executes `ClearInput` — never `On…Requested` or past-tense bodies.

## 3. XAML Conventions

### Resources: `DynamicResource` lives only in `Styles.xaml`

`DynamicResource` is used **only** inside `CalculatorApp/Resources/Styles/Styles.xaml`
(keyed styles). Pages and controls reference those styles via
`Style="{StaticResource <Key>}"` — never use `DynamicResource` directly in a page.

Example: `ExceptionLog.xaml` and `LayoutBuilder.xaml` used to reference `DynamicResource`
for shared visuals; the underlying styles were moved into `Styles.xaml` as keyed styles
(`PrimaryIcon`, `TertiaryButton`, `ErrorButton`, `ExceptionLogHeader`, …) and consumed with
`StaticResource`.

### `xmlns:` aliases match the namespace they point to

Alias prefixes mirror the folder/namespace they resolve to. When a folder is renamed, the
aliases in every file that references it are renamed accordingly.

| Alias | Namespace |
|---|---|
| `buttons` | `...CalculatorPage.Buttons` |
| `base` | ...`Buttons.Base` or the base type namespace in use |
| `display` | ...`MainGrid.Display` |
| `history`, `input`, `output`, `page` | the corresponding sub-folder namespaces |

### Compiled bindings

- Data templates and pages declare `x:DataType` so bindings are compiled.

### Indentation & attribute layout

- One nesting level = **4 spaces**;
- `xmlns` declarations come first on the root element, then `x:Class` /
  `x:DataType` / event-handler attributes.
- Enforced by `.editorconfig` (`[*.xaml]`, `indent_size = 4`) and
  `Calculator.sln.DotSettings` (XmlFormatterSettings).

## 4. Project-Structure Conventions

- Namespaces are file-scoped and follow the folder structure
  (`CalculatorApp.SettingsPage.CustomLayout.LayoutBuilder`).
- `Platforms/Android` holds platform-specific handlers/extensions; app logic lives in the
  portable projects (`CalculatorApp`, `Calculator`, `Essentials`).
- `Calculator` (pure expression model), `Essentials` (token types, immutable collections,
  result types), `CalculatorApp` (MAUI UI).

## 5. C# Formatting & Structural Conventions

### Indentation, line endings, encoding

- **4 spaces** per level; continuation lines indent **4 spaces** past the construct
  they wrap — no alignment to the opening parenthesis.
- UTF-8, LF endings, final newline. Enforced by `.editorconfig` (`root = true`,
  `[*]` defaults) and the shared `Calculator.sln.DotSettings`. The per-user
  `Calculator.sln.DotSettings.user` stays local and untracked.

### Braces: Always present

- `{` goes on its **own line** for types, methods, properties, accessors, and
  control flow. `else`, `catch`, `finally`, and do-`while` open on a new line
  after the closing brace.

```csharp
public void SetSizeScale(string sizeName, double value)
{
    if (_factors.TryGetValue(sizeName, out double previous))
    {
        ...
    }
    else
    {
        ...
    }
}
```

- **Braces are always written**, even for single-statement bodies — no
  brace-less `if`/loop bodies.
- Members whose body is a single expression use `=>` instead
  (expression-bodied): `public string AppFont => UseSystemFont ? ... : ...;`,
  `{ get => GetSizeScale(name); set => SetSizeScale(name, value); }`.

### Spacing around keywords, parens, and operators

- Space after control keywords: `if (`, `while (`, `for (`, `foreach (`,
  `switch (`, `catch (`, `lock (`. Method **calls** attach the paren directly:
  `Evaluate(x)`, `Preferences.Set(name, value)`.
- No space inside parentheses: `if (x > 1)`, never `if ( x > 1 )`.
- Space around binary operators: `startIndex + count`, `operand >= 0`,
  `z + 7.5`. No space before the `(` of a call or after `[`/`]` in indexers.
- `?` / `:` in ternaries: spaces around them (`a ? b : c`).

### Declarations and members

- File-scoped namespaces; the body is one level deep (4 spaces).
- One declaration per line; wrapped parameter lists indent 4 (see the
  `BindableProperty.Create(` example in section 2).
- Arrays/collections initialized with one entry per line and a trailing comma;
  closing `];` on its own line.
- Extension methods take `this` as the first parameter and live in
  `<Noun>Extensions` classes (`OperandExtensions`, `ExpressionParsingExtensions`).
- `nameof()` is used for member names in bindings, `Preferences`/dictionary
  keys, and `BindableProperty.Create` — never string literals.
- Null-conditional `?.` and C# pattern matching (`is { }`, property patterns)
  are preferred over null checks with `== null` guards where they read clearest
  (`e.AffectedPart is { } affectedPart`).

### Call chains

Chained fluent calls start a new line indented 4 spaces, one call per line
(`.SelfMap(...)`, `.SelectMany(...)`, `.Where(...)`).

## Tooling & enforcement

| File | Scope | Used by |
|---|---|---|
| `.editorconfig` (repo root) | indentation, braces, spacing, XAML layout | Rider, Visual Studio, `dotnet format` |
| `Calculator.sln.DotSettings` | ReSharper-native formatting keys (shared layer) | Rider / ReSharper |
| `Calculator.sln.DotSettings.user` | machine-local settings — **not committed** | Rider |

Adjusting a rule in Rider and saving to the `Calculator.sln` layer updates the
shared `.DotSettings`; `.editorconfig` changes apply to every IDE and CLI
formatter that honors it.
