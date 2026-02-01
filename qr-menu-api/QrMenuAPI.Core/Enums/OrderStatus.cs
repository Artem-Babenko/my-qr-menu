namespace QrMenuAPI.Core.Enums;

public enum OrderStatus
{
    New = 1,      // створено, ще ніхто не взяв
    Accepted,    // офіціант взяв у роботу
    InKitchen,    // передано на кухню
    Cooking,      // готується
    Ready,        // готове, чекає видачі
    Completed,    // виконано (подано клієнту)
    Cancelled,    // скасовано
}
