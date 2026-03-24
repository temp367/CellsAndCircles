public static class GameServices
{
    public static GameManager Game;
    public static GridManager Grid { get; private set; }
    public static TurnManager Turn { get; private set; }
    public static CommandSystem CommandSys { get; private set; }
    public static AbilitySystem Ability { get; private set; }
    public static EtherSystem Ether { get; private set; }
    public static UIManager Ui { get; private set; }
    public static HighlightSystem Highlight { get; private set; }

    public static void Register(GridManager grid) => Grid = grid;
    public static void Register(TurnManager turn) => Turn = turn;
    public static void Register(CommandSystem command) => CommandSys = command;
    public static void Register(AbilitySystem ability) => Ability = ability;
    public static void Register(EtherSystem ether) => Ether = ether;
    public static void Register(UIManager ui) => Ui = ui;
    public static void Register(HighlightSystem hl) => Highlight = hl;
}