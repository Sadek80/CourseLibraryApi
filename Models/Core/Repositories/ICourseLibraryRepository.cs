using CourseLibrary.Api.Models.Core.Domain;
using CourseLibrary.Api.ResourcesParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Models.Core.Repositories
{
    public interface ICourseLibraryRepository
    {
        IEnumerable<Course> GetCourses(Guid authorId);
        Course GetCourse(Guid authorId, Guid courseId);
        void AddCourse(Guid authorId, Course course);
        void UpdateCourse(Course course);
        void DeleteCourse(Course course);
        IEnumerable<Author> GetAuthors();
        IEnumerable<Author> GetAuthors(AuthorsResourceParameters authorsParameters);
        Author GetAuthor(Guid AuhtorIs);
        IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorIds);
        void AddAuthor(Author author);
        void UpdateAuthor(Author author);
        void DeleteAuthor(Author author);
        bool AuthorExists(Guid authorId);
        bool Save();
    }
}
