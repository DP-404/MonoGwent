using System;
using MonoGwent;

while (true) {
    try {
        using var game = new Gwent();
        game.Run();
    } catch (GameRestartException) {
        continue;
    }
    Environment.Exit(0);
}