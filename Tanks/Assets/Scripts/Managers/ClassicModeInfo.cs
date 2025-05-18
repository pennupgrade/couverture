public class ClassicModeInfo {
    private StaticSaveStateManager classicModeSave;
    
    public ClassicModeInfo() {
        classicModeSave = StaticSaveStateManager.LoadStaticSave();
    }

    public int GetClassicModeHighScore() {
        if (classicModeSave is null) {
            return 0;
        }
        return classicModeSave.GetClassicModeHighScore();
    }
}