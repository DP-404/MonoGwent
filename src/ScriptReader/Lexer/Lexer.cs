using System;
using System.Collections.Generic;

namespace MonoGwent;

public class Lexer
{
    private string input;
    private int pos;
    private char i {get => input[pos];}

    public Lexer(string input)
    {
        this.input = input;
        pos = 0;
    }

    public List<Token> Tokenize() {
        var tokens = new List<Token>();

        while (pos < input.Length) {
            TokenType tType;
            string tVal = "";
            int tPos;

            // Check Whitespace > Ignore
            if(char.IsWhiteSpace(i)) {
                pos++;
                continue;
            }

            // Words nor Keywords can start with number
            // Check Number > Store
            else if(char.IsDigit(i)) {
                string number = "";
                while (pos < input.Length && char.IsDigit(i)) {
                    number += i;
                    pos++;
                }
                if (
                    pos < input.Length
                    && char.IsLetter(i)
                ) throw new SystemException($"Syntax error in: {pos}");
                tType = TokenType.Number;
                tVal = number;
                tPos = pos-number.Length;
            }

            // Check Word
            else if(char.IsLetter(i)) {
                string word = "";
                while (
                    pos < input.Length
                    && (char.IsLetterOrDigit(i) || i == '_')
                ) {
                    word += i;
                    pos++;
                }

                // Keyword > Store as Keyword
                if (Enum.IsDefined(typeof(Keyword), word)) {
                    tType = TokenType.Keyword;
                    tVal = word;
                    tPos = pos-word.Length;
                }
                // Word > Store as Identifier
                else {
                    tType = TokenType.Identifier;
                    tVal = word;
                    tPos = pos-word.Length;
                }
            }

            // Check Symbol
            else {
                switch (i) {
                    case '.':
                        pos++;
                        tType = TokenType.Point;
                        tPos = pos;
                        break;
                    case ':':
                        pos++;
                        tType = TokenType.Colon;
                        tPos = pos;
                        break;
                    case ';':
                        pos++;
                        tType = TokenType.Semicolon;
                        tPos = pos;
                        break;
                    case ',':
                        pos++;
                        tType = TokenType.Comma;
                        tPos = pos;
                        break;
                    case '(':
                        pos++;
                        tType = TokenType.LParen;
                        tPos = pos;
                        break;
                    case ')':
                        pos++;
                        tType = TokenType.RParen;
                        tPos = pos;
                        break;
                    case '{':
                        pos++;
                        tType = TokenType.LCBracket;
                        tPos = pos;
                        break;
                    case '}':
                        pos++;
                        tType = TokenType.RCBracket;
                        tPos = pos;
                        break;
                    case '[':
                        pos++;
                        tType = TokenType.LSBracket;
                        tPos = pos;
                        break;
                    case ']':
                        pos++;
                        tType = TokenType.RSBracket;
                        tPos = pos;
                        break;
                    case '+':
                        pos++;
                        if (i == '+') {
                            tVal += input[pos-1];
                            pos++;
                            tType = TokenType.Incr;
                        } else {
                            tType = TokenType.Plus;
                        }
                        tPos = pos;
                        break;
                    case '-':
                        pos++;
                        if (i == '-') {
                            tVal += input[pos-1];
                            pos++;
                            tType = TokenType.Decr;
                        } else {
                            tType = TokenType.Minus;
                        }
                        tPos = pos;
                        break;
                    case '*':
                        pos++;
                        tType = TokenType.Mult;
                        tPos = pos;
                        break;
                    case '/':
                        pos++;
                        tType = TokenType.Div;
                        tPos = pos;
                        break;
                    case '"':
                        pos++;
                        tType = TokenType.Quote;
                        tPos = pos;
                        break;
                    case '=':
                        pos++;
                        if (i == '=') {
                            tVal += input[pos-1];
                            pos++;
                            tType = TokenType.EQ;
                        } else {
                            tType = TokenType.Asign;
                        }
                        tPos = pos;
                        break;
                    case '!':
                        pos++;
                        if (i == '=') {
                            tVal += input[pos-1];
                            pos++;
                            tType = TokenType.NE;
                        } else {
                            throw new Exception($"Invalid token: {pos}");
                        }
                        tPos = pos;
                        break;
                    case '<':
                        pos++;
                        if (i == '=') {
                            tVal += input[pos-1];
                            pos++;
                            tType = TokenType.LE;
                        } else {
                            tType = TokenType.LT;
                        }
                        tPos = pos;
                        break;
                    case '>':
                        pos++;
                        if (i == '=') {
                            tVal += input[pos-1];
                            pos++;
                            tType = TokenType.GE;
                        } else {
                            tType = TokenType.GT;
                        }
                        tPos = pos;
                        break;
                    case '|':
                        pos++;
                        if (i == '|') {
                            tVal += input[pos-1];
                            pos++;
                            tType = TokenType.OR;
                        } else {
                            throw new Exception($"Invalid token: {pos}");
                        }
                        tPos = pos;
                        break;
                    case '&':
                        pos++;
                        if (i == '&') {
                            tVal += input[pos-1];
                            pos++;
                            tType = TokenType.AND;
                        } else {
                            throw new Exception($"Invalid token: {pos}");
                        }
                        tPos = pos;
                        break;
                    case '^':
                        pos++;
                        tType = TokenType.XOR;
                        tPos = pos;
                        break;
                    default:
                        throw new Exception($"Invalid token: {pos}");
                }
                tVal += input[pos-1];
            }

            tokens.Add(new Token(tType, tVal, tPos));
        }

        return tokens;
    }

}
