using CourseLibrary.Api.Helpers;
using CourseLibrary.Api.Models.Core;
using CourseLibrary.Api.Models.Core.Domain;
using CourseLibrary.Api.Models.Core.Repositories;
using CourseLibrary.Api.ResourcesParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Models.Persistence
{
    public class CourseLibraryRepository : ICourseLibraryRepository
    {
        private readonly AppDataContext _context;

        public CourseLibraryRepository(AppDataContext context)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(_context));
        }

        public void AddCourse(Guid authorId, Course course)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            if (course == null)
            {
                throw new ArgumentNullException(nameof(course));
            }
            // always set the AuthorId to the passed-in authorId
            course.AuthorId = authorId;
            _context.Courses.Add(course);
        }

        public void DeleteCourse(Course course)
        {
            _context.Courses.Remove(course);
        }

        public Course GetCourse(Guid authorId, Guid courseId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            if (courseId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(courseId));
            }

            return _context.Courses
              .Where(c => c.AuthorId == authorId && c.Id == courseId).FirstOrDefault();
        }

        public PagedList<Course> GetCourses(Guid authorId, BaseResourcesParameters parameters)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            var collection = _context.Courses
                        .Where(c => c.AuthorId == authorId)
                        .OrderBy(c => c.Title) as IQueryable<Course>;

            if (!string.IsNullOrWhiteSpace(parameters.searchQuery))
            {
                collection = collection.Where(c => c.Title.Contains(parameters.searchQuery.Trim()));
            }

            return PagedList<Course>.CreatePagedList(collection, parameters.PageNumber, parameters.PageSize);
        }

        public void UpdateCourse(Course course)
        {
            //Just for Redability
        }

        public void AddAuthor(Author author)
        {
            if (author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            // the repository fills the id
            if(author.Id == Guid.Empty)
                author.Id = Guid.NewGuid();

            foreach (var course in author.Courses)
            {
                course.Id = Guid.NewGuid();
            }

            _context.Authors.Add(author);
        }

        public bool AuthorExists(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return _context.Authors.Any(a => a.Id == authorId);
        }

        public void DeleteAuthor(Author author)
        {
            if (author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            _context.Authors.Remove(author);
        }

        public Author GetAuthor(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return _context.Authors.FirstOrDefault(a => a.Id == authorId);
        }

        public PagedList<Author> GetAuthors(AuthorsResourceParameters authorsParameters)
        {
            // Get the IQuerable authors for better performance manners
            var collection = _context.Authors as IQueryable<Author>;

            if(!string.IsNullOrWhiteSpace(authorsParameters.mainCategory))
            {
                var mainCategory = authorsParameters.mainCategory.Trim();
                collection = collection.Where(a => a.MainCategory == mainCategory);
            }

            if(!string.IsNullOrWhiteSpace(authorsParameters.searchQuery))
            {
                var searchQuery = authorsParameters.searchQuery.Trim();
                collection = collection
                                    .Where(a => a.MainCategory.Contains(searchQuery) 
                                               ||a.FirstName.Contains(searchQuery)
                                               || a.LastName.Contains(searchQuery));
            }

            return PagedList<Author>.CreatePagedList(
                collection,
                authorsParameters.PageNumber, 
                authorsParameters.PageSize);
        }
        public IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorIds)
        {
            if (authorIds == null)
            {
                throw new ArgumentNullException(nameof(authorIds));
            }

            return _context.Authors.Where(a => authorIds.Contains(a.Id))
                .OrderBy(a => a.FirstName)
                .OrderBy(a => a.LastName)
                .ToList();
        }

        public void UpdateAuthor(Author author)
        {
            //Just for Redability
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose resources when needed
            }
        }
    }
}
