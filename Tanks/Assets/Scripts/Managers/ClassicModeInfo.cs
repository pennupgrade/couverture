public class ClassicModeInfo {
    private SaveStateManager classicModeSave;
    
    public ClassicModeInfo() {
        classicModeSave = SaveStateManager.TryLoadSaveState(SaveStateManagerGameObject.CLASSIC_MODE_SAVE_FILE);
    }

    public int GetClassicModeHighScore() {
        if (classicModeSave is null) {
            return 0;
        }
        return classicModeSave.GetClassicModeHighScore();
    }
}