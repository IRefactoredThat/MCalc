using System.Collections;
using System.Text;
using CalculatorApp.SettingsPage.Formatting;
using Essentials.Calculator;

namespace CalculatorApp.CalculatorPage.Input;

public class Tokens : IReadOnlyList<IToken>
{
    private readonly List<IToken> _tokens = [];

    public const int MaxTotalDigits = 15, Precision = 5;

    private int _tokenIndex, _tokenOffset;
    public int DisplayPosition { get; private set; }

    public void Clear() => _tokens.Clear();

    private static int MapNumberDisplayToTokenOffset(int displayOffset, int wholePartLength)
    {
        var integerDisplayOffset = Math.Min(displayOffset, wholePartLength + (wholePartLength - 1) / 3);
        var firstGroup = (wholePartLength - 1) % 3 + 1;

        var separatorCount = integerDisplayOffset <= firstGroup ?
            0 : (integerDisplayOffset - firstGroup + 3) / 4;

        return displayOffset - separatorCount;
    }

    private int GetLogicalPosition() => _tokens
        .Take(_tokenIndex)
        .Sum(token => token.Value.Length) + _tokenOffset;

    public void UpdateTokenPosition(int cursorPosition, bool afterThousandSeparator)
    {
        if (_tokens.Count == 0)
        {
            _tokenIndex = _tokenOffset = DisplayPosition = 0;
            return;
        }
        var displayIndex = 0;

        for (var tokenIndex = 0; tokenIndex < _tokens.Count; tokenIndex++)
        {
            var token = _tokens[tokenIndex];
            var wholePartLength = token is NumberToken number ? number.IntegerLength : 0;
            var totalSeparators = (wholePartLength - 1) / 3;
            var displayLength = token.Value.Length + totalSeparators;

            if (cursorPosition < displayIndex + displayLength)
            {
                var displayOffset = cursorPosition - displayIndex;
                var tokenOffset = token switch
                {
                    OperatorToken op => displayOffset <= op.Value.Length / 2
                        ? 0 : op.Value.Length,
                    NumberToken => MapNumberDisplayToTokenOffset(displayOffset, wholePartLength),
                    _ => displayOffset
                };

                if (token is OperatorToken)
                {
                    DisplayPosition = cursorPosition + tokenOffset - displayOffset;
                }
                else if (token is NumberToken && afterThousandSeparator)
                {
                    DisplayPosition = cursorPosition - 1;
                }
                else
                {
                    DisplayPosition = cursorPosition;
                }

                _tokenIndex = tokenIndex;
                _tokenOffset = tokenOffset;
                return;
            }

            displayIndex += displayLength;
        }

        _tokenIndex = _tokens.Count - 1;
        _tokenOffset = _tokens[^1].Value.Length;
        DisplayPosition = cursorPosition;
    }

    public int AddToken(IToken newToken, DecimalSeparator separator)
    {
        if (_tokens.Count == 0)
        {
            _tokens.Add(newToken);
            return newToken.Value.Length;
        }

        var token = _tokens[_tokenIndex];
        var logicalPosition = GetLogicalPosition();

        if (token is NumberToken current && newToken is NumberToken)
        {
            if (current.Value.Length == MaxTotalDigits)
            {
                return logicalPosition;
            }
            var newValue = current.Value.Insert(_tokenOffset, newToken.Value);
            _tokens[_tokenIndex] = new NumberToken(newValue, (char)separator);
        }
        else if (token is NumberToken && newToken is OperatorToken)
        {
            if (_tokenOffset == 0)
            {
                _tokens.Insert(_tokenIndex, newToken);
            }
            else if (_tokenOffset == token.Value.Length)
            {
                _tokens.Insert(_tokenIndex + 1, newToken);
            }
            else
            {
                var value = token.Value;
                var left = value[.._tokenOffset];
                var right = value[_tokenOffset..];

                _tokens[_tokenIndex] = new NumberToken(left, (char)separator);
                _tokens.Insert(_tokenIndex + 1, newToken);
                _tokens.Insert(_tokenIndex + 2, new NumberToken(right, (char)separator));
            }
        }
        else if (token is OperatorToken && newToken is NumberToken)
        {
            if (_tokenOffset == 0)
            {
                if (_tokenIndex == 0)
                {
                    _tokens.Insert(0, newToken);
                }
                else if (_tokens[_tokenIndex - 1] is NumberToken num)
                {
                    if (num.Value.Length == MaxTotalDigits)
                    {
                        return logicalPosition;
                    }
                    _tokens[_tokenIndex - 1] = new NumberToken(num.Value + newToken.Value, (char)separator);
                }
                else if (_tokens[_tokenIndex - 1] is OperatorToken)
                {
                    _tokens.Insert(_tokenIndex, newToken);
                }
            }
            else if (_tokenOffset == token.Value.Length)
            {
                if (_tokenIndex == _tokens.Count - 1)
                {
                    _tokens.Add(newToken);
                }
                else if (_tokens[_tokenIndex + 1] is NumberToken num)
                {
                    if (num.Value.Length == MaxTotalDigits)
                    {
                        return logicalPosition;
                    }
                    _tokens[_tokenIndex + 1] = new NumberToken(newToken.Value + num.Value, (char)separator);
                }
                else if (_tokens[_tokenIndex + 1] is OperatorToken)
                {
                    _tokens.Insert(_tokenIndex, newToken);
                }
            }
        }
        else if (token is OperatorToken && newToken is OperatorToken)
        {
            if (_tokenOffset == 0)
            {
                _tokens.Insert(_tokenIndex, newToken);
            }
            else if (_tokenOffset == _tokens[_tokenIndex].Value.Length)
            {
                _tokens.Insert(_tokenIndex + 1, newToken);
            }
        }

        return logicalPosition + newToken.Value.Length;
    }

    public int RemoveToken(DecimalSeparator separator)
    {
        if (_tokenIndex == 0 && _tokenOffset == 0)
        {
            return 0;
        }

        var token = _tokens[_tokenIndex];
        var logicalPosition = GetLogicalPosition();

        if (_tokenOffset == 0 && _tokenIndex > 1 &&
            token is NumberToken right &&
            _tokens[_tokenIndex - 1] is OperatorToken op &&
            _tokens[_tokenIndex - 2] is NumberToken left)
        {
            if (left.Value.Length + right.Value.Length >= MaxTotalDigits)
            {
                return logicalPosition;
            }
            var newNumber = new NumberToken(left.Value + right.Value, (char)separator);
            _tokens[_tokenIndex - 2] = newNumber;
            _tokens.RemoveRange(_tokenIndex - 1, 2);
            return logicalPosition - op.Value.Length;
        }

        if (token is NumberToken num)
        {
            if (_tokenOffset == 0 && _tokenIndex > 0)
            {
                var removedLength = _tokens[_tokenIndex - 1].Value.Length;
                _tokens.RemoveAt(_tokenIndex - 1);
                return logicalPosition - removedLength;
            }

            if (num.Value.Length == 1)
            {
                _tokens.RemoveAt(_tokenIndex);
                return logicalPosition - 1;
            }

            if (num.Value[_tokenOffset - 1] == 'E')
            {
                var mantissa = num.Value[..(_tokenOffset - 1)];

                _tokens[_tokenIndex] = new NumberToken(mantissa, (char)separator);

                if (_tokenOffset < num.Value.Length)
                {
                    var exponentSign = num.Value[_tokenOffset];

                    if (exponentSign == '+')
                    {
                        _tokens.Insert(_tokenIndex + 1, PlusToken.Instance);
                        var exponentPart = num.Value[(_tokenOffset + 1)..];
                        _tokens.Insert(_tokenIndex + 2, new NumberToken(exponentPart, (char)separator));
                    }
                    else if (exponentSign == '-')
                    {
                        _tokens.Insert(_tokenIndex + 1, MinusToken.Instance);
                        var exponentPart = num.Value[(_tokenOffset + 1)..];
                        _tokens.Insert(_tokenIndex + 2, new NumberToken(exponentPart, (char)separator));
                    }
                    else
                    {
                        var exponentPart = num.Value[_tokenOffset..];
                        _tokens.Insert(_tokenIndex + 1, new NumberToken(exponentPart, (char)separator));
                    }
                }

                return logicalPosition - 1;
            }

            var newValue = num.Value.Remove(_tokenOffset - 1, 1);
            _tokens[_tokenIndex] = new NumberToken(newValue, (char)separator);
            return logicalPosition - 1;
        }

        if (_tokenOffset == 0)
        {
            var previousToken = _tokens[_tokenIndex - 1];

            if (previousToken is NumberToken previousNumber)
            {
                if (previousNumber.Value.Length == 1)
                {
                    _tokens.RemoveAt(_tokenIndex - 1);
                }
                else
                {
                    _tokens[_tokenIndex - 1] = new NumberToken(previousNumber.Value[..^1], (char)separator);
                }

                return logicalPosition - 1;
            }

            var removedLength = previousToken.Value.Length;
            _tokens.RemoveAt(_tokenIndex - 1);

            return logicalPosition - removedLength;
        }

        var currentTokenLength = _tokens[_tokenIndex].Value.Length;
        _tokens.RemoveAt(_tokenIndex);
        return logicalPosition - currentTokenLength;
    }

    public int AddTokens(IReadOnlyList<IToken> sequence, DecimalSeparator separator)
    {
        if (sequence.Count == 0)
            return GetLogicalPosition();

        var logicalPosition = GetLogicalPosition();
        var insertIndex = Math.Clamp(_tokenIndex + 1, 0, _tokens.Count);

        NumberToken? left = null;
        NumberToken? right = null;
        var firstToken = sequence[0] as NumberToken;
        var lastToken = sequence[^1] as NumberToken;

        bool mergeLeft =
            firstToken != null &&
            _tokenIndex >= 0 &&
            _tokenIndex < _tokens.Count &&
            (left = _tokens[_tokenIndex] as NumberToken) != null;

        bool mergeRight =
            lastToken != null &&
            insertIndex < _tokens.Count &&
            (right = _tokens[insertIndex] as NumberToken) != null;

        if (sequence.Count == 1 && mergeLeft && mergeRight)
        {
            if (left!.Value.Length + firstToken!.Value.Length + right!.Value.Length >= MaxTotalDigits)
            {
                return logicalPosition;
            }

            _tokens[_tokenIndex] = new NumberToken(left.Value + firstToken.Value + right.Value, (char)separator);
            _tokens.RemoveAt(insertIndex);

            return logicalPosition + firstToken.Value.Length;
        }

        var startIndex = mergeLeft ? 1 : 0;
        var insertCount = sequence.Count - (mergeLeft ? 1 : 0) - (mergeRight ? 1 : 0);

        var addedLength = 0;

        if (insertCount > 0)
        {
            for (var i = startIndex; i < insertCount; i++)
            {
                addedLength += sequence[i].Value.Length;
            }

            _tokens.InsertRange(insertIndex, sequence.Skip(startIndex).Take(insertCount));
        }

        if (mergeLeft)
        {
            if (left!.Value.Length + firstToken!.Value.Length >= MaxTotalDigits)
            {
                return logicalPosition;
            }

            _tokens[_tokenIndex] = new NumberToken(left.Value + firstToken.Value, (char)separator);
            addedLength += firstToken.Value.Length;
        }

        if (mergeRight)
        {
            if (lastToken!.Value.Length + right!.Value.Length >= MaxTotalDigits)
            {
                return logicalPosition;
            }

            _tokens[insertIndex + insertCount] = new NumberToken(lastToken.Value + right.Value, (char)separator);
            addedLength += lastToken.Value.Length;
        }

        return logicalPosition + addedLength;
    }

    private readonly StringBuilder _buffer = new();

    public (string Text, int Position) GetTextInfo(int logicalPosition,
        ThousandSeparator thousandSeparator, DecimalSeparator decimalSeparator)
    {
        var (logicalIndex, displayIndex) = (0, 0);
        int? selection = null;

        void EmitRaw(char c)
        {
            if (logicalIndex == logicalPosition)
                selection = displayIndex;

            _buffer.Append(c);

            logicalIndex++;
            displayIndex++;
        }

        void EmitInserted(char c)
        {
            _buffer.Append(c);
            displayIndex++;
        }

        foreach (var token in _tokens)
        {
            if (token is NumberToken number)
            {
                var firstGroup = number.IntegerLength % 3;
                if (firstGroup == 0)
                    firstGroup = 3;

                for (var offset = 0; offset < number.IntegerLength; offset++)
                {
                    EmitRaw(number.Value[offset]);

                    var isGroupEnd =
                        offset + 1 < number.IntegerLength &&
                        ((offset + 1 == firstGroup) ||
                         ((offset + 1 > firstGroup) &&
                          ((offset + 1 - firstGroup) % 3 == 0)));

                    if (isGroupEnd)
                    {
                        EmitInserted((char)thousandSeparator);
                    }
                }

                if (logicalIndex == logicalPosition)
                    selection = displayIndex;

                if (number.IntegerLength < number.Value.Length)
                {
                    EmitRaw(number.Value[number.IntegerLength] == 'E' ? 'E' : (char)decimalSeparator);
                }

                for (var i = number.IntegerLength + 1; i < number.Value.Length; i++)
                {
                    EmitRaw(number.Value[i]);
                }

                if (logicalIndex == logicalPosition)
                    selection = displayIndex;
            }
            else
            {
                foreach (var character in token.Value)
                {
                    EmitRaw(character);
                }

                if (logicalIndex == logicalPosition)
                    selection = displayIndex;
            }
        }

        var text = _buffer.ToString();
        _buffer.Clear();
        return (text, selection ?? displayIndex);
    }

    public IEnumerator<IToken> GetEnumerator() => _tokens.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => _tokens.Count;
    public IToken this[int index] => _tokens[index];
}
