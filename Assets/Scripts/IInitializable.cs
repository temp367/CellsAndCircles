public interface IInitializable
{
    InitStage InitStage { get; }
    
    void Initialize();
}