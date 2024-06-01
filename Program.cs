using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Channels;

namespace calculator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string inString = Console.ReadLine();

            if (string.IsNullOrEmpty(inString))
                throw new Exception("Error");

            var outString = RPN(inString);
            var result = RPNReader(outString);
            Console.WriteLine(result);


            // преобразует инфиксную нотацию в постфиксную
            static List<string> RPN(string inString)
            {
                MatchCollection matches = Regex.Matches(inString, @"\d+|\(|\)|\+|\-|\*|\/|\^");
                List<string> tokens = matches.Select(x => x.ToString()).ToList();
                var outString = new List<string>();

                Dictionary<char, int> priority = new Dictionary<char, int>()
                {
                    {'^', 3 },
                    {'*', 2 },
                    {'/', 2 },
                    {'+', 1 },
                    {'-', 1 },
                };

                Stack<string> operations = new Stack<string>();

                /*
5*2/4+(2-1+2*(2142-6^(3^3)))+2

2+2*2-(1+1)
                 */
                for (int i = 0; i < tokens.Count; i++)
                {
                    // если число
                    if (double.TryParse(tokens[i], out double digit))
                    {
                        outString.Add(digit.ToString());
                    }
                    // операция
                    else if (priority.ContainsKey(char.Parse(tokens[i])))
                    {
                        while (operations.Count != 0 && operations.Peek() != "(" &&
                            (priority[char.Parse(operations.Peek())] >= priority[char.Parse(tokens[i])]))
                        {
                            outString.Add(operations.Pop());
                        }
                        operations.Push(tokens[i]);
                    }
                    // откр скобка
                    else if (char.Parse(tokens[i]) == '(')
                    {
                        operations.Push(tokens[i]);
                    }

                    // закр скобка
                    else if (char.Parse(tokens[i]) == ')')
                    {
                        while (operations.Count > 0 && operations.Peek() != "(")
                        {
                            outString.Add(operations.Pop());
                        }
                        if (operations.Peek() == "(")
                        {
                            operations.Pop();
                        }
                    }
                }

                
                Console.WriteLine("\n\n");
                while (operations.Count > 0)
                {
                    outString.Add(operations.Pop());
                }
                outString.ForEach(x => Console.Write(x + " "));

                return outString;
            }

            // калькулятор
            double RPNReader(List<string> outString)
            {
                var numbers = new Stack<double>();

                for (int i = 0; i < outString.Count; i++)
                {
                    var isNum = double.TryParse(outString[i], out double number);
                    if (isNum)
                        numbers.Push(number);
                    else
                    {
                        switch (outString[i])
                        {
                            case "^":
                                var pow = numbers.Pop();
                                numbers.Push(Math.Pow(numbers.Pop(), pow));
                                break;
                            case "*":
                                numbers.Push(numbers.Pop() * numbers.Pop());
                                break;
                            case "/":
                                double verif = numbers.Pop();
                                if (verif == 0)
                                    throw new Exception("Деление на ноль запрещено");
                                numbers.Push(numbers.Pop() / verif);

                                break;
                            case "+":
                                numbers.Push(numbers.Pop() + numbers.Pop());
                                break;
                            case "-":
                                numbers.Push(numbers.Pop() - numbers.Pop());
                                break;
                        }
                    }

                }

                return numbers.Pop();
            }

        }
    }
}
