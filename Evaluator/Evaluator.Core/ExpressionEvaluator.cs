namespace Evaluator.Core;

public class ExpressionEvaluator
{
    public static double Evaluate(string infix)
    {
        var postfix = InfixToPostfix(infix);
        Console.WriteLine($"Postfix: {postfix}");
        return Calculate(postfix);
    }

    private static string InfixToPostfix(string infix)
    {
        var stack = new Stack<char>();
        var postfixBuilder = new System.Text.StringBuilder();

        for (int i = 0; i < infix.Length; i++)
        {
            char item = infix[i];

            // with decimals
            if (char.IsDigit(item) || item == '.')
            {
                while (i < infix.Length && (char.IsDigit(infix[i]) || infix[i] == '.'))
                {
                    postfixBuilder.Append(infix[i]);
                    i++;
                }
                postfixBuilder.Append(' ');
                i--; // to check the space between chars
            }
            else if (item == '-' && (i == 0 || IsOperator(infix[i - 1]) && infix[i - 1] != ')'))
            {
                postfixBuilder.Append("0 "); 
                stack.Push('-');
            }
            else if (IsOperator(item))
            {
                if (item == '(')
                {
                    stack.Push(item);
                }
                else if (item == ')')
                {
                    while (stack.Count > 0 && stack.Peek() != '(')
                    {
                        postfixBuilder.Append(stack.Pop()).Append(' ');
                    }
                    if (stack.Count > 0 && stack.Peek() == '(')
                    {
                        stack.Pop();
                    }
                }
                else
                {
                    while (stack.Count > 0 && PriorityInfix(item) <= PriorityStack(stack.Peek()))
                    {
                        postfixBuilder.Append(stack.Pop()).Append(' ');
                    }
                    stack.Push(item);
                }
            }
            else if (item == ' ')
            {
                continue;
            }
        }

        // to put the stack empty
        while (stack.Count > 0)
        {
            postfixBuilder.Append(stack.Pop()).Append(' ');
        }

        return postfixBuilder.ToString().Trim();
    }

    private static double Calculate(string postfix)
    {
        var stack = new Stack<double>();
        var tokens = postfix.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var token in tokens)
        {
            if (token.Length == 1 && IsOperator(token[0]) && token[0] != '(' && token[0] != ')')
            {
                if (stack.Count < 2)
                    throw new Exception("Expresión inválida: operandos insuficientes.");

                var op2 = stack.Pop();
                var op1 = stack.Pop();
                stack.Push(Calculate(op1, token[0], op2));
            }
            else
            {
                if (double.TryParse(token, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double number))
                {
                    stack.Push(number);
                }
                else
                {
                    throw new Exception($"Operando inválido: {token}");
                }
            }
        }

        if (stack.Count != 1)
            throw new Exception("Expresión inválida.");

        return stack.Peek();
    }

    private static double Calculate(double op1, char op, double op2) => op switch
    {
        '*' => op1 * op2,
        '/' => op2 == 0 ? throw new DivideByZeroException() : op1 / op2,
        '^' => Math.Pow(op1, op2),
        '+' => op1 + op2,
        '-' => op1 - op2,
        _ => throw new Exception("Operador inválido."),
    };

    // Helpers
    private static bool IsOperator(char c) =>
        c == '+' || c == '-' || c == '*' || c == '/' || c == '^' || c == '(' || c == ')';

    private static int PriorityInfix(char c) => c switch
    {
        '+' or '-' => 1,
        '*' or '/' => 2,
        '^' => 3,
        _ => 0
    };

    private static int PriorityStack(char c) => c switch
    {
        '+' or '-' => 1,
        '*' or '/' => 2,
        '^' => 4,   // the most important priority
        '(' => 0,
        _ => 0
    };
}
