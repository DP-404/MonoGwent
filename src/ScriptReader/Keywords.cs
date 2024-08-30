
namespace MonoGwent;

enum Keyword
{
    effect,
    Name,
    Params,
    Action,
    TriggerPlayer,
    Board,
    HandOfPlayer,
    FieldOfPlayer,
    GraveyardOfPlayer,
    DeckOfPlayer,
    Hand,
    Field,
    Deck,
    Graveyard,
    Owner,
    Find,
    Push,
    SendBottom,
    Pop,
    Remove,
    Shuffle,
    Add,
    Number,
    String,
    Bool,
    card,
    Type,
    Faction,
    Power,
    Range,
    OnActivation,
    Effect,
    Selector,
    Source,
    Single,
    Predicate,
    PostAction
}

public enum TokenType {
    // Base
    Number,
    Identifier,
    Keyword,

    // Symbols
    Point,
    Colon,
    Semicolon,
    Comma,
    LParen,
    RParen,
    LCBracket,
    RCBracket,
    LSBracket,
    RSBracket,

    // Arithmetic Operators
    Plus,
    Minus,
    Mult,
    Div,
    Incr,
    Decr,

    // Boolean Operators
    Quote,
    String,
    Asign,
    EQ,
    NE,
    LT,
    LE,
    GT,
    GE,
    OR,
    AND,
    XOR,
    At,
    DoubleAt,
    PlusCom,
    MinusCom,
    MultiCom,
    DivisionCom,
    XORCom,

    // Extra
    End
}

enum ContextPropertiesAndMethods
{
    TriggerPlayer,
    Board,
    HandOfPlayer,
    FieldOfPlayer,
    GraveyardOfPlayer,
    DeckOfPlayer,
    Hand,
    Field,
    Deck,
    Graveyard
}

enum CardProperties
{
    Owner,
    Power,
    Range,
    Faction,
    Type,
    Name
}

enum CardListProperties
{
    Find,
    Pop,
}

enum SyntacticSugarContext
{
    Board,
    Hand,
    Field,
    Deck,
    Graveyard
}

enum ContextMethods
{
    Board,
    HandOfPlayer,
    FieldOfPlayer,
    GraveyardOfPlayer,
    DeckOfPlayer,
    Hand,
    Field,
    Deck,
    Graveyard
}

enum CardListMethods
{
    Push,
    SendBottom,
    Remove,
    Shuffle,
    Add,
    Find
}

public enum Source
{
    hand,
    otherHand,
    deck,
    otherDeck,
    field,
    otherField,
    parent
}

public enum Booleans
{
    EQ,
    NE,
    LT,
    GT,
    LE,
    GE,
    OR,
    AND
}

public enum CompositeOperators
{
    PlusCom,
    MinusCom,
    MultiCom,
    DivisionCom,
    XORCom
}
