using System;
using System.Collections.Generic;

internal sealed class Program
{
    public static void Main(string[] args)
    {
        var game = new Game();
        game.KnockedDownPins(4);
        game.KnockedDownPins(6);
        game.KnockedDownPins(5);
        game.KnockedDownPins(5);
        game.KnockedDownPins(10);
        game.KnockedDownPins(6);
    }
}

internal sealed class Game
{
    private readonly List<int> _rolls = new();
    private readonly Rule _rule = new();
    private readonly ConsolePrinter _consolePrinter = new();

    public void KnockedDownPins(int inputPins)
    {
        if (!_rule.IsValidRoll(_rolls, inputPins))
        {
            return;
        }
        
        _rolls.Add(inputPins);
        _consolePrinter.Print(_rolls);
    }
}

internal sealed class Rule
{
    public const int START_FRAME = 1;
    public const int MAX_FRAME = 10;
    public const int STRIKE_ROLLS = 1;
    public const int SPARE_ROLLS = 2;
    public const int MAX_PINS = 10;
    private const int MIN_PINS = 0;
    
    public bool IsValidRoll(IReadOnlyList<int> rolls, int inputPins)
    {
        if (!IsValidPins(inputPins))
        {
            var msg = $"{MIN_PINS} ~ {MAX_PINS} 사이의 값을 입력해주세요.";
            Console.WriteLine(msg);
            return false;
        }

        if (!IsValidBonusRoll(rolls, inputPins))
        {
            return false;
        }

        return IsValidSecondRollInFrame(rolls, inputPins);
    }

    private bool IsValidPins(int inputPins)
    {
        return inputPins >= MIN_PINS 
            && inputPins <= MAX_PINS;
    }

    private bool IsValidBonusRoll(IReadOnlyList<int> rolls, int inputPins)
    {
        var (curFrame, curFrameStartRollIdx) = GetCurFrameAndRollIndex(rolls);
        if (curFrame < MAX_FRAME)
        {
            return false;
        }
        
        var bonusCount = rolls.Count - curFrameStartRollIdx;
        if (bonusCount != 1 && bonusCount != 2)
        {
            return false;
        }

        var firstPins = rolls[curFrameStartRollIdx];
        if (bonusCount == 1)
        {
            if (IsStrike(firstPins))
            {
                return IsValidPins(inputPins);
            }

            return firstPins + inputPins <= MAX_PINS;
        }

        var secondRollIdx = curFrameStartRollIdx + 1;
        if (secondRollIdx >= rolls.Count)
        {
            return false;
        }

        var secondPins = rolls[secondRollIdx];
        return IsStrike(firstPins)
            || IsStrike(firstPins + secondPins);
    }
    
    private (int curFrame, int curRollIdx) GetCurFrameAndRollIndex(IReadOnlyList<int> rolls)
    {
        var rollIdx = 0;
        var curFrame = START_FRAME;

        while (rollIdx < rolls.Count && curFrame <= MAX_FRAME)
        {
            var pins = rolls[rollIdx];
            if (IsStrike(pins))
            {
                rollIdx += STRIKE_ROLLS;
            }
            else
            {
                rollIdx += SPARE_ROLLS;
            }

            curFrame++;
        }

        return (curFrame, rollIdx);
    }

    private bool IsValidSecondRollInFrame(IReadOnlyList<int> rolls, int inputPins)
    {
        if (rolls.Count == 0 || !IsWaitSecondRoll(rolls))
        {
            return false;
        }

        var lastPins = rolls[^1];
        return lastPins + inputPins <= MAX_PINS;
    }

    private bool IsWaitSecondRoll(IReadOnlyList<int> rolls)
    {
        var rollIdx = 0;
        var frame = START_FRAME;

        while (rollIdx < rolls.Count && frame <= MAX_FRAME)
        {
            var pins = rolls[rollIdx];
            if (IsStrike(pins))
            {
                rollIdx += STRIKE_ROLLS;
            }
            else
            {
                var secondRollIdx = rollIdx + 1;
                if (secondRollIdx == rolls.Count)
                {
                    return true;
                }

                rollIdx += SPARE_ROLLS;
            }

            frame++;
        }

        return false;
    }

    public static bool IsStrike(int pins)
    {
        return pins == MAX_PINS;
    }
}

internal sealed class ConsolePrinter
{
    private string _firstRollStr;
    private string _secondRollStr;
    private string _scoreStr;

    private void ResetStr()
    {
        _firstRollStr = " ";
        _secondRollStr = " "; 
        _scoreStr = "     ";
    }
    
    public void Print(IReadOnlyList<int> rolls)
    {
        var frameResultsLineStr = string.Empty;
        var scoreLineStr = string.Empty;
        var rollIdx = 0;
        var score = 0;
        
        for (var frame = Rule.START_FRAME; frame <= Rule.MAX_FRAME; frame++)
        {
            ResetStr();
            ProcessFrame(rolls, ref rollIdx, ref score, frame);
            frameResultsLineStr += $"{frame}:[{_firstRollStr},{_secondRollStr}] ";
            scoreLineStr += $"  {_scoreStr} ";
        }
        
        Console.WriteLine(frameResultsLineStr);
        Console.WriteLine(scoreLineStr);
    }

    private void ProcessFrame(IReadOnlyList<int> rolls, ref int rollIdx, ref int score, int frame)
    {
        if (rollIdx >= rolls.Count)
        {
            return;
        }
        
        var firstPins = rolls[rollIdx];
        if (Rule.IsStrike(firstPins))
        {
            ProcessStrike(rolls, ref rollIdx, ref score);
            return;
        }

        var secondRollIdx = rollIdx + 1;
        if (secondRollIdx < rolls.Count)
        {
            ProcessSecondRoll(rolls, ref rollIdx, ref score, frame);
            return;
        }

        _firstRollStr = FormatRollResult(firstPins);
        rollIdx += 1;
    }
    
    private void ProcessStrike(IReadOnlyList<int> rolls, ref int rollIdx, ref int score)
    {
        const string STRIKE_MSG = "X"; 
        _firstRollStr = STRIKE_MSG;
                
        var spareRollIdx = rollIdx + Rule.SPARE_ROLLS;
        if (spareRollIdx < rolls.Count)
        {
            var strikeRollIdx = rollIdx + Rule.STRIKE_ROLLS;
            var totalPins = rolls[strikeRollIdx] + rolls[spareRollIdx];
            score += Rule.MAX_PINS + totalPins;
            _scoreStr = $"[{score,3}]";
        }

        rollIdx += Rule.STRIKE_ROLLS;
    }
    
    private void ProcessSecondRoll(IReadOnlyList<int> rolls, ref int rollIdx, ref int score, int frame)
    {
        var firstPins = rolls[rollIdx];
        _firstRollStr = FormatRollResult(firstPins);

        var secondRollIdx = rollIdx + 1;
        var secondPins = rolls[secondRollIdx];
        var totalPins = firstPins + secondPins;
        var isStrike = Rule.IsStrike(totalPins);
        _secondRollStr = isStrike ? "/" : FormatRollResult(secondPins);

        var spareRollIdx = rollIdx + Rule.SPARE_ROLLS;
        if (spareRollIdx < rolls.Count || frame == Rule.MAX_FRAME)
        {
            score += totalPins;

            if (isStrike && spareRollIdx < rolls.Count)
            {
                score += rolls[spareRollIdx];
            }
            
            _scoreStr = $"[{score,3}]";
        }

        rollIdx += Rule.SPARE_ROLLS;
    }
    
    private string FormatRollResult(int pins)
    {
        return pins == 0 
            ? "-" 
            : pins.ToString();
    }
}