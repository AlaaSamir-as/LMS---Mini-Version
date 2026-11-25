namespace LMS___Mini_Version.features.TrackFeature.GetAllTracks.Dtos
{
    public class TrackDto
    {
        public int id { set; get;}

        public string name { set; get; } = string.Empty;

        public int MaxCapacity { get; set; }

        public int? totalenrollment { get; set; } = 0;
    }
}
