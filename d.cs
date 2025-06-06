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
    private readonly Print _print = new();
    private readonly Rule _rule;
    private readonly List<int> _pinHistoryList = new();

    public Game()
    {
        _rule = new Rule(_pinHistoryList);
    }

    public void KnockedDownPins(int inputPins)
    {
        if (!_rule.IsValidRoll(inputPins))
        {
            return;
        }
        
        _pinHistoryList.Add(inputPins);
    }
}

internal sealed class Rule
{
    private const int MIN_PINS = 0;
    private const int MAX_PINS = 10;
    private const int START_FRAME = 1;
    private const int MAX_FRAME = 10;
    private const int STRIKE = 1;
    private const int NORMAL_FRAME = 2;
    private readonly IReadOnlyList<int> _pinHistoryList;
    
    public Rule(IReadOnlyList<int> pinHistoryList)
    {
        _pinHistoryList = pinHistoryList;
    }
    
    public bool IsValidRoll(int inputPins)
    {
        if (!IsValidPins(inputPins))
        {
            Console.WriteLine("0 ~ 10 사이의 값을 입력해주세요.");
            return false;
        }

        if (!IsValidBonusRoll(inputPins))
        {
            return false;
        }

        return IsValidSecondRollInFrame(inputPins);
    }

    private bool IsValidPins(int inputPins)
    {
        return inputPins >= MIN_PINS 
            && inputPins <= MAX_PINS;
    }

    private bool IsValidBonusRoll(int inputPins)
    {
        var (curFrame, curPinHistoryIdx) = GetCurFrameAndPinHistoryIdx();
        if (curFrame <= MAX_FRAME)
        {
            return true;
        }

        var maxFrameStartIdx = curPinHistoryIdx;
        var bonusCount = _pinHistoryList.Count - maxFrameStartIdx;
        
        if (bonusCount != 1 && bonusCount != 2)
        {
            return false;
        }

        var pins = _pinHistoryList[maxFrameStartIdx];
        if (bonusCount == 1)
        {
            if (pins == MAX_FRAME)
            {
                return IsValidPins(inputPins);
            }

            return pins + inputPins <= MAX_PINS;
        }

        var nextPins = _pinHistoryList[maxFrameStartIdx + 1];
        return pins == MAX_PINS 
            || pins + nextPins == MAX_PINS;
    }
    
    private (int curFrame, int curPinHistoryIdx) GetCurFrameAndPinHistoryIdx()
    {
        var pinHistoryIndex = 0;
        var curFrame = START_FRAME;

        while (pinHistoryIndex < _pinHistoryList.Count && curFrame <= MAX_FRAME)
        {
            var pins = _pinHistoryList[pinHistoryIndex];
            if (pins == MAX_PINS)
            {
                pinHistoryIndex += STRIKE;
            }
            else
            {
                pinHistoryIndex += NORMAL_FRAME;
            }

            curFrame++;
        }

        return (curFrame, pinHistoryIndex);
    }

    private bool IsValidSecondRollInFrame(int inputPins)
    {
        if (_pinHistoryList.Count == 0)
        {
            return true;
        }

        var lastPins = _pinHistoryList[^1];
        return lastPins + inputPins <= MAX_PINS;
    }
}

internal sealed class Print
{
    
}






using System;
using System.Collections.Generic;

internal class Program1
{
    public static void Maind(string[] args)
    {
        Game1 game1 = new Game1();

        // while (true)
        // {
        //     Console.Write("핀 수를 입력하세요 (0~10): ");
        //     string input = Console.ReadLine();
        //
        //     if (int.TryParse(input, out int pins))
        //     {
        //         game.KnockedDownPins(pins);
        //     }
        //     else
        //     {
        //         Console.WriteLine("[잘못된 입력] 숫자를 입력해주세요.");
        //     }
        // }
        
        game1.KnockedDownPins(4);
        game1.KnockedDownPins(6);
        game1.KnockedDownPins(5);
        game1.KnockedDownPins(5);
        game1.KnockedDownPins(10);
        game1.KnockedDownPins(6);
    }
}
    

public class Game1
{
    private List<int> rolls = new List<int>();

    public void KnockedDownPins(int pins)
    {
        if (!IsValidRoll(pins))
        {
            Console.WriteLine($"[잘못된 입력] {pins}개 핀은 현재 상황에서 유효하지 않습니다.");
            return;
        }

        rolls.Add(pins);
        PrintOneLine();
    }

    private bool IsValidRoll(int pins)
    {
        if (pins < 0 || pins > 10)
            return false;

        int frame = 1;
        int rollIndex = 0;

        while (rollIndex < rolls.Count && frame <= 10)
        {
            if (rolls[rollIndex] == 10)
                rollIndex += 1;
            else
                rollIndex += 2;

            frame++;
        }

        // 10프레임 보너스 처리
        if (frame > 10)
        {
            int start = GetFrameStartRollIndex(10);
            int bonusCount = rolls.Count - start;

            if (bonusCount == 1)
            {
                int first = rolls[start];
                // 첫 스트라이크면 두 번째 투구는 어떤 숫자든 가능
                // 첫 스페어면 보너스는 1개만 허용
                if (first == 10)
                    return pins >= 0 && pins <= 10;
                else
                    return first + pins <= 10;
            }
            else if (bonusCount == 2)
            {
                int first = rolls[start];
                int second = rolls[start + 1];
                // 첫 스트라이크거나 스페어인 경우 보너스 투구 허용
                return (first == 10 || first + second == 10);
            }
            else
            {
                return false;
            }
        }

        var lastIdx = rolls.Count - 1;
        // 프레임 내 첫 번째 투구가 10인 경우: 두 번째 투구 없음 → OK
        if (rolls.Count > 0 && IsFirstRollOfFrameIncomplete())
        {
            int last = rolls[lastIdx];
            // 두 번째 투구인데 첫 투구와 합이 10 초과면 ❌
            return last + pins <= 10;
        }

        return true;
    }

    private bool IsFirstRollOfFrameIncomplete()
    {
        int index = 0;
        int frame = 1;

        while (index < rolls.Count && frame <= 10)
        {
            if (rolls[index] == 10)
            {
                index += 1;
            }
            else
            {
                if (index + 1 == rolls.Count)
                    return true; // 두 번째 투구 기다리는 중
                index += 2;
            }
            frame++;
        }

        return false;
    }


    private void PrintOneLine()
    {
        int rollIndex = 0;
        int score = 0;
        string topLine = "";
        string botLine = "";

        for (int frame = 1; frame <= 10; frame++)
        {
            string r1 = " ";
            string r2 = " ";
            string scoreStr = "     ";

            if (rollIndex < rolls.Count)
            {
                int first = rolls[rollIndex];

                if (first == 10)
                {
                    r1 = "X";
                    if (rollIndex + 2 < rolls.Count)
                    {
                        score += 10 + rolls[rollIndex + 1] + rolls[rollIndex + 2];
                        scoreStr = $"[{score,3}]";
                    }
                    rollIndex += 1;
                }
                else if (rollIndex + 1 < rolls.Count)
                {
                    int second = rolls[rollIndex + 1];
                    r1 = Format(first);
                    r2 = (first + second == 10) ? "/" : Format(second);

                    if (rollIndex + 2 < rolls.Count || frame == 10)
                    {
                        score += first + second;
                        if (first + second == 10 && rollIndex + 2 < rolls.Count)
                            score += rolls[rollIndex + 2];
                        scoreStr = $"[{score,3}]";
                    }

                    rollIndex += 2;
                }
                else
                {
                    r1 = Format(first);
                    rollIndex += 1;
                }
            }

            topLine += $"{frame}:[{r1},{r2}] ";
            botLine += $"  {scoreStr} ";
        }

        Console.WriteLine(topLine.TrimEnd());
        Console.WriteLine(botLine.TrimEnd());
    }

    private string Format(int pins)
    {
        return pins == 0 ? "-" : pins.ToString();
    }

    private int GetFrameStartRollIndex(int targetFrame)
    {
        int frame = 1;
        int index = 0;
        while (frame < targetFrame && index < rolls.Count)
        {
            if (rolls[index] == 10) index += 1;
            else index += 2;
            frame++;
        }
        return index;
    }
}