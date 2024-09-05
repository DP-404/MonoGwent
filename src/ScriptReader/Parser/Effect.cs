using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoGwent;

public class Effect : IEffect, ICloneable {
    // Effect name
    public string name {get;}
    // Card parameters
    public LinkedList<Variable> parms;
    // Targets variable name
    private string TargetsName;
    // Targets to search
    public string sourceTargets {get;set;}

    public bool single {get;set;}
    // Selector predicate
    public Instruction predicate = new();
    // Stored variables
    public LinkedList<Variable> vars;
    // Effect instructions to exec
    private List<Instruction> instructions;
    // After-effect
    public Effect PostEffect {get;set;}
    // Parent effect (if any)
    public Effect ParentEffect {get;set;}

    private Instruction cur_instruction;
    private int cur_instructionIndex = 0;

    private string cur_keyword;
    private int cur_keywordIndex = 0;


    public string Description {get => "";}
    public EffectType Type {get => EffectType.ON_USE;}

    delegate List<Card> ListReturn(string player);

    public Effect(string name, string targetName, List<Instruction> instructions, LinkedList<Variable> vars) {
        this.name = name;
        this.TargetsName = targetName;
        this.instructions = instructions;
        this.vars = vars;
        cur_instruction = instructions[0];
        cur_keyword = cur_instruction.Keywords[0];
    } 

    public Effect(string name, string targetName, List<Instruction> instructions, LinkedList<Variable> vars ,LinkedList<Variable> parms) {
        this.name = name;
        this.TargetsName = targetName;
        this.instructions = instructions;
        this.vars = vars;
        this.parms = parms;
        cur_instruction = instructions[0];
        cur_keyword = cur_instruction.Keywords[0];
    }

    public object Clone() {
        Effect effect = new(name, TargetsName, this.instructions, vars, parms) {
            sourceTargets = sourceTargets,
            single = single,
            predicate = (Instruction)predicate.Clone()
        };

        if(PostEffect != null) effect.PostEffect = (Effect)PostEffect.Clone();
        if(ParentEffect != null) effect.ParentEffect = (Effect)ParentEffect.Clone();

        List<Instruction> instructions = new();

        foreach(Instruction instruction in this.instructions)
            instructions.Add((Instruction)instruction.Clone());

        effect.instructions = instructions;

        LinkedList<Variable> variables = new();

        foreach(Variable variable in vars)
            variables.AddLast((Variable)variable.Clone());

        effect.vars = variables;

        LinkedList<Variable> parameters = new();

        foreach(Variable parameter in parms)
            parameters.AddLast((Variable)parameter.Clone());

        effect.parms = parameters;

        return effect;
    }

    public void Activate() {   
        Source();
        FinishInstruction();
        Reset();
    }
    public bool Eval(BattleManager bm) {
        return true;
    }
    public void Use(BattleManager bm) {
        Activate();
    }

    private void FinishInstruction() {
        Console.WriteLine("Starded instruction.");
        cur_instruction.Debug();

        // Means declaration
        if (cur_instruction.Keywords[1] == "=") {
            foreach(Variable variable in vars) {
                if (
                    variable.name == cur_keyword
                    && variable.val is int
                ) {
                    Next();
                    Next();
                    variable.val = NumericExpression();
                    break;
                } else if (
                    variable.name == cur_keyword
                    && variable.val is bool
                ) {
                    Next();
                    Next();
                    variable.val = BooleanExpression();
                    break;
                } else if(variable.name == cur_keyword) {
                    variable.val = Declaration();
                    break;
                }
            }
        } else if (cur_keyword == "for") {
            ForIntruction();
        } else if (cur_keyword == "while") {
            WhileInstruction();
        } else if (
            cur_keyword == "++"
            || IsNumericVariable()
            || IsNumericProperty()
        ) {
            Increase();
        } else MethodCall();

        if (cur_instruction.Count != 0)
            FinishInstruction();
        else if(PostEffect != null)
        {
            PostEffect.ParentEffect = this;
            PostEffect.Activate();
        }
    }

    private void Source() {
        foreach (var variable in vars) {
            if (variable.name == TargetsName) {
                variable.val = sourceTargets switch {
                    "board" or "Board" => Context.Board,
                    "hand" or "Hand" => Context.Hand,
                    "deck" or "Deck" => Context.Deck,
                    "field" or "Field" => Context.Field,
                    "graveyard" or "Graveyard" => Context.Graveyard,
                    "otherHand" or "OtherHand" => Context.HandOfPlayer(Context.OtherPlayer),
                    "otherDeck" or "OtherDeck" => Context.DeckOfPlayer(Context.OtherPlayer),
                    "otherField" or "OtherField" => Context.FieldOfPlayer(Context.OtherPlayer),
                    "otherGraveyard" or "OtherGraveyard" => Context.GraveyardOfPlayer(Context.OtherPlayer),
                    "parent" or "Parent" => ParentEffect.vars.Where(pVar => pVar.name == ParentEffect.TargetsName).Select(pVar => (List<Card>)pVar.val).First(),
                    "empty" or "Empty" => new List<Card>(),
                    _ => throw new Exception($"Invalid variable name: {variable.val}.")
                };

                if (sourceTargets != "empty") {
                    if(single) {
                        List<Card> cards = (List<Card>)variable.val;
                        List<Card> cards2 = new(){cards[0]};
                        variable.val = cards2;
                    }

                    predicate.Debug();
                    cur_instruction = predicate;
                    cur_keyword = cur_instruction.Keywords[0]; 
                    Variable variable2 = SetVariableValue(cur_keyword);
                    Next();

                    List<Card> cards3 = [.. (List<Card>)variable.val];

                    foreach (var card in (List<Card>)variable.val) {
                        variable2.val = card;

                        if (!BooleanExpression()) cards3.Remove(card);

                        cur_instruction = predicate;
                        cur_keywordIndex = 1;
                        cur_keyword = cur_instruction.Keywords[1];
                    }

                    variable.val = cards3;

                    cur_instruction = instructions[0];
                    cur_keywordIndex = 0;
                    cur_keyword = cur_instruction.Keywords[0];
                    cur_instructionIndex = 0;
                }

                return;
            }
        }

        throw new Exception();
    }

    private void Reset() {
        cur_instruction = instructions[0];
        cur_keywordIndex = 0;
        cur_keyword = cur_instruction.Keywords[0];
        cur_instructionIndex = 0;
    }

    // Advance to next instruction
    private void Next() {
        // If End, finish
        if (cur_keyword != "End") {
            cur_keywordIndex++;
            cur_keyword = cur_keywordIndex < cur_instruction.Count ?
            cur_instruction.Keywords[cur_keywordIndex] : cur_keyword = "End";
        }

       // End of Instruction > Step to next instruction
        if (cur_keyword == "End") {
            cur_keywordIndex = 0;
            cur_instructionIndex++;
            cur_instruction = cur_instructionIndex < instructions.Count ?
            instructions[cur_instructionIndex] : new Instruction();

            if (cur_instruction.Count > 0) {
                cur_keyword = cur_instruction.Keywords[cur_keywordIndex];
            }
        }
    }

    private object Declaration() {
        Next(); 
        Next();
        return Parameter();
    }

    private object Parameter() {
        var vartype = VariableType();
        if (vartype == "Context") {
            Next();

            if (cur_keywordIndex == 0) return new Context();
            else if (cur_keyword == Enum.GetName(Keyword.TriggerPlayer)) {
                Next();
                return Context.TriggerPlayer;
            } else {
                List<Card> cards;

                var keyword = cur_keyword;
                Next();
                cards = keyword switch {
                    "board" or "Board" => Context.Board,
                    "hand" or "Hand" => Context.Hand,
                    "deck" or "Deck" => Context.Deck,
                    "field" or "Field" => Context.Field,
                    "graveyard" or "Graveyard" => Context.Graveyard,
                    "handOfPlayer" or "HandOfPlayer" => Context.HandOfPlayer((string)Parameter()),
                    "deckOfPlayer" or "DeckOfPlayer" => Context.DeckOfPlayer((string)Parameter()),
                    "fieldOfPlayer" or "FieldOfPlayer" => Context.FieldOfPlayer((string)Parameter()),
                    "graveyardOfPlayer" or "GraveyardOfPlayer" => Context.GraveyardOfPlayer((string)Parameter()),
                    _ => throw new Exception($"Invalid keyword: {keyword}.")
                };

                while (
                    cur_keyword == "Pop"
                    || cur_keyword == "Find"
                    || cur_keyword == "["
                    || cur_keywordIndex == 0
                ) {
                    if (cur_keywordIndex == 0) return cards;
                    else if (cur_keyword == "Pop") {
                        Next();
                        Card card = cards.Last();
                        cards.Remove(card);
                        return card;
                    } else if(cur_keyword == "[") {
                        Next();
                        var card = cards[NumericExpression()];
                        Next();
                        if(cur_keywordIndex == 0) return card;
                        else if(cur_keyword == "Owner")
                        {
                            Next();

                            return card.owner;
                        }
                    } else if(cur_keyword == "Find") {
                        Next();
                        Variable variable = SetVariableValue(cur_keyword);
                        Next();

                        Instruction startInstruction = cur_instruction;
                        int startInstructionIndex = cur_instructionIndex;
                        string startKeyWord = cur_keyword;
                        int startKeyWordIndex = cur_keywordIndex; 

                        List<Card> cards2 = [.. cards];

                        foreach (var card in cards) {
                            cur_instruction = startInstruction;
                            cur_instructionIndex = startInstructionIndex;
                            cur_keyword = startKeyWord;
                            cur_keywordIndex = startKeyWordIndex;

                            variable.val = card;

                            if(!BooleanExpression()) cards2.Remove(card);
                        }

                        cards = cards2;

                        if (
                            cur_keyword != "Pop"
                            && cur_keyword != "Find"
                            && cur_keyword != "["
                        )
                            return cards;
                    }
                    else throw new Exception();    
                }
                throw new Exception();
            }
        }

        else if (vartype == "List<Card>") {
            string nameVariable = cur_keyword;

            var cards = (List<Card>)SetVariableValue(cur_keyword).val;

            Next();

            while(cur_keyword == "Pop" || cur_keyword == "Find" || cur_keywordIndex == 0 || cur_keyword == "[") {
                if (cur_keywordIndex == 0) return SetVariableValue(nameVariable).val;
                else if (cur_keyword == "Pop") {
                    Next();
                    Card card = cards.Last();
                    cards.Remove(card);
                    return card;
                } else if(cur_keyword == "[") {
                    Next();
                    var card = cards[NumericExpression()];
                    Next();
                    return card;
                } else if (cur_keyword == "Find") {
                    Next();
                    Variable variable = SetVariableValue(cur_keyword);
                    Next();
        
                    Instruction startInstruction = cur_instruction;
                    int startInstructionIndex = cur_instructionIndex;
                    string startKeyWord = cur_keyword;
                    int startKeyWordIndex = cur_keywordIndex; 

                    List<Card> cards2 = [.. cards];

                    foreach (var card in cards) {
                        cur_instruction = startInstruction;
                        cur_instructionIndex = startInstructionIndex;
                        cur_keyword = startKeyWord;
                        cur_keywordIndex = startKeyWordIndex;

                        variable.val = card;

                        if(!BooleanExpression()) cards2.Remove(card);
                    }

                    cards = cards2;

                    if (cur_keyword != "Pop" && cur_keyword != "Find") return cards;
                } else throw new Exception();  
            }
            throw new Exception();

        } else if (vartype == "Card") {
            string nameVariable = cur_keyword;
            Next();

            if (cur_keywordIndex == 0) return SetVariableValue(nameVariable).val;
            else if (cur_keyword == "Owner") {
                Next();
                var card = (Card)SetVariableValue(nameVariable).val;
                return card.owner;
            }
            else throw new Exception();

        } else if (VariableType() == "Player") {
            string nameVariable = cur_keyword;
            Next();

            return SetVariableValue(nameVariable).val;
        } 
        else throw new Exception();
    }

    private string VariableType() {
        foreach (var variable in vars) {
            if (variable.name == cur_keyword)
                return variable.type;
        }
        return null;
    }

    private Variable SetVariableValue(string name) {
        foreach (var variable in vars)
        if(variable.name == name) return variable;

        throw new Exception();
    }

    private void MethodCall() {
        foreach (var variable in vars) {
            if (variable.name == cur_keyword) {
                Next();
                if(variable.type == "Context") GetContext();
                else if(variable.type == "List<Card>") CardList((List<Card>)variable.val);
                break;
            } 
        } 
    }

    public bool IsNumericVariable() {
        foreach (var variable in vars)
            if (
                variable.name == cur_keyword
                && variable.val is int
            )
                return true;
        return false;
    }

    public bool IsNumericProperty() {
        foreach (var variable in vars)
            if (
                variable.name == cur_keyword
                && variable.type == "Card"
                && cur_instruction.Keywords[cur_keywordIndex+1] == "Power"
            )
                return true;
        return false;
    }

    private void Increase() {
        if (cur_keyword != "++") {
            string variableName = cur_keyword;
            int newValue = 0;

            foreach(var variable in vars) {
                if (
                    variable.name == cur_keyword
                    && variable.val is int value
                ) {
                    newValue = value;

                    Next();
                    break;
                } else if (
                    variable.name == cur_keyword
                    && variable.type == "Card" 
                    && cur_instruction.Keywords[cur_keywordIndex+1] == "Power")
                {
                    var card = (Card)variable.val;
                    newValue = card.power;

                    Next();
                    Next();
                    break;
                }
            } 

            if (cur_keyword == "+=") {
                Next();
                newValue += NumericExpression();
            } else if(cur_keyword == "-=") {
                Next();
                newValue -= NumericExpression();
            } else if(cur_keyword == "/=") {
                Next();
                newValue /= NumericExpression();
            } else if(cur_keyword == "*=") {
                Next();
                newValue *= NumericExpression();
            } else if(cur_keyword == "^=") {
                Next();
                newValue ^= NumericExpression();
            } else if(cur_keyword == "++") {
                Next();
                newValue++;
            }

            foreach (var variable in vars) {
                if (
                    variable.name == variableName
                    && variable.val is int value1
                ) {
                    variable.val = newValue;
                    break;
                } else if (
                    variable.name == variableName
                    && variable.type == "Card"
                ) {
                    var card = (Card)variable.val;
                    card.power = newValue;
                    break;
                }
            }
        } else {
            Next();

            foreach(var variable in vars) {
                if(
                    variable.name == cur_keyword
                    && variable.val is int value
                ) {
                    int temp = value;
                    temp++;
                    variable.val = temp;

                    Next();
                    return;
                } else if (
                    variable.name == cur_keyword
                    && variable.type == "Card" 
                    && cur_instruction.Keywords[cur_keywordIndex+1] == "Power"
                ) {
                    var card = (Card)variable.val;
                    card.power++;

                    Next();
                    Next();
                    return;
                }
            } 
            throw new Exception();
        }
    }

    private void ForIntruction() {
        Next();
        string nameCard = cur_keyword;
        Next();
        var cards = (List<Card>)SetVariableValue(cur_keyword).val;
        Next();

        Instruction startInstruction = cur_instruction;
        int startInstructionIndex = cur_instructionIndex;

        int count = cards.Count - 1;

        foreach (var card in cards) {
            Variable variable = SetVariableValue(nameCard);
            variable.val = card;

            FinishFor(count--);
        }

        while (cur_keyword != "ForEnd") Next();

        Next();

        void FinishFor(int count) {
            Console.WriteLine("For statement started.");

            cur_instruction.Debug();

            if (cur_instruction.Keywords[1] == "=") {
                foreach (Variable variable in vars) {
                    if (
                        variable.name == cur_keyword
                        && variable.val is int
                    ) {
                        Next();
                        Next();  
                        variable.val = NumericExpression();
                        break;
                    } else if (
                        variable.name == cur_keyword
                        && variable.val is bool
                    ) {
                        Next();
                        Next();
                        variable.val = BooleanExpression();
                        break;
                    } else if (variable.name == cur_keyword) {
                        variable.val = Declaration();
                        break;
                    }
                }
            } else if (cur_keyword == "for") {
                ForIntruction();
            } else if (cur_keyword == "while") {
                WhileInstruction();
            } else if (
                cur_keyword == "++"
                || IsNumericVariable()
                || IsNumericProperty()
            ) {
                Increase();
            } else MethodCall();

            // Not completed > Read next Instruction
            if (cur_keyword != "ForEnd") {
                FinishFor(count);
            } else if (
                cur_keyword == "ForEnd"
                && count > 0
            ) { // Restart cycle
                cur_instruction = startInstruction;
                cur_instructionIndex = startInstructionIndex;
                cur_keyword = startInstruction.Keywords[0];
            }
        }
    }

    private void WhileInstruction() {
        Next();

        Instruction startInstruction = cur_instruction;
        int startInstructionIndex = cur_instructionIndex;

        bool argument = BooleanExpressionWhile();

        if (!argument) {
            while(cur_keyword != "WhileEnd") {
                Next();
            }
            Next();
        }  

        while (argument) {
            FinishWhile();

            if (!BooleanExpressionWhile()) {
                argument = false;

                while(cur_keyword != "WhileEnd") {
                    Next();
                }
                Next();
            }  
        }

        void FinishWhile() {
            Console.WriteLine("Started while statement.");

            cur_instruction.Debug();

            if (cur_instruction.Keywords[1] == "=") {
                foreach (var variable in vars) {
                    if (
                        variable.name == cur_keyword
                        && variable.val is int
                    ) {
                        Next();
                        Next();
                        variable.val = NumericExpression();
                        break;
                    } else if (
                        variable.name == cur_keyword
                        && variable.val is bool
                    ) {
                        Next();
                        Next();
                        variable.val = BooleanExpression();
                        break;
                    } else if (variable.name == cur_keyword) {
                        variable.val = Declaration();
                        break;
                    }
                }
            } else if (cur_keyword == "for") {
                ForIntruction();
            } else if (cur_keyword == "while") {
                WhileInstruction();
            } else if (
                cur_keyword == "++"
                || IsNumericVariable()
                || IsNumericProperty()
            ) {
                Increase();
            }
            else MethodCall();

            // Read next until end
            if(cur_keyword != "WhileEnd")
                FinishWhile();
        }

        bool BooleanExpressionWhile() {
            cur_instruction = startInstruction;
            cur_instructionIndex = startInstructionIndex;
            cur_keyword = startInstruction.Keywords[1];
            cur_keywordIndex = 1;

            return BooleanExpression();
        }
    }

    private void GetContext() {
        List<Card> cards;

        var keyword = cur_keyword;
        Next();
        cards = keyword switch {
            "board" or "Board" => Context.Board,
            "hand" or "Hand" => Context.Hand,
            "deck" or "Deck" => Context.Deck,
            "field" or "Field" => Context.Field,
            "graveyard" or "Graveyard" => Context.Graveyard,
            "handOfPlayer" or "HandOfPlayer" => Context.HandOfPlayer((string)Parameter()),
            "deckOfPlayer" or "DeckOfPlayer" => Context.DeckOfPlayer((string)Parameter()),
            "fieldOfPlayer" or "FieldOfPlayer" => Context.FieldOfPlayer((string)Parameter()),
            "graveyardOfPlayer" or "GraveyardOfPlayer" => Context.GraveyardOfPlayer((string)Parameter()),
            _ => throw new Exception($"Invalid keyword: {keyword}.")
        };

        CardList(cards);
    }

    private void CardList(List<Card> cards) {
        if (
            cur_keyword == "Push"
            || cur_keyword == "Add"
        ) {
            if (cards.SequenceEqual(Context.Board))
                throw new Exception("Cannot add cards to board through effects.");

            Next();
            var card = (Card)Parameter();
            cards.Add(card);
        } else if (cur_keyword == "SendBottom") {
            if (cards.SequenceEqual(Context.Board))
                throw new Exception("Cannot add cards to board through effects.");

            Next();
            cards.Insert(0, (Card)Parameter());
        } else if (cur_keyword == "Remove") {
            Next();

            var card = (Card)Parameter();
            if (cards.SequenceEqual(Context.Board)) {
                Context.bm.GetPlayerByName(card.owner).field.Remove(card);
            } else {
                cards.Remove(card);
            }
        } else if (cur_keyword == "Shuffle") {
            Next();

            Random random = new();

            int n = cards.Count;
            while (n > 1) {
                n--;
                int k = random.Next(n+1);
                (cards[n], cards[k]) = (cards[k], cards[n]);
            }
        } else if (cur_keyword == "Find") {
            Next();
            Variable variable = SetVariableValue(cur_keyword);
            Next();

            Instruction startInstruction = cur_instruction;
            int startInstructionIndex = cur_instructionIndex;
            string startKeyWord = cur_keyword;
            int startKeyWordIndex = cur_keywordIndex; 

            List<Card> cards2 = [.. cards];

            foreach (var card in cards) {
                cur_instruction = startInstruction;
                cur_instructionIndex = startInstructionIndex;
                cur_keyword = startKeyWord;
                cur_keywordIndex = startKeyWordIndex;

                variable.val = card;

                if (!BooleanExpression()) cards2.Remove(card);
            }

            cards = cards2;
            CardList(cards);
        }
    }


    bool BooleanExpression() {
        var result = ParseOrExpression();
        return result;
    }

    bool ParseOrExpression() {
        var result = ParseAndExpression();

        while (cur_keyword == "||") {
            Next();
            var result2 = ParseAndExpression();
            result = result || result2;
        }
        return result;
    }

    bool ParseAndExpression() {
        var result = ParseRelationalExpression();

        while (cur_keyword == "&&") {
            Next();
            var result2 = ParseRelationalExpression();
            result = result && result2;
        }
        return result;
    }

    bool ParseRelationalExpression() {
        var left = ParsePrimaryExpression();

        while (
            cur_keyword == "<"
            || cur_keyword == ">"
            || cur_keyword == "<=" 
            || cur_keyword == ">="
            || cur_keyword == "=="
            || cur_keyword == "!="
        ) {

            if (cur_keyword == "<") {
                Next();
                int leftValue = Convert.ToInt32(left);
                int rightValue = Convert.ToInt32(ParsePrimaryExpression());
                left = leftValue < rightValue;
            } else if (cur_keyword == ">" ) {
                Next();
                int leftValue = Convert.ToInt32(left);
                int rightValue = Convert.ToInt32(ParsePrimaryExpression());
                left = leftValue > rightValue;
            } else if (cur_keyword == "<=") {
                Next();
                int leftValue = Convert.ToInt32(left);
                int rightValue = Convert.ToInt32(ParsePrimaryExpression());
                left = leftValue <= rightValue;
            } else if (cur_keyword == ">=") {
                Next();
                int leftValue = Convert.ToInt32(left);
                int rightValue = Convert.ToInt32(ParsePrimaryExpression());
                left = leftValue >= rightValue;
            } else if (cur_keyword == "==") {
                Next();
                if (left is int or bool) {
                    int leftValue = Convert.ToInt32(left);
                    int rightValue = Convert.ToInt32(ParsePrimaryExpression());
                    left = leftValue == rightValue;
                } else if (left is string value) {
                    string rightValue = ParseString();
                    left = value == rightValue;
                }
            } else if (cur_keyword == "!=") {
                Next();
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


    object ParsePrimaryExpression()
    {
        if (cur_keyword == "true")
        {
            Next();
            return true;
        }
        else if (cur_keyword == "false")
        {
            Next();
            return false;
        }
        else if (int.TryParse(cur_keyword, out int value))
        {
            Next();

            return value;
        }
        else if(cur_keyword == "++")
        {
            Next();

            foreach(Variable variable in vars)
            {
                if(variable.name == cur_keyword)
                {
                    Next();
                    int numberTemp = (int)variable.val;
                    numberTemp++;
                    variable.val = numberTemp;
                    return numberTemp;
                }
            }
            
            throw new Exception();
        }
        else if (cur_keyword == "(")
        {
            Next();
            bool result = ParseOrExpression();
            Next();
            return result;
        }
        else if(char.IsLetter(cur_keyword[0]))
        {
            foreach(Variable variable in vars)
            {
                if(variable.name == cur_keyword && variable.val is int value2)
                {
                    Next();

                    if(cur_keyword == "++")
                    {       
                        Next();
                        int numberTemp = (int)variable.val;
                        numberTemp++;
                        value2 = (int)variable.val;
                        variable.val = numberTemp;
                    }
                    
                    return value2;
                }
                else if(variable.name == cur_keyword && variable.val is bool value3)
                {
                    Next();
                    return value3;
                }
                else if(variable.name == cur_keyword && variable.val is string value4) return ParseString();
                else if(variable.name == cur_keyword && variable.type == "Card" 
                && (cur_instruction.Keywords[cur_keywordIndex + 1] == "Power"))
                {
                    Next();
                    Next();

                    var card = (Card)variable.val;
                    return card.power;
                }
                else if(variable.name == cur_keyword && variable.type == "Card" 
                && (cur_instruction.Keywords[cur_keywordIndex + 1] == "Name" 
                || cur_instruction.Keywords[cur_keywordIndex + 1] == "Type"
                || cur_instruction.Keywords[cur_keywordIndex + 1] == "Range" 
                || cur_instruction.Keywords[cur_keywordIndex + 1] == "Faction"))
                return ParseString();    
            }

            throw new Exception("Wrong variable.");
        }
        else if (cur_keyword == "\"") return ParseString();
        else throw new Exception("Invalid boolean expression.");
    }


    private int Pow() {
        string keyWord = cur_keyword;

        // Correct keyword > Next and return num or (expr)
        if (int.TryParse(cur_keyword, out int value)) {
            int result = value;

            Next();

            if(cur_keyword == "^") {
                Next();
                result ^= Pow(); 
            }

            keyWord = cur_keyword;

            while (keyWord == "(") {
                Next();
                result *= NumericExpression();
                Next();

                keyWord = cur_keyword;
            }
            return result;
        } else if (keyWord == "(") {
            Next();
            int result = NumericExpression();
            Next();

            if (cur_keyword == "^") {
                Next();
                result ^= Pow(); 
            }

            keyWord = cur_keyword;

            while (keyWord == "(") {
                Next();
                result *= NumericExpression();
                Next();

                keyWord = cur_keyword;
            }
            return result;
        } else if (char.IsLetter(cur_keyword[0])) {
            foreach (var variable in vars) {
                if (
                    variable.name == cur_keyword
                    && variable.val is int value2
                ) {
                    int result = value2;

                    Next();

                    if (cur_keyword == "++") {
                        int temp = (int)variable.val;
                        temp++;
                        variable.val = temp;
                        Next();
                    }

                    if (cur_keyword == "^") {
                        Next();

                        result ^= Pow(); 
                    }

                    keyWord = cur_keyword;

                    while (keyWord == "(") {
                        Next();
                        result *= NumericExpression();
                        Next();

                        keyWord = cur_keyword;
                    }
                    return result;
                }   
            }
            throw new Exception("Invalid variable type. Expected [int].");
        } else if (cur_keyword == "++") {
            Next();

            foreach (var variable in vars) {
                if (
                    variable.name == cur_keyword
                    && variable.val is int value2
                ) {
                    value2++;
                    variable.val = value2;

                    int result = value2;   

                    Next();

                    if(cur_keyword == "^") {
                        Next();
                        result ^= Pow(); 
                    }

                    keyWord = cur_keyword;

                    while(keyWord == "(") {
                        Next();
                        result *= NumericExpression();
                        Next();

                        keyWord = cur_keyword;
                    }
                    return result;
                }   
            }
            throw new Exception("Invalid variable type. Expected [int].");
        }
        else throw new Exception($"Unexpected keyword: {keyWord}");
    }

    private int MultDiv() {
        int result = Pow();

        // Keep processing so long as keyword is mult or div
        while (
            cur_keyword == "*"
            || cur_keyword == "/"
        ) {
            string keyWord = cur_keyword;

            if (keyWord == "*") {
                Next();
                result *= Pow();
            } else if(keyWord == "/") {
                Next();
                result /= Pow();
            }
        }
        return result;
    }

    private int NumericExpression() {
        int result = MultDiv();

        // Keep processing so long as keyword is sum or sub
        while (
            cur_keyword == "+"
            || cur_keyword == "-"
        ) {
            string keyWord = cur_keyword;

            if (keyWord == "+") {
                Next();
                result += MultDiv();
            } else if (keyWord == "-") {
                Next();
                result -= MultDiv();
            }
        }
        return result;
    }

    private string ParseString() { 
        string result = null;

        if (cur_keyword == "\"") {
            Next();
            result = "";
            result += cur_keyword;
            Next();
            Next();
        } else if (char.IsLetter(cur_keyword[0])) {
            bool IsCorrect = false;

            foreach (var variable in vars) {
                if (
                    variable.name == cur_keyword
                    && variable.val is string value
                ) {
                    result = "";
                    result += value;
                    Next();
                    IsCorrect = true;
                } else if(
                    variable.name == cur_keyword && variable.type == "Card" 
                    && (
                        cur_instruction.Keywords[cur_keywordIndex + 1] == "Name"
                        || cur_instruction.Keywords[cur_keywordIndex + 1] == "Type" 
                        || cur_instruction.Keywords[cur_keywordIndex + 1] == "Range" 
                        || cur_instruction.Keywords[cur_keywordIndex + 1] == "Faction"
                    )
                ) {
                    var card = (Card)variable.val;
                    
                    Next();
                    
                    if(cur_keyword == "Name") result = card.name;
                    else if(cur_keyword == "Type") result = card.type_name;
                    else if(cur_keyword == "Faction") result = card.faction;
                    else if(cur_keyword == "Range") {
                        result = "";

                        foreach (var row in card.types) {
                            if (result != "") result += ", ";
                            result += Enum.GetName(row);
                        }
                    } 
                    
                    Next();
                    IsCorrect = true;
                }
            }
            
            if (!IsCorrect) throw new Exception("Invalid variable name.");
        }

        if (cur_keyword == "@") {
            Next();
            result += ParseString(); 
        } else if (cur_keyword == "@@") {
            Next();
            result += " " + ParseString(); 
        }

        return result;
    }
}