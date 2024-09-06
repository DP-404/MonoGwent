using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoGwent;


public class Parser {
    private readonly List<Token> tokens;
    private int cur_tokenIndex;
    private Token cur_token;
    private List<Property> props;
    private Statement cur_statement = new();
    private List<Effect> effects = new();
    private List<Effect> cardEffects = new();
    private LinkedList<Variable> cur_vars = new();

    public Parser(List<Token> tokens) {
        this.tokens = tokens;
        cur_tokenIndex = 0;
        cur_token = this.tokens[cur_tokenIndex];
        props = new();
    }

    public void Parse() {
        // Check first token
        if (
            cur_token.type == TokenType.Keyword
            || cur_token.val == "card"
        ) {
            // While "effect" > Create
            while (cur_token.val == "effect") {
                Next(TokenType.Keyword);
                Next(TokenType.LCBracket);

                // Collect name
                if (cur_token.val == "Name")
                    Next(TokenType.Keyword);
                else
                    throw new Exception("Missing effect name.");
                Next(TokenType.Colon);
                dNext = Next;
                string nameEffect = ParseString();
                Next(TokenType.Comma);

                // Collect params
                LinkedList<Variable> parameters = new();
                LinkedList<Variable> variables = new();
                LinkedList<Variable> variables2 = new();

                if (cur_token.val == "Params") {
                    Next(TokenType.Keyword);
                    Next(TokenType.Colon);
                    Next(TokenType.LCBracket);

                    while (true) {
                        string name = cur_token.val;
                        Next(TokenType.Word);
                        Next(TokenType.Colon);
                        string type;
                        object value;

                        if (cur_token.val == "Number") {
                            type = cur_token.val;
                            value = 0;
                        } else if (cur_token.val == "Bool") {
                            type = cur_token.val;
                            value = false;
                        } else if(cur_token.val == "String") {
                            type = cur_token.val;
                            value = "";
                        } else throw new Exception("Parameter does not exist.");

                        Next(TokenType.Keyword);

                        Variable param = new(name, type){val = value};
                        parameters.AddLast(param);
                        variables.AddLast(param);
                        variables2.AddLast(param);

                        if (cur_token.type == TokenType.Comma)
                            Next(TokenType.Comma);
                        else
                            break;
                    }
                    Next(TokenType.RCBracket);
                    Next(TokenType.Comma);
                }

                if (parameters.Count != 0) {
                    Console.WriteLine("Effect params:");
                    foreach (var param in parameters) {
                        Console.WriteLine($"Param: {param.name}, Type: {param.type}");
                    }
                } else Console.WriteLine("No params.");

                // Effect action
                if (cur_token.val == "Action")
                    Next(TokenType.Keyword);
                else
                    throw new Exception("Invalid action declaration.");

                // Collect "targets" and "context"
                Next(TokenType.Colon);
                Next(TokenType.LParen);
                string targets = cur_token.val;
                Next(TokenType.Word);
                Next(TokenType.Comma);
                string context = cur_token.val;
                Next(TokenType.Word);
                Next(TokenType.RParen);
                Next(TokenType.Asign);
                Next(TokenType.GT);
                Next(TokenType.LCBracket);

                // Collect statements
                List<Statement> statements = new();

                // Add vars
                variables.AddLast(new Variable(targets, "List<Card>"));
                variables.AddLast(new Variable(context, "Context")); 

                variables2.AddLast(new Variable(targets, "List<Card>"));
                variables2.AddLast(new Variable(context, "Context")); 

                int count = 0;

                ActionRecolector();

                foreach (var statement in statements)
                    statement.Debug();

                // Create Effect
                if (parameters != null)
                    effects.Add(new Effect(nameEffect, targets, statements, variables2, parameters));
                else
                    effects.Add(new Effect(nameEffect, targets, statements, variables2));

                cur_vars.Clear();

                Console.WriteLine("Effect created.");

                // Collect vars and method calls
                void ActionRecolector() {
                    StatementsCollector(false);

                    Next(TokenType.RCBracket);
                    Next(TokenType.RCBracket);

                    Console.WriteLine("Action collected.");
                }

                void StatementsCollector(bool OneStatement) {
                    // Add statements until }
                    if (cur_token.type != TokenType.RCBracket) {
                        // Create var
                        if (Enum.IsDefined(typeof(Keyword), cur_token.val))
                            throw new Exception($"Cannot use [{cur_token.val}].");
                        else if (cur_token.val == "for") {
                            // Add statement and change current
                            statements.Add(new Statement());
                            cur_statement = statements.Last();

                            // Save outside loop vars
                            List<Variable> variablesFor = [.. variables];

                            NextAndSave(TokenType.Word);

                            // Save new var
                            variables.AddLast(new Variable(cur_token.val, "Card"));
                            variables2.AddLast(new Variable(cur_token.val, "Card"));
                            NextAndSave(TokenType.Word);

                            if (cur_token.val != "in")
                                throw new Exception("Expected [in] keyword.");
                            Next(TokenType.Word);

                            // Save list
                            if (!IsCardList())
                                throw new Exception("Invalid list type. Expected [List<Card>].");
                            NextAndSave(TokenType.Word);

                            bool isOneStatement;

                            if (cur_token.type == TokenType.LCBracket) {
                                Next(TokenType.LCBracket);
                                isOneStatement = false;
                            } else {
                                isOneStatement = true;
                            }

                            // Collect statements inside loop
                            StatementsCollector(isOneStatement);

                            // Delete loop vars
                            variables.Clear();

                            foreach (var variable in variablesFor) {
                                variables.AddLast(variable);
                            }

                            if (!isOneStatement) {
                                Next(TokenType.RCBracket);
                                Next(TokenType.Semicolon);
                            }

                            // End loop
                            statements.Add(new Statement());
                            cur_statement = statements.Last();
                            cur_statement.Add("ForEnd");

                            Console.WriteLine($"Collected statement {++count}: for");
                            if(!OneStatement) StatementsCollector(false);

                        } else if(cur_token.val == "while") {
                            // Add statement and change current
                            statements.Add(new Statement());
                            cur_statement = statements.Last();

                            // Save vars outside loop
                            List<Variable> variablesWhile = [.. variables];

                            NextAndSave(TokenType.Word);
                            Next(TokenType.LParen);

                            dNext = NextAndSave;
                            ParseBooleanExpression();

                            Next(TokenType.RParen);

                            bool isOneStatement;

                            if (cur_token.type == TokenType.LCBracket) {
                                Next(TokenType.LCBracket);
                                isOneStatement = false;
                            } else {
                                isOneStatement = true;
                            }

                            // Collect statements inside loop
                            StatementsCollector(isOneStatement);

                            if (!isOneStatement) {
                                Next(TokenType.RCBracket);
                                Next(TokenType.Semicolon);
                            }

                            // Delete loop vars
                            variables.Clear();

                            foreach (var variable in variablesWhile) {
                                variables.AddLast(variable);
                            }

                            // End loop
                            statements.Add(new Statement());
                            cur_statement = statements.Last();
                            cur_statement.Add("WhileEnd");

                            Console.WriteLine($"Collected statement {++count}: while");
                            if(!OneStatement) StatementsCollector(false);

                        } else if(IsNumericVariable() || IsNumericProperty()) { // Incr or CombOp
                            // Add new statement and change current
                            statements.Add(new Statement());
                            cur_statement = statements.Last();

                            if(IsNumericProperty()) {
                                NextAndSave(TokenType.Word);
                                Next(TokenType.Point);
                                NextAndSave(TokenType.Keyword);
                            } else if (cur_token.type == TokenType.Word) {
                                NextAndSave(TokenType.Word);
                            }

                            if (cur_token.type == TokenType.SumCom) {
                                NextAndSave(TokenType.SumCom);
                                dNext = NextAndSave;
                                Expr();
                            } else if(cur_token.type == TokenType.SubCom) {
                                NextAndSave(TokenType.SubCom);
                                dNext = NextAndSave;
                                Expr();
                            } else if(cur_token.type == TokenType.MultCom) {
                                NextAndSave(TokenType.MultCom);
                                dNext = NextAndSave;
                                Expr();
                            } else if(cur_token.type == TokenType.DivCom) {
                                NextAndSave(TokenType.DivCom);
                                dNext = NextAndSave;
                                Expr();
                            } else if(cur_token.type == TokenType.XORCom) {
                                NextAndSave(TokenType.XORCom);
                                dNext = NextAndSave;
                                Expr();
                            } else if (cur_token.type == TokenType.Incr) {
                                NextAndSave(TokenType.Incr);
                            }

                            // Statement end > Collect next
                            if (cur_token.type == TokenType.Semicolon) {
                                Next(TokenType.Semicolon);
                                Console.WriteLine($"Collected statement {++count}: Incr or CombOp");
                                if(!OneStatement) StatementsCollector(false);
                            } else throw new Exception("Expected semicolon.");

                        } else if (cur_token.type == TokenType.Incr) { // Increase
                            // Add new statement and change current
                            statements.Add(new Statement());
                            cur_statement = statements.Last();

                            NextAndSave(TokenType.Incr);

                            if (IsNumericProperty()) {
                                NextAndSave(TokenType.Word);
                                Next(TokenType.Point);
                                NextAndSave(TokenType.Keyword);
                            } else if (cur_token.type == TokenType.Word) {
                                NextAndSave(TokenType.Word);
                            }

                            // Statement end > Collect next
                            if (cur_token.type == TokenType.Semicolon) {
                                Next(TokenType.Semicolon);
                                Console.WriteLine($"Collected statement {++count}: Incr");
                                if(!OneStatement) StatementsCollector(false);
                            } else throw new Exception("Semicolon was expected");

                        } else if(!IsVariable() && cur_token.type == TokenType.Word) { // Var declaration
                            // Add statement and change current
                            statements.Add(new Statement());
                            cur_statement = statements.Last();

                            string name = cur_token.val;
                            NextAndSave(TokenType.Word);
                            NextAndSave(TokenType.Asign);

                            if (IsVariable()) {
                                cur_vars = variables;

                                Variable variable = new(name, WichTypeIs(TokenType.Semicolon));

                                variables.AddLast(variable);
                                variables2.AddLast(variable);
                            } else if (IsBoolean()) {
                                cur_vars = variables;

                                dNext = NextAndSave;
                                Variable boolean = new(name, "Bool") {val = ParseBooleanExpression()};

                                variables.AddLast(boolean);
                                variables2.AddLast(boolean);
                            } else if (
                                cur_token.type == TokenType.Quote
                                || IsString()
                            ) {
                                cur_vars = variables;

                                dNext = NextAndSave;
                                Variable variable = new(name, "String") {val = ParseString()};

                                variables.AddLast(variable);
                                variables2.AddLast(variable);
                            } else {
                                cur_vars = variables;

                                dNext = NextAndSave;
                                Variable number = new(name, "Number") {val = Expr()};
                                
                                variables.AddLast(number);
                                variables2.AddLast(number);
                            }

                            // Statement end > Collect next
                            if (cur_token.type == TokenType.Semicolon) {
                                Next(TokenType.Semicolon);
                                Console.WriteLine($"Collected statement {++count}: Var");
                                if (!OneStatement) StatementsCollector(false);
                            }
                            else throw new Exception("Expected semicolon.");

                        } else if (IsVariable()) { // Var > Await method call
                            // Add statement and change current
                            statements.Add(new Statement());
                            cur_statement = statements.Last();

                            // Collect statement
                            if (!CheckMethodCall()) throw new Exception("Bad method call");

                            // Statement end > Collect next
                            if (cur_token.type == TokenType.Semicolon) {
                                Next(TokenType.Semicolon);
                                Console.WriteLine($"Collected statement {++count}: Method");
                                if (!OneStatement) StatementsCollector(false);
                            } else throw new Exception("Expected semicolon.");
                        } else throw new Exception("Expected statement.");
                    }
                }

                // Change to next token and save
                void NextAndSave(TokenType type) {
                    cur_statement.Add(cur_token.val);
                    Next(type);
                }

                bool IsVariable() {
                    if (Enum.IsDefined(typeof(Keyword), cur_token.val)) {
                        return false;
                    } else {
                        foreach (var variable in variables) {
                            if (
                                variable.name == cur_token.val
                                && (
                                    (
                                        variable.type != "Number" 
                                        && variable.type != "Bool"
                                        && variable.type != "String"
                                        && variable.type != "Card"
                                    ) || (
                                        variable.type == "Card"
                                        && tokens[cur_tokenIndex + 2].val == "Owner"
                                    )
                                )
                            ) return true;
                        }
                        return false;
                    }
                }

                bool IsNumericVariable() {
                    if (Enum.IsDefined(typeof(Keyword), cur_token.val)) {
                        return false;
                    } else {
                        foreach (var variable in variables) {
                            if (
                                variable.name == cur_token.val
                                && variable.type == "Number"
                            ) return true;
                        }
                        return false;
                    }
                }

                bool IsNumericProperty() {
                    if (Enum.IsDefined(typeof(Keyword), cur_token.val)) {
                        return false;
                    } else {
                        foreach (var variable in variables) {
                            if (
                                variable.name == cur_token.val
                                && variable.type == "Card" 
                                && tokens[cur_tokenIndex + 2].val == "Power"
                            ) return true;
                        }
                        return false;
                    }
                }

                bool IsString() {
                    if (Enum.IsDefined(typeof(Keyword), cur_token.val)) {
                        return false;
                    } else {
                        foreach (var variable in variables) {
                            if (
                                variable.name == cur_token.val
                                && variable.type == "String"
                            ) {
                                return true;
                            } else if (
                                variable.name == cur_token.val
                                && variable.type == "Card" 
                                && (
                                    tokens[cur_tokenIndex + 2].val == "Name"
                                    || tokens[cur_tokenIndex + 2].val == "Type"
                                    || tokens[cur_tokenIndex + 2].val == "Range"
                                    || tokens[cur_tokenIndex + 2].val == "Faction"
                                )
                            ) return true;
                        }
                        return false;
                    }
                }

                bool IsCardList() {
                    foreach (var variable in variables) {
                        if (
                            variable.name == cur_token.val
                            && variable.type == "List<Card>"
                        ) return true;
                    }
                    return false;
                }

                // Collect method call statement
                bool CheckMethodCall() {
                    // Check if var
                    if (IsVariable()) {
                        foreach (var variable in variables) {
                            if (variable.name == cur_token.val) {
                                if (variable.type == "Context") {
                                    NextAndSave(TokenType.Word);
                                    if (cur_token.type == TokenType.Point)
                                        Next(TokenType.Point);
                                    else
                                        return false;
                                    if (Enum.IsDefined(typeof(ContextMethods), cur_token.val))
                                        return CheckMethodCall();
                                    else
                                        return false;
                                } else if(variable.type == "List<Card>") {
                                    NextAndSave(TokenType.Word);
                                    if (cur_token.type == TokenType.Point)
                                        Next(TokenType.Point);
                                    else
                                        return false;
                                    if (Enum.IsDefined(typeof(CardListMethods), cur_token.val))
                                        return CheckMethodCall();
                                    else
                                        return false;
                                }
                                return false;
                            }
                        }
                        return false;

                    // Check if method
                    } else if (Enum.IsDefined(typeof(ContextMethods), cur_token.val)) {
                        if (Enum.IsDefined(typeof(SyntacticSugarContext), cur_token.val)) {
                            NextAndSave(TokenType.Keyword);
                            Next(TokenType.Point);
                            if (Enum.IsDefined(typeof(CardListMethods), cur_token.val))
                                return CheckMethodCall();
                            else
                                return false;
                        } else {
                            NextAndSave(TokenType.Keyword);
                            Next(TokenType.LParen);

                            bool correct = false;

                            foreach (var variable in variables) {
                                if (
                                    variable.name == cur_token.val
                                    && WichTypeIs(TokenType.RParen) == "Player"
                                ) correct = true;
                            }

                            if (correct)
                                Next(TokenType.RParen);
                            else
                                return false;

                            Next(TokenType.Point);

                            if (Enum.IsDefined(typeof(CardListMethods), cur_token.val))
                                return CheckMethodCall();
                            else
                                return false;
                        }
                    } else if (Enum.IsDefined(typeof(CardListMethods), cur_token.val)) {
                        if (cur_token.val == "Shuffle") {
                            NextAndSave(TokenType.Keyword);
                            Next(TokenType.LParen);
                            Next(TokenType.RParen);
                            if (cur_token.type == TokenType.Semicolon)
                                return true;
                            else
                                return false;
                        } else if (
                            cur_token.val == "Add"
                            || cur_token.val == "Push"
                            || cur_token.val == "SendBottom"
                            || cur_token.val == "Remove"
                        ) {
                            NextAndSave(TokenType.Keyword);
                            Next(TokenType.LParen);

                            if (WichTypeIs(TokenType.RParen) == "Card") {
                                Next(TokenType.RParen);
                                if (cur_token.type == TokenType.Semicolon)
                                    return true;
                                else
                                    return false;
                            }
                            return false;  
                        } else if(cur_token.val == "Find") {
                            NextAndSave(TokenType.Keyword);
                            Next(TokenType.LParen);
                            dNext = NextAndSave;
                            Predicate(variables2);
                            Next(TokenType.RParen);

                            Next(TokenType.Point);

                            if (Enum.IsDefined(typeof(CardListMethods), cur_token.val))
                                return CheckMethodCall();
                            else
                                return false;
                        }
                        throw new Exception("Invalid method.");
                    } else return false;
                }

                // Get var type
                string WichTypeIs(TokenType finalToken) {
                    // Check if var
                    if (cur_token.type == TokenType.Word) {
                        foreach (var variable in variables) {
                            if (variable.name == cur_token.val) {
                                // Check var type
                                if (variable.type == "Context") {
                                    NextAndSave(TokenType.Word);
                                    if (cur_token.type == finalToken)
                                        return "Context";
                                    else if (cur_token.type == TokenType.Point) {
                                        Next(TokenType.Point);
                                        if (Enum.IsDefined(typeof(ContextPropertiesAndMethods), cur_token.val))
                                            return WichTypeIs(finalToken);
                                        throw new Exception("Unknown method or property.");
                                    } 
                                } else if (variable.type == "List<Card>") {
                                    NextAndSave(TokenType.Word);
                                    if (cur_token.type == finalToken)
                                        return "List<Card>";
                                    else if (cur_token.type == TokenType.Point) {
                                        Next(TokenType.Point);
                                        if (Enum.IsDefined(typeof(CardListProperties), cur_token.val))
                                            return WichTypeIs(finalToken);
                                        else
                                            throw new Exception("Unknown method or property.");
                                    } else if (cur_token.type == TokenType.LSBracket) {
                                        NextAndSave(TokenType.LSBracket);
                                        dNext = NextAndSave;
                                        Expr();
                                        NextAndSave(TokenType.RSBracket);
                                        if (cur_token.type == finalToken)
                                            return "Card";
                                        else if (cur_token.type == TokenType.Point) {
                                            Next(TokenType.Point);
                                            if (Enum.IsDefined(typeof(CardProperties), cur_token.val))
                                                return WichTypeIs(finalToken);
                                            throw new Exception("Unknown method or property.");
                                        }
                                        throw new Exception("Variable declaration error.");
                                    }
                                } else if (variable.type == "Card") {
                                    NextAndSave(TokenType.Word);
                                    if (cur_token.type == finalToken)
                                        return "Card";
                                    else if (cur_token.type == TokenType.Point) {
                                        Next(TokenType.Point);
                                        if(Enum.IsDefined(typeof(CardProperties), cur_token.val))
                                            return WichTypeIs(finalToken);
                                        throw new Exception("Unknown method or property.");
                                    } 
                                } else if(variable.type == "Player") {
                                    NextAndSave(TokenType.Word);
                                    if (cur_token.type == finalToken)
                                        return "Player";
                                    throw new Exception("Unknown method or property."); 
                                }
                            } 
                        }
                        throw new Exception("Expected variable.");
                    } else if(cur_token.type == TokenType.Keyword) {
                        // Check if method
                        if (Enum.IsDefined(typeof(ContextPropertiesAndMethods), cur_token.val)) {
                            if (cur_token.val == "TriggerPlayer") {
                                NextAndSave(TokenType.Keyword);
                                return "Player";
                            } else if (Enum.IsDefined(typeof(SyntacticSugarContext), cur_token.val)) {
                                NextAndSave(TokenType.Keyword);
                                if (cur_token.type == finalToken)
                                    return "List<Card>";
                                else if (cur_token.type == TokenType.Point) {
                                    Next(TokenType.Point);
                                    return WichTypeIs(finalToken);
                                } else if(cur_token.type == TokenType.LSBracket) {
                                    NextAndSave(TokenType.LSBracket);
                                    dNext = NextAndSave;
                                    Expr();
                                    NextAndSave(TokenType.RSBracket);
                                    if (cur_token.type == finalToken)
                                        return "Card";
                                    else if (cur_token.type == TokenType.Point) {
                                        Next(TokenType.Point);
                                        if (Enum.IsDefined(typeof(CardProperties), cur_token.val))
                                            return WichTypeIs(finalToken);
                                        throw new Exception("Unknown method or property.");
                                    }
                                    throw new Exception("Variable declaration error.");
                                }
                                throw new Exception("Expected semicolon or method call.");
                            } else {
                                NextAndSave(TokenType.Keyword);
                                Next(TokenType.LParen);

                                if (WichTypeIs(TokenType.RParen) == "Player")
                                    Next(TokenType.RParen);
                                else throw new Exception("Expected Player parameter.");

                                if (cur_token.type == finalToken)
                                    return "List<Card>";
                                else if (cur_token.type == TokenType.Point) {
                                    Next(TokenType.Point);
                                    if (Enum.IsDefined(typeof(CardListProperties), cur_token.val))
                                        return WichTypeIs(finalToken);
                                    throw new Exception("Unknown method or property.");
                                } else if (cur_token.type == TokenType.LSBracket) {
                                    NextAndSave(TokenType.LSBracket);
                                    dNext = NextAndSave;
                                    Expr();
                                    NextAndSave(TokenType.RSBracket);
                                    if (cur_token.type == finalToken)
                                        return "Card";
                                    else if (cur_token.type == TokenType.Point) {
                                        Next(TokenType.Point);
                                        if (Enum.IsDefined(typeof(CardProperties), cur_token.val))
                                            return WichTypeIs(finalToken);
                                        throw new Exception("Unknown method or property.");
                                    }
                                    throw new Exception("Variable declaration error.");
                                }
                                throw new Exception("Expected semicolon or method call.");
                            }
                        } else if (Enum.IsDefined(typeof(CardProperties), cur_token.val)) {
                            Token token = cur_token;
                            NextAndSave(TokenType.Keyword);
                            if (token.val == "Owner")
                                return "Player";
                            throw new Exception("Invalid Card property.");
                        } else if (Enum.IsDefined(typeof(CardListProperties), cur_token.val)) {
                            if (cur_token.val == "Pop") {
                                NextAndSave(TokenType.Keyword);
                                Next(TokenType.LParen);
                                Next(TokenType.RParen);
                                if (cur_token.type == finalToken)
                                    return "Card";
                                else if (cur_token.type == TokenType.Point) {
                                    Next(TokenType.Point);

                                    if (Enum.IsDefined(typeof(CardProperties), cur_token.val))
                                        return WichTypeIs(finalToken);
                                    throw new Exception("Expected method return type not null.");
                                }
                                throw new Exception("Expected semicolon or method call.");
                            } else if (cur_token.val == "Find") {
                                NextAndSave(TokenType.Keyword);
                                Next(TokenType.LParen);
                                dNext = NextAndSave;
                                Predicate(variables2);
                                Next(TokenType.RParen);
                                if (cur_token.type == finalToken)
                                    return "List<Card>";
                                else if (cur_token.type == TokenType.Point) {
                                    Next(TokenType.Point);

                                    if (Enum.IsDefined(typeof(CardListProperties), cur_token.val))
                                        return WichTypeIs(finalToken);
                                    throw new Exception("Expected method return type not null.");
                                } else if(cur_token.type == TokenType.LSBracket) {
                                    NextAndSave(TokenType.LSBracket);
                                    dNext = NextAndSave;
                                    Expr();
                                    NextAndSave(TokenType.RSBracket);
                                    if (cur_token.type == finalToken)
                                        return "Card";
                                    else if (cur_token.type == TokenType.Point) {
                                        Next(TokenType.Point);
                                        if (Enum.IsDefined(typeof(CardProperties), cur_token.val))
                                            return WichTypeIs(finalToken);
                                        throw new Exception("Uknown method or property.");
                                    }
                                    throw new Exception("Variable declaration error.");
                                }
                                throw new Exception("Expected semicolon or method call.");
                            } 
                            throw new Exception("Expected method return type not null.");
                        }
                        throw new Exception("Expected variable or method.");
                    }
                    throw new Exception("Expected variable or method.");
                }
            }
            // Expect "card"
            if (
                cur_token.val != "card"
                && cur_token.type != TokenType.End
            )
                throw new Exception($"Expected card declaration after effects declarations.");

            while (cur_token.val == "card") {
                // Collect properties
                Next(TokenType.Keyword);
                Next(TokenType.LCBracket);
                Properties(); 

                // Effect found
                if (cur_token.type == TokenType.Comma) {
                    Next(TokenType.Comma);
                    if (cur_token.val == "OnActivation")
                        Next(TokenType.Keyword);
                    else
                        throw new Exception("OnActivation keyword expected");
                    Next(TokenType.Colon);
                    Next(TokenType.LSBracket);
                    CardEffect();
                    Next(TokenType.RSBracket);  
                }

                Next(TokenType.RCBracket);

                // Create card blueprint
                CardMaker cardCreator = new();
                if (
                    !IsCardWithEffectCorrect()
                    && cardEffects.Count == 0
                )
                    cardCreator.Create(props, new List<Effect>());
                else if (IsCardWithEffectCorrect())
                    cardCreator.Create(props, cardEffects);
                else
                    throw new Exception("This card cannot have effects");

                Console.WriteLine("Card created with properties:");
                foreach (var property in props)
                    Console.WriteLine($"Prop: {property.type}, Val: {property.value}");

                props.Clear();
                cardEffects.Clear();
            }
            if (cur_token.type != TokenType.End)
                throw new Exception($"Syntax error in: {cur_token.pos}");  
        }
        else throw new Exception($"Syntax error in: {cur_token.pos}");
    }

    // Change to next token
    private void Next(TokenType tokenType) {
        if (cur_token.type == tokenType) {
            cur_tokenIndex++;
            cur_token = cur_tokenIndex < tokens.Count ? tokens[cur_tokenIndex] : new Token(TokenType.End, "null", 0);
        } else throw new SystemException($"Unexpected token: {cur_token.type}, was expected {tokenType}");
    }

    // Process numbers
    private int Factor() {
        Token token = cur_token;

        // Correct > Next and return value
        if (token.type ==  TokenType.Number) {
            int result = int.Parse(token.val);

            dNext(TokenType.Number);

            if (cur_token.type == TokenType.XOR) {
                dNext(TokenType.XOR);
                result ^= Factor(); 
            }

            token = cur_token;

            while (token.type == TokenType.LParen) {
                dNext(TokenType.LParen);
                result *= Expr();
                dNext(TokenType.RParen);

                token = cur_token;
            }
            return result;
        } else if (token.type == TokenType.LParen) {
            dNext(TokenType.LParen);
            int result = Expr();
            dNext(TokenType.RParen);

            if (cur_token.type == TokenType.XOR) {
                dNext(TokenType.XOR);
                result ^= Factor(); 
            }

            token = cur_token;

            while (token.type == TokenType.LParen) {
                dNext(TokenType.LParen);
                result *= Expr();
                dNext(TokenType.RParen);

                token = cur_token;
            }
            return result;
        } else if (cur_token.type == TokenType.Word) {
            foreach (var variable in cur_vars) {
                if (
                    variable.name == cur_token.val
                    && variable.val is int value
                ) {
                    int result = value;

                    dNext(TokenType.Word);

                    if (cur_token.type == TokenType.Incr)
                        dNext(TokenType.Incr);

                    if (cur_token.type == TokenType.XOR) {
                        dNext(TokenType.XOR);
                        result ^= Factor(); 
                    }

                    token = cur_token;

                    while (token.type == TokenType.LParen) {
                        dNext(TokenType.LParen);
                        result *= Expr();
                        dNext(TokenType.RParen);

                        token = cur_token;
                    }
                    return result;
                } else if (
                    variable.name == cur_token.val
                    && variable.type == "Card"
                    && tokens[cur_tokenIndex + 2].val == "Power"
                ) {
                    dNext(TokenType.Word);
                    Next(TokenType.Point);
                    dNext(TokenType.Keyword);

                    int result = 0;

                    if (cur_token.type == TokenType.Incr)
                        throw new Exception("Cannot increase property.");

                    if (cur_token.type == TokenType.XOR) {
                        dNext(TokenType.XOR);
                        result ^= Factor(); 
                    }

                    token = cur_token;

                    while (token.type == TokenType.LParen) {
                        dNext(TokenType.LParen);
                        result *= Expr();
                        dNext(TokenType.RParen);

                        token = cur_token;
                    }
                    return result;
                }  
            }
            throw new Exception("Invalid variable type. Expected: [int].");
        } else if (cur_token.type == TokenType.Incr) {
            dNext(TokenType.Incr);

            foreach (var variable in cur_vars) {
                if (
                    variable.name == cur_token.val
                    && variable.val is int value
                ) {
                    int result = value;

                    dNext(TokenType.Word);

                    if (cur_token.type == TokenType.XOR) {
                        dNext(TokenType.XOR);
                        result ^= Factor(); 
                    }

                    token = cur_token;

                    while (token.type == TokenType.LParen) {
                        dNext(TokenType.LParen);
                        result *= Expr();
                        dNext(TokenType.RParen);

                        token = cur_token;
                    }
                    return result;
                }
                else if (
                    variable.name == cur_token.val
                    && variable.type == "Card"
                    && tokens[cur_tokenIndex + 2].val == "Power"
                )
                    throw new Exception("Cannot increase property.");
            }
            throw new Exception("Invalid variable type. Expected: [int].");
        }
        throw new Exception($"Unexpected token: {token.type}");
    }

    // Process mult and div
    private int Term() {
        int result = Factor();

        // Mult or Div > Next and perform operation
        while (
            cur_token.type == TokenType.Mult
            || cur_token.type == TokenType.Div
        ) {
            Token token = cur_token;
            if (token.type == TokenType.Mult) {
                dNext(TokenType.Mult);
                result *= Factor();
            } else if (token.type == TokenType.Div) {
                dNext(TokenType.Div);
                result /= Factor();
            }
        }
        return result;
    }

    // Process Sum and Sub
    private int Expr() {
        int result = Term();

        // Sum or Sub > Next and perform operation
        while (
            cur_token.type == TokenType.Plus
            || cur_token.type == TokenType.Minus
        ) {
            Token token = cur_token;
            if (token.type == TokenType.Plus) {
                dNext(TokenType.Plus);
                result += Term();
            } else if (token.type == TokenType.Minus) {
                dNext(TokenType.Minus);
                result -= Term();
            }
        }
        return result;
    }

    // Collect properties
    private void Properties() {
        int count = 0;
        bool isSpecialCard = false;

        // Save property type and value
        while (
            cur_token.val == "Type"
            || cur_token.val == "Name"
            || cur_token.val == "Faction"
            || cur_token.val == "Power"
            || cur_token.val == "Range"
        ) {
            Token token = cur_token;

            Next(TokenType.Keyword);
            Next(TokenType.Colon);
            dNext = Next;
            if (token.val == "Power")
                ToProperty(token.val);
            else if (token.val == "Range") {
                Next(TokenType.LSBracket);
                ToProperty(token.val, ParseString());

                while (cur_token.type == TokenType.Comma) {
                    Next(TokenType.Comma);
                    ToProperty(token.val, ParseString());
                }

                Next(TokenType.RSBracket);
            } else {   
                string type = ParseString();

                if (
                    token.val == "Type"
                    && type != "Gold"
                    && type != "Silver"
                ) {
                    isSpecialCard = true;
                } 
                ToProperty(token.val, type);
            } 

            count++;

            if (
                (
                    count < 3
                    && isSpecialCard
                ) || (
                    count < 5
                    && !isSpecialCard
                )
            ) Next(TokenType.Comma);
        }

        // Verify properties
        CheckProperties();
    }

    // Add card property to list
    private void ToProperty(string type, string value) {
        Property property = new(type, value);
        props.Add(property);
    }

    // Overload for power
    private void ToProperty(string type) {
        dNext = Next;
        int value = Expr();

        Property property = new(type, value);
        props.Add(property);
    }

    // Validate card properties
    public void CheckProperties() {
        bool isSpecialCard = false;

        bool hasType = false;
        bool hasName = false;
        bool hasFaction = false;

        bool hasPower = false;
        bool hasRange = false;

        //Comprueba si tiene o no las propiedades necesarias
        foreach (var property in props) {
            if (property.type == "Type") {
                var val = (string)property.value;
                if (
                    val == "Dispel"
                    || val == "Weather"
                    || val == "Leader"
                    || val == "Decoy"
                    || val == "Boost"
                )
                    isSpecialCard = true;
                else if (
                    val != "Gold"
                    && val != "Silver"
                ) throw new Exception($"Card type mismatch. Got: {property.value}");
                hasType = true;
            }
            else if (property.type == "Name") hasName = true;
            else if (property.type == "Faction") hasFaction = true;
            else if (property.type == "Power") {
                if ((int)property.value < 0)
                    throw new Exception("Power cannot be negative");
                else
                    hasPower = true;
            } else if (property.type == "Range") hasRange = true;
        }

        // Mismatch > Exception
        if ((
            isSpecialCard
            && (
                !hasType
                || !hasFaction
                || !hasName
                || hasPower
                || hasRange
            ))
            ||
            (
            !isSpecialCard
            && (
                !hasType
                || !hasFaction
                || !hasName
                || !hasPower
                || !hasRange
            )
        ))
            throw new Exception("Missing card property/ies.");
    }

    // Collect card effects
    private void CardEffect() {
        Next(TokenType.LCBracket);
        if (cur_token.val == "Effect")
            Next(TokenType.Keyword);
        else
            throw new Exception("Effect keyword expected");
        Next(TokenType.Colon);

        Effect effect;

        bool isSyntacticSugarOn = false;

        if (cur_token.type == TokenType.Quote) {
            effect = FindEffect(ParseString());

            isSyntacticSugarOn = true;
        } else if (cur_token.type == TokenType.LCBracket) {
            Next(TokenType.LCBracket);
            if (cur_token.val == "Name")
                Next(TokenType.Keyword);
            else
                throw new Exception("Expected effect name.");
            Next(TokenType.Colon);

            effect = FindEffect(ParseString());
        } else throw new Exception("Expected effect name.");

        // Check parms and add values
        if (effect.parms.Count != 0) {
            if (isSyntacticSugarOn)
                Next(TokenType.LCBracket);
            else
                Next(TokenType.Comma);

            int count = 0;

            SetParams(effect, ref count);

            if (count != effect.parms.Count)
                throw new Exception("Missing parameters.");

            Next(TokenType.RCBracket);
        }
        else if (!isSyntacticSugarOn)
            Next(TokenType.RCBracket);

        // Check Selector
        if (cur_token.type == TokenType.Comma) {
            Next(TokenType.Comma);

            if (cur_token.val == "Selector") {
                Next(TokenType.Keyword);
                Selector(null, effect);
            } 
            // Void Source for empty targets
            else effect.sourceTargets = "empty"; 
        }

        // Check PostAction
        if (cur_token.type == TokenType.Comma) {
            Next(TokenType.Comma);
            PostAction(effect);
        } else if (
            cur_token.val == Keyword.PostAction.ToString()
            && effect.sourceTargets == "empty"
        )
            PostAction(effect);
        else
            effect.sourceTargets ??= "empty";

        Next(TokenType.RCBracket);

        Console.WriteLine($"Params added to effect: \"{effect.name}\"");

        // Save effect
        cardEffects.Add(effect);

        // Another Effect > Collect
        if (cur_token.type == TokenType.Comma) {
            Next(TokenType.Comma);
            CardEffect();
        } 
    }

    private Effect FindEffect(string name) {
        foreach (var effect in effects) {
            if (effect.name == name)
                return (Effect)effect.Clone();
        }

        throw new Exception("Fffect does not exist.");
    }

    private void SetParams(Effect effect, ref int count) {
        bool isCorrectParam = false;

        foreach (var param in effect.parms) {
            if (param.name == cur_token.val) { 
                foreach (var variable in effect.vars) {
                    if (variable.name == param.name) {
                        Next(TokenType.Word);
                        Next(TokenType.Colon);
                        if (param.type == "Number")
                            variable.val = Expr();
                        else if (param.type == "Bool") {
                            dNext = Next;
                            variable.val = ParseBooleanExpression();
                        } else if(param.type == "String") {                     
                            variable.val = ParseString();
                        } else throw new Exception("Invalid parameter type.");

                        count++;

                        if (cur_token.type == TokenType.Comma) {
                            Next(TokenType.Comma);
                            SetParams(effect, ref count);
                        }

                        isCorrectParam = true;
                        break;
                    }
                }   
            }
        }

        if (!isCorrectParam)
            throw new Exception("Invalid or missing parameter. Perhaps you put an tailing comma?");
    }

    public void Selector(Effect effectParent, Effect effect) {
        Next(TokenType.Colon);
        Next(TokenType.LCBracket);

        //Source
        if (cur_token.val == "Source")
            Next(TokenType.Keyword);
        else
            throw new Exception("Source expected");

        Next(TokenType.Colon);

        string source = ParseString();

        if (
            Enum.IsDefined(typeof(Source), source)
            || source != "parent"
        )
            effect.sourceTargets = source;
        else if (
            source == "parent"
            && effectParent != null
        )
            effect.sourceTargets = effectParent.sourceTargets;
        else
            throw new Exception("Invalid card filter");

        Next(TokenType.Comma);

        //Single
        if (cur_token.val == "Single")
            Next(TokenType.Keyword);
        else
            throw new Exception("Single expected");

        Next(TokenType.Colon);

        if (cur_token.val == "false") {
            effect.single = false;
            Next(TokenType.Word);
        } else if(cur_token.val == "true") {
            effect.single = true;
            Next(TokenType.Word);
        } else throw new Exception("Expected [true] or [false].");

        Next(TokenType.Comma);

        //Predicate
        if (cur_token.val == "Predicate") {
            Next(TokenType.Keyword);
            Next(TokenType.Colon);

            dNext = NextAndSavePredicate;
            Predicate(effect.vars);
            
            cur_vars.Clear();
        } else throw new Exception("Expected predicate.");

        Next(TokenType.RCBracket);

        // Next Token and save as Predicate
        void NextAndSavePredicate(TokenType type) {
            effect.predicate.Add(cur_token.val);
            Next(type);
        }
    }

    private void Predicate(LinkedList<Variable> variables) {
        Next(TokenType.LParen);

        foreach (var variable in cur_vars)
            if (variable.name == cur_token.val) 
                throw new Exception($"Variable already used: {cur_token.val}");

        if (cur_token.type == TokenType.Word) {
            cur_vars.AddLast(new Variable(cur_token.val, "Card"));
            variables?.AddLast(new Variable(cur_token.val, "Card"));

            dNext(TokenType.Word);
        } else throw new Exception("Expected variable.");

        Next(TokenType.RParen);

        Next(TokenType.Asign);
        Next(TokenType.GT);

        ParseBooleanExpression();
    }

    private void PostAction(Effect effectParent) {
        if (cur_token.val == Keyword.PostAction.ToString())
            Next(TokenType.Keyword);
        else
            throw new Exception("Expected PostAction.");

        Next(TokenType.Colon);
        Next(TokenType.LCBracket);

        if (cur_token.val == Keyword.Type.ToString())
            Next(TokenType.Keyword);
        else
            throw new Exception("Expected Type.");

        Next(TokenType.Colon);

        Effect effect = FindEffect(ParseString());

        // Check and add params
        if (effect.parms.Count != 0) {
            Next(TokenType.Comma);

            int count = 0;

            SetParams(effect, ref count);

            if (count != effect.parms.Count)
                throw new Exception("Missing params.");
        }

        // Check if next Selector or PostAction
        if (cur_token.type == TokenType.Comma) {
            Next(TokenType.Comma);
            if (cur_token.val == Keyword.Selector.ToString()) {
                Next(TokenType.Keyword);
                Selector(effectParent, effect);

                if (cur_token.type == TokenType.Comma) {
                    Next(TokenType.Comma);
                    PostAction(effect);
                }
            } else if (cur_token.val == Keyword.PostAction.ToString()) {
                PostAction(effect);
            } else throw new Exception("Expected Selector or PostAction.");
        }

        Next(TokenType.RCBracket);

        // Add as postEffect of parent
        effectParent.PostEffect = effect;

        Console.WriteLine("Correct postEffect.");
    }

    private bool IsCardWithEffectCorrect() {
        foreach (var property in props)
        if (
            (string)property.value == "Silver"
            || (string)property.value == "Gold"
            || (string)property.value == "Leader"
        )
            return true;
        return false;
    }   

    private bool IsBoolean() {
        int startTokenIndex = cur_tokenIndex;
        Token startToken = cur_token;

        while (
            cur_token.type != TokenType.Semicolon
            && cur_token.type != TokenType.End
        ) {
            if (
                Enum.IsDefined(typeof(Boolean), cur_token.type.ToString())
                || cur_token.val == "true"
                || cur_token.val == "false"
            ) {
                cur_tokenIndex = startTokenIndex;
                cur_token = startToken;
                return true;
            }
            

            foreach (var variable in cur_vars) {
                if (
                    variable.name == cur_token.val
                    && variable.val is bool
                ) {
                    cur_tokenIndex = startTokenIndex;
                    cur_token = startToken;
                    return true;
                }
            }

            TokenType forceNext = cur_token.type;
            Next(forceNext);
        }

        cur_tokenIndex = startTokenIndex;
        cur_token = startToken;
        return false;
    }


    private delegate void DNext(TokenType tokenType);
    DNext dNext;

    public bool ParseBooleanExpression() {
        // Result to show
        var result = ParseOrExpression();

        // Check if end
        if (
            cur_token.type != TokenType.RCBracket
            && cur_token.type != TokenType.Comma 
            && cur_token.type != TokenType.RParen
            && cur_token.type != TokenType.Semicolon
        )
            throw new Exception("Invalid boolean expression");
        return result;
    }

    private bool ParseOrExpression() {
        var result = ParseAndExpression();

        while (cur_token.type == TokenType.OR) {
            dNext(TokenType.OR);
            var result2 = ParseAndExpression();
            result = result || result2;
        }
        return result;
    }

    private bool ParseAndExpression()
    {
        var result = ParseRelationalExpression();

        while (cur_token.type == TokenType.AND) {
            dNext(TokenType.AND);
            var result2 = ParseRelationalExpression();
            result = result && result2;
        }
        return result;
    }

    private bool ParseRelationalExpression() {
        var left = ParsePrimaryExpression();

        while (
            cur_token.type == TokenType.LT
            || cur_token.type == TokenType.GT
            || cur_token.type == TokenType.LE
            || cur_token.type == TokenType.GE
            || cur_token.type == TokenType.EQ
            || cur_token.type == TokenType.NE
        ) {
            if (cur_token.type == TokenType.LT) {
                dNext(TokenType.LT);

                if (left is int or bool) {
                    int leftValue = Convert.ToInt32(left);
                    int rightValue = Convert.ToInt32(ParsePrimaryExpression());
                    left = leftValue < rightValue;
                } else throw new Exception("Invalid type. Expected [int] or [bool].");
            } else if (cur_token.type == TokenType.GT) {
                dNext(TokenType.GT);

                if (left is int or bool) {
                    int leftValue = Convert.ToInt32(left);
                    int rightValue = Convert.ToInt32(ParsePrimaryExpression());
                    left = leftValue > rightValue;
                } else throw new Exception("Invalid type. Expected [int] or [bool].");
            } else if (cur_token.type == TokenType.LE) {
                dNext(TokenType.LE);

                if (left is int or bool) {
                    int leftValue = Convert.ToInt32(left);
                    int rightValue = Convert.ToInt32(ParsePrimaryExpression());
                    left = leftValue <= rightValue;
                } else throw new Exception("Invalid type. Expected [int] or [bool].");
            } else if (cur_token.type == TokenType.GE) {
                dNext(TokenType.GE);

                if (left is int or bool) {
                    int leftValue = Convert.ToInt32(left);
                    int rightValue = Convert.ToInt32(ParsePrimaryExpression());
                    left = leftValue >= rightValue;
                } else throw new Exception("Invalid type. Expected [int] or [bool].");
            } else if (cur_token.type == TokenType.EQ) {
                dNext(TokenType.EQ);

                if (left is int or bool) {
                    int leftValue = Convert.ToInt32(left);
                    int rightValue = Convert.ToInt32(ParsePrimaryExpression());
                    left = leftValue == rightValue;
                } else if (left is string value) {
                    string rightValue = ParseString();
                    left = value == rightValue;
                }
            } else if (cur_token.type == TokenType.NE) {
                dNext(TokenType.NE);

                if (left is int or bool) {
                    int leftValue = Convert.ToInt32(left);
                    int rightValue = Convert.ToInt32(ParsePrimaryExpression());
                    left = leftValue != rightValue;
                } else if (left is string value) {
                    string rightValue = ParseString();
                    left = value != rightValue;
                }
            }
        }
        return (bool)left;
    }

    private object ParsePrimaryExpression() {
        if (cur_token.val == "true") {
            dNext(TokenType.Word);
            return true;
        } else if (cur_token.val == "false") {
            dNext(TokenType.Word);
            return false;
        } else if (cur_token.type == TokenType.Number) {
            int value = int.Parse(cur_token.val);
            dNext(TokenType.Number);
            return value;
        } else if (cur_token.type == TokenType.LParen) {
            dNext(TokenType.LParen);
            bool result = ParseOrExpression();
            dNext(TokenType.RParen);
            return result;
        } else if(cur_token.type == TokenType.Word) {
            foreach (var variable in cur_vars) {
                if (
                    variable.name == cur_token.val
                    && variable.val is int value
                ) {
                    dNext(TokenType.Word);

                    if (cur_token.type == TokenType.Incr)
                        dNext(TokenType.Incr);

                    return value;
                } else if (
                    variable.name == cur_token.val
                    && variable.val is bool value2
                ) {
                    dNext(TokenType.Word);
                    return value2;
                } else if (
                    variable.name == cur_token.val
                    && variable.val is string value3
                )
                    return ParseString();
                else if (
                    variable.name == cur_token.val
                    && variable.type == "Card"
                    && (tokens[cur_tokenIndex + 2].val == "Power")
                ) {
                    dNext(TokenType.Word);
                    Next(TokenType.Point);
                    dNext(TokenType.Keyword);

                    return 0;
                } else if (
                    variable.name == cur_token.val
                    && variable.type == "Card" 
                    && (
                        tokens[cur_tokenIndex + 2].val == "Name"
                        || tokens[cur_tokenIndex + 2].val == "Type"
                        || tokens[cur_tokenIndex + 2].val == "Range"
                        || tokens[cur_tokenIndex + 2].val == "Faction"
                    )
                ) {
                    return ParseString();
                }
            }
            throw new Exception("Invalid variable.");
        } else if (cur_token.type == TokenType.Incr) {
            dNext(TokenType.Incr);

            foreach (var variable in cur_vars) {
                if (
                    variable.name == cur_token.val
                    && variable.val is int value
                ) {
                    dNext(TokenType.Word);
                    return value;
                }
            }

            throw new Exception("Invalid variable.");
        }
        else if (cur_token.val == "\"")
            return ParseString();
        else
            throw new Exception("Invalid boolean expression.");
    }

    private string ParseString() {
        string result = null;

        if (cur_token.type == TokenType.Quote) {
            dNext(TokenType.Quote);
            result = "";
            result += cur_token.val;
            dNext(TokenType.String);
            dNext(TokenType.Quote);
        } else if (cur_token.type == TokenType.Word) {
            bool IsCorrect = false;

            foreach (var variable in cur_vars) {
                if (
                    variable.name == cur_token.val
                    && variable.val is string value
                ) {
                    result = "";
                    result += value;
                    dNext(TokenType.Word);
                    IsCorrect = true;
                } else if (
                    variable.name == cur_token.val
                    && variable.type == "Card"
                    && (
                        tokens[cur_tokenIndex + 2].val == "Name"
                        || tokens[cur_tokenIndex + 2].val == "Type"
                        || tokens[cur_tokenIndex + 2].val == "Range" 
                        || tokens[cur_tokenIndex + 2].val == "Faction"
                    )
                ) {
                    result = "";
                    dNext(TokenType.Word);
                    Next(TokenType.Point);
                    dNext(TokenType.Keyword);
                    IsCorrect = true;
                }
            }

            if (!IsCorrect)
                throw new Exception("Invalid variable.");
        }

        if (cur_token.type == TokenType.At) {
            dNext(TokenType.At);
            result += ParseString(); 
        } else if(cur_token.type == TokenType.DoubleAt) {
            dNext(TokenType.DoubleAt);
            result += " " + ParseString(); 
        }

        return result;
    }
}   