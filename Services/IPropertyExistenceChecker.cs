namespace CourseLibrary.Api.Services
{
    public interface IPropertyExistenceChecker
    {
        bool TypeHasProperties<T>(string fields);
        bool FieldsHasIdProperty(string fields);
    }
}