using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Calculator.ExpressionComposition;
using Calculator.ExpressionParsing;
using Calculator.ExpressionSolving;
using CalculatorApp.CalculatorPage.Buttons;
using CalculatorApp.CalculatorPage.History;
using CalculatorApp.CalculatorPage.Input;
using CalculatorApp.CalculatorPage.Output;
using CalculatorApp.SettingsPage.Animations;
using CalculatorApp.SettingsPage.App;
using CalculatorApp.SettingsPage.Formatting;
using CalculatorApp.SettingsPage.History;
using CalculatorApp.SettingsPage.Layout;
using Essentials.Calculator;
using Essentials.ResultType;

namespace CalculatorApp.CalculatorPage.Page;

public class CalculatorPageViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly Tokens _tokens;
    private bool _isAnimating;

    public AnimationSettings AnimationSettings { get; }
    public FormattingSettings FormattingSettings { get; }
    public HistorySettings HistorySettings { get; }
    public HistoryViewModel HistoryViewModel { get; }
    public CalculatorInputViewModel InputViewModel { get; }
    public CalculatorOutputViewModel OutputViewModel { get; }
    public LayoutSettings LayoutSettings { get; }
    public AppSettings AppSettings { get; }

    public AngleMode AngleMode
    {
        get;
        private set
        {
            field = value;
            OnPropertyChanged();
        }
    } = AngleMode.RAD;

    public string InverseMode
    {
        get;
        private set
        {
            field = value;
            OnPropertyChanged();
        }
    } = "1/2";

    public ICommand ToggleAngleModeCommand { get; }
    public ICommand ClearInputCommand { get; }
    public ICommand DeleteTokenCommand { get; }
    public ICommand EvaluateCommand { get; }
    public ICommand AppendTokenCommand { get; }
    public ICommand AppendTokensCommand { get; }
    public ICommand ToggleInverseModeCommand { get; }

    public CalculatorPageViewModel(CalculatorInputViewModel inputViewModel,
        CalculatorOutputViewModel outputViewModel, FormattingSettings formattingSettings,
        HistoryViewModel historyViewModel, HistorySettings historySettings,
        AnimationSettings animationSettings, LayoutSettings layoutSettings,
        AppSettings appSettings, Tokens tokens)
    {
        FormattingSettings = formattingSettings;
        AnimationSettings = animationSettings;
        InputViewModel = inputViewModel;
        OutputViewModel = outputViewModel;
        HistoryViewModel = historyViewModel;
        HistorySettings = historySettings;
        LayoutSettings = layoutSettings;
        AppSettings = appSettings;
        _tokens = tokens;

        ToggleAngleModeCommand = new Command(ToggleAngleMode);
        ClearInputCommand = new Command(ClearInput);
        DeleteTokenCommand = new Command(DeleteToken);
        EvaluateCommand = new Command<IInputOutput>(Evaluate);
        AppendTokenCommand = new Command<IToken>(AppendTokenToInput);
        AppendTokensCommand = new Command<IReadOnlyList<IToken>>(AppendTokensToInput);
        ToggleInverseModeCommand = new Command(ToggleInverseMode);
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void ToggleAngleMode()
    {
        AngleMode = AngleMode == AngleMode.RAD ? AngleMode.DEG : AngleMode.RAD;
        UpdateOutput();
    }

    private void ToggleInverseMode()
    {
        InverseMode = InverseMode == "1/2" ? "2/2" : "1/2";

        foreach (var button in LayoutSettings.GridViewModel.Parts.Values.SelectMany(x => x))
        {
            if (button is AlternateButtonViewModel alteringButton)
            {
                alteringButton.ToAlternate();
            }
        }
    }

    private void ClearInput()
    {
        if (_isAnimating) return;

        InputViewModel.Clear();
        UpdateOutput();
    }

    private void UpdateOutput()
    {
        OutputViewModel.Update(ExpressionParsingExtensions
            .Parse(_tokens, FormattingSettings.NumberFormatInfo, AngleMode)
            .Map(ExpressionEvaluatingExtensions.EvaluateAndRound));
    }

    private void Evaluate(IInputOutput inputOutput)
    {
        if (_isAnimating)
        {
            return;
        }

        OutputViewModel.ConsumeResult(async result =>
        {
            _isAnimating = true;
            var resultToken = new NumberToken(result, FormattingSettings.DecimalSeparator);
            if (HistorySettings.HistoryEnabled)
            {
                var item = new HistoryItem
                {
                    Expression = [.._tokens],
                    Result = resultToken
                };
                HistoryViewModel.InsertCommand.Execute(item);
            }
            await inputOutput.AnimateResultPreTransition(AnimationSettings);
            InputViewModel.SetResult(resultToken);
            await inputOutput.AnimateResultPostTransition(AnimationSettings);
            _isAnimating = false;
        }, error =>
        {
            AppSettings.UserMessageDisplayer.ShowMessage(error.Message, null);
        });
    }

    private void AppendTokenToInput(IToken token)
    {
        if (_isAnimating)
        {
            return;
        }

        InputViewModel.AddTokenAtCursor(token);
        UpdateOutput();
    }

    private void AppendTokensToInput(IReadOnlyList<IToken> tokens)
    {
        if (_isAnimating)
        {
            return;
        }

        InputViewModel.AddTokensAtCursor(tokens);
        UpdateOutput();
    }

    private void DeleteToken()
    {
        if (_isAnimating)
        {
            return;
        }

        InputViewModel.RemoveTokenAtCursor();
        UpdateOutput();
    }
}
