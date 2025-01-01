namespace VisualizationSystem.Services.Utilities.Settings;

public interface ISettingsSubject
{
    void Attach(ISettingsObserver observer);

    void Detach(ISettingsObserver observer);

    void Notify();
}
