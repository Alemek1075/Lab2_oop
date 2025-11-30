using System.Xml.Linq;
using Lab2_oop.Models;

namespace Lab2_oop.Analyzers
{
    public class LinqAnalyzer : IXmlAnalyzer
    {
        public List<MemberInfo> Search(string filePath, SearchCriteria criteria)
        {
            XDocument doc = XDocument.Load(filePath);

            var query = from p in doc.Descendants("Participant")
                        let sectionNode = p.Parent
                        let sectionName = sectionNode?.Attribute("name")?.Value ?? ""
                        let coachName = sectionNode?.Attribute("coach")?.Value ?? ""
                        let name = p.Attribute("fullName")?.Value ?? ""
                        let role = p.Attribute("type")?.Value ?? ""
                        let grp = p.Attribute("group")?.Value ?? "-"
                        let rank = p.Attribute("rank")?.Value ?? "-"
                        let position = p.Attribute("position")?.Value ?? ""

                        where (string.IsNullOrEmpty(criteria.Keyword) ||
                               name.Contains(criteria.Keyword, StringComparison.OrdinalIgnoreCase) ||
                               sectionName.Contains(criteria.Keyword, StringComparison.OrdinalIgnoreCase) ||
                               coachName.Contains(criteria.Keyword, StringComparison.OrdinalIgnoreCase))
                        && (string.IsNullOrEmpty(criteria.Role) || role == criteria.Role)
                        && (string.IsNullOrEmpty(criteria.Section) || sectionName == criteria.Section)
                        && (string.IsNullOrEmpty(criteria.Group) || grp == criteria.Group)
                        && (string.IsNullOrEmpty(criteria.Rank) || rank == criteria.Rank)

                        select new MemberInfo { SectionName = sectionName, CoachName = coachName, FullName = name, Role = role, Group = grp, Rank = rank, AdditionalInfo = !string.IsNullOrEmpty(grp) && grp != "-" ? grp : position };

            return query.ToList();
        }
    }
}