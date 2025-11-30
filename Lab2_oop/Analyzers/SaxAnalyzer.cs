using System.Xml;
using Lab2_oop.Models;

namespace Lab2_oop.Analyzers
{
    public class SaxAnalyzer : IXmlAnalyzer
    {
        public List<MemberInfo> Search(string filePath, SearchCriteria criteria)
        {
            var results = new List<MemberInfo>();
            using (XmlReader reader = XmlReader.Create(filePath))
            {
                string currentSection = "";
                string currentCoach = "";

                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == "Section")
                        {
                            currentSection = reader.GetAttribute("name") ?? "";
                            currentCoach = reader.GetAttribute("coach") ?? "";
                        }

                        if (reader.Name == "Participant")
                        {
                            string name = reader.GetAttribute("fullName") ?? "";
                            string role = reader.GetAttribute("type") ?? "";
                            string group = reader.GetAttribute("group") ?? "-";
                            string rank = reader.GetAttribute("rank") ?? "-";
                            string position = reader.GetAttribute("position") ?? "";
                            string info = !string.IsNullOrEmpty(group) && group != "-" ? group : position;

                            bool matchKeyword = string.IsNullOrEmpty(criteria.Keyword) ||
                                                name.Contains(criteria.Keyword, StringComparison.OrdinalIgnoreCase) ||
                                                currentSection.Contains(criteria.Keyword, StringComparison.OrdinalIgnoreCase) ||
                                                currentCoach.Contains(criteria.Keyword, StringComparison.OrdinalIgnoreCase);

                            bool matchRole = string.IsNullOrEmpty(criteria.Role) || role == criteria.Role;
                            bool matchSection = string.IsNullOrEmpty(criteria.Section) || currentSection == criteria.Section;
                            bool matchGroup = string.IsNullOrEmpty(criteria.Group) || group == criteria.Group;
                            bool matchRank = string.IsNullOrEmpty(criteria.Rank) || rank == criteria.Rank;

                            if (matchKeyword && matchRole && matchSection && matchGroup && matchRank)
                            {
                                results.Add(new MemberInfo { SectionName = currentSection, CoachName = currentCoach, FullName = name, Role = role, Group = group, Rank = rank, AdditionalInfo = info });
                            }
                        }
                    }
                }
            }
            return results;
        }
    }
}