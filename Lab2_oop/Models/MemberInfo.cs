namespace Lab2_oop.Models
{
    public class MemberInfo
    {
        public string SectionName { get; set; } = string.Empty;
        public string CoachName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;
        public string Rank { get; set; } = string.Empty;
        public string AdditionalInfo { get; set; } = string.Empty;
        public override string ToString() => $"{Role}: {FullName} ({SectionName})";
    }
}