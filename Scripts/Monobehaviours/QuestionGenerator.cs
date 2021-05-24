using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// Script for question generation and to show it on screen
/// </summary>
public class QuestionGenerator : MonoBehaviour
{
    public TextMeshProUGUI operand1Text;
    public TextMeshProUGUI operatorText;
    public TextMeshProUGUI operand2Text;
    public TextMeshProUGUI solutionText;

    public float floatThreshold = 0.0001f;

    [HideInInspector] public QuestionTypes questionType = QuestionTypes.Solution;
    [HideInInspector] public int difficultyLevel = 1;
    [HideInInspector] public float solutionNumber;
    [HideInInspector] public float operand1;
    [HideInInspector] public Operators oper;
    [HideInInspector] public float operand2;
    [HideInInspector] public float solution;

    [HideInInspector] public QuestionLevel level;

    /// <summary>
    /// Generates and shows a new question by type and difficulty level.
    /// </summary>
    /// <param name="questionType"></param>
    /// <param name="questionLevel"></param>
    public void GenerateQuestion(QuestionTypes questionType, QuestionLevel questionLevel)
    {
        this.questionType = questionType;
        level = questionLevel;

        switch (questionType)
        {
            case QuestionTypes.Solution:
                GenerateQuestionTypeSolution();
                break;

            case QuestionTypes.Operator:
                GenerateQuestionTypeOperator();
                break;

            case QuestionTypes.Operand:
                GenerateQuestionTypeOperand();
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Generates a new question of the Operand question type
    /// </summary>
    private void GenerateQuestionTypeOperand()
    {
        GenerateQuestion(out oper, out operand1, out operand2, out solution);
        int randomOperand = Random.Range(0, 2);
        switch (randomOperand)
        {
            case 0:
                solutionNumber = operand1;
                ShowQuestionText(oper, null, operand2, solution);
                break;

            case 1:
                solutionNumber = operand2;
                ShowQuestionText(oper, operand1, null, solution);
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Generates a new question of the Operator question type
    /// </summary>
    private void GenerateQuestionTypeOperator()
    {
        GenerateQuestion(out oper, out operand1, out operand2, out solution);
        ShowQuestionText(null, operand1, operand2, solution);
    }

    /// <summary>
    /// Generates a new question of the Solution question type
    /// </summary>
    private void GenerateQuestionTypeSolution()
    {
        GenerateQuestion(out oper, out operand1, out operand2, out solution);
        solutionNumber = solution;
        ShowQuestionText(oper, operand1, operand2, null);
    }

    /// <summary>
    /// Generates a random question, generating random operands and operator and calculating the solution
    /// </summary>
    /// <param name="oper"></param>
    /// <param name="operand1"></param>
    /// <param name="operand2"></param>
    /// <param name="solution"></param>
    private void GenerateQuestion(out Operators oper, out float operand1, out float operand2, out float solution)
    {
        oper = GenerateRandomOperator();

        if (oper == Operators.Divide) // operand2 must not be 0
        {
            do
            {
                operand2 = GenerateRandomNumber(level.minimumRandom, level.maximumRandom, level.numberDecimals);
            }
            while (operand2 == 0);

            solution = GenerateRandomNumber(level.minimumRandom, level.maximumRandom, level.numberDecimals);
            operand1 = (float)Math.Round(operand2 * solution, level.numberDecimals + 2);
        }
        else
        {
            operand2 = GenerateRandomNumber(level.minimumRandom, level.maximumRandom, level.numberDecimals);
            operand1 = GenerateRandomNumber(level.minimumRandom, level.maximumRandom, level.numberDecimals);
            solution = (float)Math.Round(CalculateSolution(oper, operand1, operand2), level.numberDecimals + 2);
        }
    }

    /// <summary>
    /// Calculates the solution taking the operands and operator
    /// </summary>
    /// <param name="oper"></param>
    /// <param name="operand1"></param>
    /// <param name="operand2"></param>
    /// <returns></returns>
    private float CalculateSolution(Operators oper, float operand1, float operand2)
    {
        float solution;
        switch (oper)
        {
            case Operators.Add:
                solution = operand1 + operand2;
                break;

            case Operators.Substract:
                solution = operand1 - operand2;
                break;

            case Operators.Multiply:
                solution = operand1 * operand2;
                break;

            case Operators.Divide:
                solution = operand1 / operand2;
                break;

            default:
                solution = 0;
                break;
        }

        return solution;
    }

    // TODO Consider moving it to its own class
    /// <summary>
    /// Returns the text symbol for the operator. If null returns '?'
    /// </summary>
    /// <param name="oper"></param>
    /// <returns></returns>
    private string GetOperatorText(Operators? oper)
    {
        switch (oper)
        {
            case Operators.Add:
                return "+";

            case Operators.Substract:
                return "-";

            case Operators.Multiply:
                return "x";

            case Operators.Divide:
                return "÷";

            default:
                return "?";
        }
    }

    /// <summary>
    /// update the UI components on screen with the generated question
    /// </summary>
    /// <param name="oper"></param>
    /// <param name="operand1"></param>
    /// <param name="operand2"></param>
    /// <param name="solution"></param>
    private void ShowQuestionText(Operators? oper, float? operand1, float? operand2, float? solution)
    {
        operand1Text.text = operand1 == null ? "?" : operand1.ToString();
        operand2Text.text = operand2 == null ? "?" : operand2.ToString();
        solutionText.text = solution == null ? "?" : solution.ToString();
        operatorText.text = GetOperatorText(oper);
    }

    /// <summary>
    /// Returns a random operator
    /// </summary>
    /// <returns></returns>
    public Operators GenerateRandomOperator()
    {
        return (Operators)Random.Range(0, Enum.GetNames(typeof(Operators)).Length);
    }

    /// <summary>
    /// Returns a random number with a range limit and a decimal number limit
    /// </summary>
    /// <param name="minimum"></param>
    /// <param name="maximum"></param>
    /// <param name="numberDecimals"></param>
    /// <returns></returns>
    private float GenerateRandomNumber(float minimum, float maximum, int numberDecimals = 0)
    {
        if (numberDecimals == 0) // If no decimals, then use integers
        {
            return Random.Range((int)minimum, (int)maximum + 1);
        }

        return (float)Math.Round(Random.Range(minimum, maximum), numberDecimals);
    }

    /// <summary>
    /// Returns a random number following the loaded difficulty level parameters
    /// </summary>
    /// <returns></returns>
    public float GenerateRandomNumber()
    {
        return GenerateRandomNumber(level.minimumRandom, level.maximumRandom, level.numberDecimals);
    }

    /// <summary>
    /// Check if the input is valid respecting to the question solution
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public bool ValidInputGuessOperand(float input)
    {
        if (operand1Text.text == "?")
        {
            return CalculateSolution(oper, input, operand2) == solution;
        }

        if (operand2Text.text == "?")
        {
            return CalculateSolution(oper, operand1, input) == solution;
        }

        return false;
    }

    /// <summary>
    /// Check if the input is valid respecting to the question solution
    /// </summary>
    /// <param name="oper"></param>
    /// <returns></returns>
    public bool ValidInputGuessOperator(Operators oper)
    {
        return Math.Abs(Math.Abs(solution) - Math.Abs(CalculateSolution(oper, operand1, operand2)))
            < floatThreshold;
    }

    /// <summary>
    /// Returns the solution as text, depending of the question type in game
    /// </summary>
    /// <returns></returns>
    public string GetSolutionText()
    {
        switch (questionType)
        {
            case QuestionTypes.Solution:
            case QuestionTypes.Operand:
                return solutionNumber.ToString();

            case QuestionTypes.Operator:
                return GetOperatorText(oper);

            default:
                return string.Empty;
        }
    }
}