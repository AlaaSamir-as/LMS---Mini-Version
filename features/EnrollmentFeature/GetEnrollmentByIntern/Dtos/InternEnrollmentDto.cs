namespace LMS___Mini_Version.features.EnrollmentFeature.GetEnrollmentByIntern.Dtos
{
    public class InternEnrollmentDto
    {
        public int EnrollmentId { get; set; }
        public int TrackId { get; set; }
        public string TrackName { get; set; } = string.Empty;
        public string EnrollmentDate { get; set; } = string.Empty;
        public string EnrollmentStatus { get; set; } = string.Empty;
    }
}
