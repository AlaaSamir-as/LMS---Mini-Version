namespace LMS___Mini_Version.features.TrackFeature.AddTrack.ViewModels
{
    public class CreateTrackRequestVM
    {
        public string Name { get; set; } = string.Empty;
        public decimal Fees { get; set; }
        public int MaxCapacity { get; set; }
    }
}

