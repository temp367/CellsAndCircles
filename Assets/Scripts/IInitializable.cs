public interface IInitializable
{
    // Приоритет: чем меньше число, тем раньше инициализация
    int InitPriority { get; }
    
    // Вызывается для инициализации
    bool Initialize();
    
    // Название системы (для логов)
    string SystemName { get; }
}