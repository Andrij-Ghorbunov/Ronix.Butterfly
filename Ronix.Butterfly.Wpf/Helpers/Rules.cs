namespace Ronix.Butterfly.Wpf.Helpers
{
    public static class Rules
    {
        public const string RulesUkr = @"У цій грі чорні та білі шашки намагаються прорватися через поле бою (верхня права чверть дошки) до протилежного краю. Але при цьому вони рухаються перпендикулярно одні одним — білі прориваються зліва направо, а чорні знизу вгору.

Шашка рухається кроком шахового пішака. За хід вона може переміститися на вільне поле попереду себе або атакувати шашку супротивника діагональним рухом вперед-вліво чи вперед-вправо. При цьому напрямок «вперед» для різних гравців різний: білі шашки можуть бити вправо-вгору і вправо-вниз, а чорні — вправо-вгору і вліво-вгору.

Не можна атакувати шашку супротивника, яка ще не зайшла на поле бою.

Шашка, що досягнула останньої для себе лінії (тобто верхньої для чорних і правої для білих), може за окремий хід бути зарахована. Зарахована шашка знімається з поля і стає переможним балом відповідного гравця.

Гра припиняється, якщо гравець, який повинен ходити, не має легальних ходів. Перемагає гравець із більшою кількістю зарахованих шашок, а при рівності балів — гравець, у якого більше розвинутих шашок (тобто таких, що зараз перебувають на полі бою). При рівності й цього показника зараховується нічия.";

        public const string RulesEng = @"In this game, black and whit pieces try to break through the Battlefield (top right quarter of the board) to the opposite edge. However, they move in perpendicular directions: white move from left to right, and black from bottom to top.

A piece moves like a chess pawn. In a single turn it can move one space to the front or capture an opposite player's piece diagonally forward-left or forward-right. Here, 'forward' has different meaning for different players: white pieces can capture top-right and bottom-right, while black ones top-right and top-left.

A piece which has not yet entered the Battlefield cannot be captured.

A piece that reaches the last line (topmost for black or rightmost for white) can be Scored as a separate turn. A Scored piece is removed from the board and is converted into a Victory Point for the corresponding player.

The game ends when the player whose turn has come has no legal moves. Victory is given to the player with more Scored pieces, and if equal - the player having more Developed pieces (i. e. ones in the Battlefield currently). If both Score and Development are equal, the game ends in a draw.";
    }
}
