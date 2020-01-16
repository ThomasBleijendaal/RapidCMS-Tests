using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Repositories;
using RapidCMSTests.EFCore;
using RapidCMSTests.Entities;
using RapidCMSTests.Models.Cms;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using RapidCMS.Core.Abstractions.Forms;

namespace RapidCMSTests.Repositories
{
    public class PersonRepository : MappedBaseRepository<PersonCmsModel, Person>
    {
        private readonly TestDbContext _dbContext;

        public PersonRepository(TestDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task DeleteAsync(string id, IParent? parent)
        {
            int.TryParse(id, out var entityId);

            var person = await _dbContext.People.FirstOrDefaultAsync(x => x.Id == entityId);
            _dbContext.People.Remove(person);
            await _dbContext.SaveChangesAsync();
        }

        public override async Task<IEnumerable<PersonCmsModel>> GetAllAsync(IParent? parent, IQuery<Person> query)
        {
            var parentId = int.TryParse(parent?.Entity.Id, out var id) ? id : default(int?);

            var queryable = _dbContext.People.Where(x => x.ParentId == parentId);

            if (query.SearchTerm != null)
            {
                queryable = queryable.Where(x => x.Name.Contains(query.SearchTerm));
            }

            if (query.DataViewExpression != null)
            {
                queryable = queryable.Where(query.DataViewExpression);
            }

            queryable = query.ApplyOrder(queryable).Skip(query.Skip).Take(query.Take + 1);

            var results = await queryable.ToListAsync();

            query.HasMoreData(results.Count > query.Take);

            return results.Take(query.Take).Select(x => (PersonCmsModel)x);
        }

        public override async Task<PersonCmsModel?> GetByIdAsync(string id, IParent? parent)
        {
            int.TryParse(id, out var entityId);

            return await _dbContext.People.FirstOrDefaultAsync(x => x.Id == entityId);
        }

        public override Task<PersonCmsModel> NewAsync(IParent? parent, Type? variantType = null)
        {
            return Task.FromResult(new PersonCmsModel
            {
                ParentId = parent?.Entity.Id
            });
        }

        public override async Task<PersonCmsModel?> InsertAsync(IEditContext<PersonCmsModel> editContext)
        {
            var person = (Person)editContext.Entity;

            var entry = _dbContext.People.Add(person);
            await _dbContext.SaveChangesAsync();

            return entry.Entity;
        }

        public override async Task UpdateAsync(IEditContext<PersonCmsModel> editContext)
        {
            var person = (Person)editContext.Entity;

            var entity = await _dbContext.People.FirstOrDefaultAsync(x => x.Id == person.Id);

            entity.Name = person.Name;
            entity.ParentId = person.ParentId;

            _dbContext.People.Update(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
