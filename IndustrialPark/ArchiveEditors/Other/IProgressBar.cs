namespace IndustrialPark;

public interface IProgressBar
{
    public void SetProgressBar(int min, int max, int step);
    public void PerformStep(string label = "");
}