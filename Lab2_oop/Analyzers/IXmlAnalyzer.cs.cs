using Lab2_oop.Models;
namespace Lab2_oop.Analyzers
{
    public interface IXmlAnalyzer
    {
        List<MemberInfo> Search(string filePath, SearchCriteria criteria);
    }
}