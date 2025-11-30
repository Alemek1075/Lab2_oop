using System.Xml;
using Lab2_oop.Models;

namespace Lab2_oop.Analyzers
{
    public class DomAnalyzer : IXmlAnalyzer
    {
        public List<MemberInfo> Search(string filePath, SearchCriteria criteria)
        {
            var results = new List<MemberInfo>();
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            XmlNodeList? participants = doc.SelectNodes("//Participant");
            if (participants == null) return results;

            foreach (XmlNode node in participants)
            {
                string sectionName = node.ParentNode?.Attributes?["name"]?.Value ?? "Невідомо";
                string coachName = node.ParentNode?.Attributes?["coach"]?.Value ?? "";
                string name = node.Attributes?["fullName"]?.Value ?? "";
                string role = node.Attributes?["type"]?.Value ?? "";
                string group = node.Attributes?["group"]?.Value ?? "-";
                string rank = node.Attributes?["rank"]?.Value ?? "-";
                string position = node.Attributes?["position"]?.Value ?? "";

                string info = !string.IsNullOrEmpty(group) && group != "-" ? group : position;

                bool matchKeyword = string.IsNullOrEmpty(criteria.Keyword) ||
                                    name.Contains(criteria.Keyword, StringComparison.OrdinalIgnoreCase) ||
                                    sectionName.Contains(criteria.Keyword, StringComparison.OrdinalIgnoreCase) ||
                                    coachName.Contains(criteria.Keyword, StringComparison.OrdinalIgnoreCase);

                bool matchRole = string.IsNullOrEmpty(criteria.Role) || role == criteria.Role;
                bool matchSection = string.IsNullOrEmpty(criteria.Section) || sectionName == criteria.Section;
                bool matchGroup = string.IsNullOrEmpty(criteria.Group) || group == criteria.Group;
                bool matchRank = string.IsNullOrEmpty(criteria.Rank) || rank == criteria.Rank;

                if (matchKeyword && matchRole && matchSection && matchGroup && matchRank)
                {
                    results.Add(new MemberInfo { SectionName = sectionName, CoachName = coachName, FullName = name, Role = role, Group = group, Rank = rank, AdditionalInfo = info });
                }
            }
            return results;
        }
    }
}